using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using BotEngineClient;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpAdbClient;

namespace BotEngine
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }
        private static ILogger _logger;
        private static BOTConfig botGameConfig;
        private static bool reloadBOTGameConfig;
        private static BOTListConfig botListConfig;
        private static bool reloadBOTListConfig;
        private static BOTDeviceConfig botDeviceConfig;
        private static BOTDeviceConfig botDeviceConfigNew;
        private static bool reloadBOTDeviceConfig;
        private static Options options;
        private static bool cancelRequested;
        private static FileSystemWatcher gameConfigWatcher;
        private static string gameConfigFileName;
        private static FileSystemWatcher listWatcher;
        private static string listConfigFileName;
        private static FileSystemWatcher deviceWatcher;
        private static string deviceConfigFileName;
        private static ServiceCollection services;


        private enum ConsoleKeyPressEnum
        {
            Nothing,
            Pause,
            Exit
        }

        public class Options
        {
            [Option('d', "Device", Required = false, HelpText = "The device string that is returned by ADB.exe.  At least the following is required \"emulator-5556 device\".")]
            public string DeviceString { get; set; }

            [Option('g', "GameConfig", Required = true, HelpText = "The name of the json file that contains the Search Strings and Actions.")]
            public string ConfigFileName { get; set; }

            [Option('i', "ListConfig", Required = true, HelpText = "The name of the json file that contains the List Settings.")]
            public string ListConfigFileName { get; set; }

            [Option('c', "DeviceConfig", Required = true, HelpText = "The name of the json file that contains the last time actions were taken for a device.  If the file doesn't exist it will be created.")]
            public string DeviceFileName { get; set; }

            [Option('p', "ADBPath", Required = true, HelpText = "The path to where you have ADB.exe")]
            public string ADBPath { get; set; }

            [Option('s', "SleepTime", Required = false, HelpText = "The number of milliseconds to sleep between each loop of Actions.", Default = 500)]
            public int SleepTime { get; set; }

            [Option('l', "LogLevel", Required = false, HelpText = "The level of output from the logger (None, Critical, Error, Warning, Information, Debug, Trace).", Default = "Warning")]
            public string LogLevel { get; set; }

        }

        static int Main(string[] args)
        {
            int result = 0;
            cancelRequested = false;
            reloadBOTListConfig = false;
            reloadBOTDeviceConfig = false;
            reloadBOTGameConfig = false;
            botGameConfig = new BOTConfig();
            botListConfig = new BOTListConfig();
            botDeviceConfig = new BOTDeviceConfig();
            botDeviceConfigNew = new BOTDeviceConfig();

            services = new ServiceCollection();

            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);

            var parser = new Parser(with => { with.EnableDashDash = true; with.IgnoreUnknownArguments = true; });
            ParserResult<Options> parserResult = parser.ParseArguments<Options>(args);

            parserResult
                .WithParsed<Options>(opt => { options = opt; result = ReconfigureLogging(opt); })
                .WithNotParsed<Options>(x =>
                {
                    var helpText = HelpText.AutoBuild(parserResult, h =>
                    {
                        h.AutoHelp = true;     // hides --help
                        h.AutoVersion = true;  // hides --version
                        return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                    }, e => e);
                    Console.WriteLine(helpText);
                    result = -1;
                });
            if (result == 0)
            {
                parserResult
                    .WithParsed<Options>(opt => { result = RunOptionsAndReturnExitCode(opt); });

                if (options != null)
                {
                    SaveDeviceConfigJson();
                }
            }
            if (deviceWatcher != null)
            {
                deviceWatcher.EnableRaisingEvents = false;
                deviceWatcher.Dispose();
            }
            return result;
        }

        private static void SaveDeviceConfigJson()
        {
            if (botDeviceConfig.LastActionTaken.Count > 0)
            {
                try
                {
                    if (deviceWatcher != null) deviceWatcher.EnableRaisingEvents = false;
                    string jsonData = JsonSerializer.Serialize<BOTDeviceConfig>(botDeviceConfig, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = false });
                    File.WriteAllText(options.DeviceFileName, jsonData);
                    _logger.LogInformation("Device JSON file {0} Saved", options.DeviceFileName);
                    if (deviceWatcher != null) deviceWatcher.EnableRaisingEvents = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception thrown when writing {0}", options.DeviceFileName);
                }
            }
        }

        private static int ReconfigureLogging(Options opt)
        {
            ServiceProvider = ConfigureServices(opt);
            _logger = (ILogger)ServiceProvider.GetService(typeof(ILogger));
            return 0;
        }

        /// <summary>
        /// Event handler for Ctrl-C or Break.
        /// Sets that a cancel has been requested, and prevents the program from auto exiting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected static void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            _logger.LogCritical("Control C Captured");
            args.Cancel = true;
            cancelRequested = true;
        }

        /// <summary>
        /// Core execution engine for the bot.
        /// Loads the json config files, and runs a loop until exit keyboard command (x/X) or Ctrl-C pressed
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int RunOptionsAndReturnExitCode(Options o)
        {
            using (_logger.BeginScope(BotEngineClient.Helpers.CurrentMethodName()))
            {
                var exitCode = 0;
                _logger.LogDebug("Ensure that files exist");
                if(!File.Exists(o.ConfigFileName))
                {
                    _logger.LogError("Game Config file {0} does not exist.  Exiting.", o.ConfigFileName);
                    return -2;
                }
                if (!File.Exists(o.ListConfigFileName))
                {
                    _logger.LogError("List Config file {0} does not exist.  Exiting.", o.ListConfigFileName);
                    return -2;
                }

                _logger.LogDebug("Device String is {0}", o.DeviceString);
                AdbServer adbServer = new AdbServer();
                StartServerResult result = adbServer.StartServer(o.ADBPath, restartServerIfNewer: true);
                _logger.LogInformation("Starting ADB Server status {0}", result.ToString());
                if (result != StartServerResult.AlreadyRunning)
                {
                    Thread.Sleep(1500);
                    AdbServerStatus status = adbServer.GetStatus();
                    _logger.LogDebug("ADB Server status {0}", status.ToString());
                    if (!status.IsRunning)
                    {
                        _logger.LogError("Unable to start ADB Server");
                        throw new Exception("Unable to start ADB server");
                    }
                }
                AdbClient client = new AdbClient();
                List<DeviceData> devices = new List<DeviceData>();
                for (int i = 0; i < 5; i++)
                {
                    devices = client.GetDevices();
                    if (devices.Count > 0)
                        break;
                    _logger.LogWarning("No ADB Clients Found");
                    Thread.Sleep(1500);
                }
                if (devices.Count == 0)
                {
                    _logger.LogError("No ADB Clients Found");
                    exitCode = -2;
                }
                else
                {
                    string deviceId;
                    if (o.DeviceString is null)
                    {
                        DeviceData device = devices[0];
                        string deviceState = device.State == DeviceState.Online ? "device" : device.State.ToString().ToLower();
                        deviceId = String.Format("{0} {1} product:{2} model:{3} device:{4} features:{5}  transport_id:{6}", device.Serial, deviceState, device.Product, device.Model, device.Name, device.Features, device.TransportId);
                    }
                    else
                    {
                        deviceId = o.DeviceString;
                    }
                    string jsonString = string.Empty;
                    try
                    {
                        jsonString = File.ReadAllText(o.ConfigFileName);
                        botGameConfig = JsonSerializer.Deserialize<BOTConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
                    }
                    catch (JsonException jse)
                    {
                        _logger.LogError(jse, "JSON file format error reading {0}", o.ConfigFileName);
                        return -2;
                    }

                    try
                    {
                        jsonString = File.ReadAllText(o.ListConfigFileName);
                        botListConfig = JsonSerializer.Deserialize<BOTListConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
                    }
                    catch (JsonException jse)
                    {
                        _logger.LogError(jse, "JSON file format error reading {0}", o.ListConfigFileName);
                        return -2;
                    }

                    if (!File.Exists(o.DeviceFileName))
                    {
                        botDeviceConfig.FileId = "DeviceConfig";
                        botDeviceConfig.LastActionTaken = new Dictionary<string, ActionActivity>();
                        foreach (KeyValuePair<string, BotEngineClient.Action> item in botGameConfig.Actions)
                        {
                            if (item.Value.ActionType.ToLower() == "scheduled" || item.Value.ActionType.ToLower() == "daily")
                            {
                                ActionActivity actionActivity = new ActionActivity
                                {
                                    LastRun = DateTime.MinValue,
                                    ActionEnabled = true
                                };
                                botDeviceConfig.LastActionTaken.Add(item.Key, actionActivity);
                            }
                            else if (item.Value.ActionType.ToLower() == "adhoc")
                            {
                                ActionActivity actionActivity = new ActionActivity
                                {
                                    LastRun = DateTime.MinValue,
                                    ActionEnabled = false
                                };
                                botDeviceConfig.LastActionTaken.Add(item.Key, actionActivity);
                            }
                        }
                        string jsonData = JsonSerializer.Serialize<BOTDeviceConfig>(botDeviceConfig, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = false });
                        File.WriteAllText(o.DeviceFileName, jsonData);
                        _logger.LogInformation("Device JSON file {0} Saved", o.DeviceFileName);
                    }
                    else
                    {
                        try
                        {
                            jsonString = File.ReadAllText(o.DeviceFileName);
                            botDeviceConfig = JsonSerializer.Deserialize<BOTDeviceConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
                        }
                        catch (JsonException jse)
                        {
                            _logger.LogError(jse, "JSON file format error reading {0}", o.DeviceFileName);
                            return -2;
                        }
                    }

                    ValidateAndUpdateDeviceConfig(botDeviceConfig, botGameConfig);

                    gameConfigWatcher = new FileSystemWatcher(Path.GetDirectoryName(o.ConfigFileName))
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.Attributes,
                        Filter = "*.json"
                    };
                    gameConfigFileName = o.ConfigFileName;
                    gameConfigWatcher.Changed += ReloadJSONConfig;
                    gameConfigWatcher.Renamed += ReloadJSONConfig;
                    gameConfigWatcher.EnableRaisingEvents = true;

                    listWatcher = new FileSystemWatcher(Path.GetDirectoryName(o.ListConfigFileName))
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.Attributes,
                        Filter = Path.GetFileName(o.ListConfigFileName)
                    };
                    listConfigFileName = o.ListConfigFileName;
                    listWatcher.Changed += ReloadJSONConfig;
                    listWatcher.Renamed += ReloadJSONConfig;
                    listWatcher.EnableRaisingEvents = true;

                    deviceWatcher = new FileSystemWatcher(Path.GetDirectoryName(o.DeviceFileName))
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.Attributes,
                        Filter = Path.GetFileName(o.DeviceFileName)
                    };
                    deviceConfigFileName = o.DeviceFileName;
                    deviceWatcher.Changed += ReloadJSONConfig;
                    deviceWatcher.Renamed += ReloadJSONConfig;
                    deviceWatcher.EnableRaisingEvents = true;

                    Dictionary<string, BotEngineClient.Action> Actions = botGameConfig.Actions;
                    // Validate the BeforeAction and AfterActions...

                    bool configErrors = false;
                    foreach (KeyValuePair<string, BotEngineClient.Action> item in Actions)
                    {
                        if (item.Value.BeforeAction != null && !botGameConfig.Actions.ContainsKey(item.Value.BeforeAction) && !botGameConfig.SystemActions.ContainsKey(item.Value.BeforeAction))
                        {
                            _logger.LogError("BeforeAction {0} on Action {1} does not exist in {2}", item.Value.BeforeAction, item.Key, o.ConfigFileName);
                            configErrors = true;
                        }
                        if (item.Value.AfterAction != null && !botGameConfig.Actions.ContainsKey(item.Value.AfterAction) && !botGameConfig.SystemActions.ContainsKey(item.Value.AfterAction))
                        {
                            _logger.LogError("AfterAction {0} on Action {1} does not exist in {2}", item.Value.AfterAction, item.Key, o.ConfigFileName);
                            configErrors = true;
                        }
                    }

                    if (configErrors)
                        return -2;

                    BotEngineClient.BotEngine bot = new BotEngineClient.BotEngine(ServiceProvider, o.ADBPath, deviceId, botGameConfig.FindStrings, botGameConfig.SystemActions, botGameConfig.Actions, botListConfig);
                    BotEngineClient.BotEngine.CommandResults cr = BotEngineClient.BotEngine.CommandResults.Ok;
                    DateTime tenMinuteDateTime = DateTime.MinValue;
                    bool paused = false;
                    do
                    {
                        if (reloadBOTListConfig)
                        {
                            bot.ReloadListConfig(botListConfig);
                            reloadBOTListConfig = false;
                        }

                        if (reloadBOTGameConfig)
                        {
                            Actions = botGameConfig.Actions;
                            bot.ReloadFindStrings(botGameConfig.FindStrings);
                            bot.ReloadNormalActions(botGameConfig.Actions);
                            bot.ReloadSystemActions(botGameConfig.SystemActions);
                            ValidateAndUpdateDeviceConfig(botDeviceConfig, botGameConfig);
                            reloadBOTGameConfig = false;
                        }

                        if (reloadBOTDeviceConfig)
                        {
                            RefreshDeviceConfig(botDeviceConfig, botDeviceConfigNew);
                            ValidateAndUpdateDeviceConfig(botDeviceConfig, botGameConfig);
                            reloadBOTDeviceConfig = false;
                            tenMinuteDateTime = DateTime.Now;
                        }

                        if ((DateTime.Now - tenMinuteDateTime).TotalMinutes >= 10)
                        {
                            tenMinuteDateTime = DateTime.Now;
                            SaveDeviceConfigJson();
                            ShowBotStatus(Actions, botDeviceConfig.LastActionTaken);
                        }
                        _ = HandleKeyboard(Actions, botDeviceConfig.LastActionTaken, ref paused);
                        if ((!paused) && (!cancelRequested))
                        {
                            foreach (KeyValuePair<string, BotEngineClient.Action> item in Actions)
                            {
                                _ = HandleKeyboard(Actions, botDeviceConfig.LastActionTaken, ref paused);
                                if (cancelRequested || paused)
                                    break;
                                if (Enum.TryParse(item.Value.ActionType, true, out ValidActionType validActionType))
                                {
                                    if (   ((validActionType == ValidActionType.Scheduled) && (botDeviceConfig.LastActionTaken[item.Key].ActionEnabled))
                                        || ((validActionType == ValidActionType.Daily) && (botDeviceConfig.LastActionTaken[item.Key].ActionEnabled))
                                        || ((validActionType == ValidActionType.Adhoc) && (botDeviceConfig.LastActionTaken[item.Key].ActionEnabled))
                                        )
                                    {
                                        if (item.Value.ExecuteDue(botDeviceConfig.LastActionTaken[item.Key]))
                                        {
                                            if (item.Value.BeforeAction != null)
                                            {
                                                // ToDo: Make these Async, and support Cancelation Tokens, so Pause etc. can stop execution
                                                cr = bot.ExecuteAction(item.Value.BeforeAction);
                                                if (cr == BotEngineClient.BotEngine.CommandResults.ADBError)
                                                    break;
                                            }
                                            _ = HandleKeyboard(Actions, botDeviceConfig.LastActionTaken, ref paused);
                                            if (cancelRequested || paused)
                                                break;
                                            // ToDo: Make these Async, and support Cancelation Tokens, so Pause etc. can stop execution
                                            cr = bot.ExecuteAction(item.Key);
                                            _logger.LogInformation(string.Format("Action result was {0}", cr));
                                            _ = HandleKeyboard(Actions, botDeviceConfig.LastActionTaken, ref paused);
                                            if (cancelRequested || paused)
                                                break;
                                            if (cr == BotEngineClient.BotEngine.CommandResults.ADBError)
                                                break;
                                            if (cr == BotEngineClient.BotEngine.CommandResults.Ok)
                                            {
                                                botDeviceConfig.LastActionTaken[item.Key].MarkExecuted();
                                                if (validActionType == ValidActionType.Adhoc)
                                                    botDeviceConfig.LastActionTaken[item.Key].MarkDisabled();
                                            }
                                            if (item.Value.AfterAction != null)
                                            {
                                                // ToDo: Make these Async, and support Cancelation Tokens, so Pause etc. can stop execution
                                                cr = bot.ExecuteAction(item.Value.AfterAction);
                                                if (cr == BotEngineClient.BotEngine.CommandResults.ADBError)
                                                    break;
                                            }
                                        }
                                    }
                                    else if (validActionType == ValidActionType.Always)
                                    {
                                        // ToDo: Make these Async, and support Cancelation Tokens, so Pause etc. can stop execution
                                        cr = bot.ExecuteAction(item.Key);
                                        if (cr == BotEngineClient.BotEngine.CommandResults.ADBError)
                                            break;
                                    }
                                }
                            }
                        }
                        if (!cancelRequested)
                            Thread.Sleep(options.SleepTime);
                    }
                    while ((cr != BotEngineClient.BotEngine.CommandResults.ADBError) && (cancelRequested == false));
                }

                return exitCode;
            }
        }

        private static ActionActivity? GetOverride(Dictionary<string, ActionActivity> actionActivity, string key)
        {
            if (actionActivity != null && actionActivity.ContainsKey(key))
            {
                return actionActivity[key];
            }
            else
            {
                return null;
            }
        }

        private static void ReloadJSONConfig(object sender, FileSystemEventArgs e)
        {
            if (Path.GetFileName(e.FullPath).ToLower() == Path.GetFileName(gameConfigFileName).ToLower())
            {
                ReloadGameConfig(sender, e);
            }
            else if (Path.GetFileName(e.FullPath).ToLower() == Path.GetFileName(listConfigFileName).ToLower())
            {
                ReloadListConfig(sender, e);
            }
            else if (Path.GetFileName(e.FullPath).ToLower() == Path.GetFileName(deviceConfigFileName).ToLower())
            {
                ReloadDevice(sender, e);
            }
        }

        private static ConsoleKeyPressEnum HandleKeyboard(Dictionary<string, BotEngineClient.Action> Actions, Dictionary<string, ActionActivity> lastActionTaken, ref bool paused)
        {
            ConsoleKeyPressEnum consoleStatus = HandleConsoleKeyBoard(Actions, lastActionTaken);
            if (consoleStatus != ConsoleKeyPressEnum.Nothing)
            {
                if (consoleStatus == ConsoleKeyPressEnum.Pause)
                {
                    paused = !paused;
                    // Save the device status when Pausing, so that it's clean for an external to update it.
                    if (paused)
                        SaveDeviceConfigJson();
                }
                else if (consoleStatus == ConsoleKeyPressEnum.Exit)
                {
                    cancelRequested = true;
                }
            }

            return consoleStatus;
        }

        private static void ShowBotStatus(Dictionary<string, BotEngineClient.Action> Actions, Dictionary<string, ActionActivity> lastActionTaken)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            foreach (KeyValuePair<string, BotEngineClient.Action> item in Actions)
            {
                if (item.Value.ActionType.ToLower() == "scheduled" || item.Value.ActionType.ToLower() == "daily")
                {
                    sb.AppendFormat("Action {0} is {1} and due at {2}", item.Key, lastActionTaken[item.Key].ActionEnabled ? "Enabled":"Disabled", item.Value.NextExecuteDue(lastActionTaken[item.Key]));
                    sb.AppendLine();
                }
            }
            // Log as Warning, so that it will show by default.
            _logger.LogWarning(sb.ToString());
        }

        private static ConsoleKeyPressEnum HandleConsoleKeyBoard(Dictionary<string, BotEngineClient.Action> actions, Dictionary<string, ActionActivity> lastActionTaken)
        {
            ConsoleKeyPressEnum result = ConsoleKeyPressEnum.Nothing;
            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case 'h':
                    case 'H':
                        _logger.LogWarning("In Console Help:\r\nh/H : Help\r\np/P : Pause\r\ns/S : Execution Status\r\nx/X : Exit");
                        Thread.Sleep(2000);
                        result = ConsoleKeyPressEnum.Nothing;
                        break;
                    case 'p':
                    case 'P':
                        _logger.LogWarning("In Console Command Pause Received");
                        result = ConsoleKeyPressEnum.Pause;
                        break;
                    case 's':
                    case 'S':
                        ShowBotStatus(actions, lastActionTaken);
                        Thread.Sleep(1000);
                        result = ConsoleKeyPressEnum.Nothing;
                        break;
                    case 'x':
                    case 'X':
                        _logger.LogWarning("In Console Command Exit Received");
                        result = ConsoleKeyPressEnum.Exit;
                        break;
                    default:
                        result = ConsoleKeyPressEnum.Nothing;
                        break;
                }
            }
            return result;
        }

        private static void ValidateAndUpdateDeviceConfig(BOTDeviceConfig botDeviceConfig, BOTConfig botGameConfig)
        {
            foreach (KeyValuePair<string, BotEngineClient.Action> item in botGameConfig.Actions)
            {
                if (item.Value.ActionType.ToLower() == "scheduled" || item.Value.ActionType.ToLower() == "daily" || item.Value.ActionType.ToLower() == "adhoc")
                {
                    if (!botDeviceConfig.LastActionTaken.ContainsKey(item.Key))
                    {
                        ActionActivity actionActivty = new ActionActivity
                        {
                            ActionEnabled = true,
                            LastRun = DateTime.MinValue
                        };
                        botDeviceConfig.LastActionTaken.Add(item.Key, actionActivty);
                    }
                }
            }
        }

        private static void RefreshDeviceConfig(BOTDeviceConfig botDeviceConfig, BOTDeviceConfig botDeviceConfigNew)
        {
            foreach (KeyValuePair<string, ActionActivity> item in botDeviceConfigNew.LastActionTaken)
            {
                if (botDeviceConfig.LastActionTaken.ContainsKey(item.Key))
                {
                    botDeviceConfig.LastActionTaken[item.Key].ActionEnabled = item.Value.ActionEnabled;
                    botDeviceConfig.LastActionTaken[item.Key].DailyScheduledTime = item.Value.DailyScheduledTime;
                    botDeviceConfig.LastActionTaken[item.Key].Frequency = item.Value.Frequency;
                    // Allow update of the LastRun if the change is greater than 15 minutes.
                    // This should preclude the autosave (10 minutes) from changing the value.
                    if (Math.Abs((botDeviceConfig.LastActionTaken[item.Key].LastRun - item.Value.LastRun).TotalMinutes) > 15)
                    {
                        botDeviceConfig.LastActionTaken[item.Key].LastRun = item.Value.LastRun;
                    }
                }
                else
                {
                    ActionActivity actionActivty = new ActionActivity
                    {
                        ActionEnabled = item.Value.ActionEnabled,
                        LastRun = item.Value.LastRun,
                        DailyScheduledTime = item.Value.DailyScheduledTime,
                        Frequency = item.Value.Frequency
                    };
                    botDeviceConfig.LastActionTaken.Add(item.Key, actionActivty);
                }
            }
        }

        private static void ReloadDevice(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("Changes detected in {0}, reloading", e.FullPath);
            try
            {
                if (ReloadJsonString(e, out string jsonString))
                {
                    botDeviceConfigNew = JsonSerializer.Deserialize<BOTDeviceConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
                    reloadBOTDeviceConfig = true;
                }
            }
            catch (JsonException jse)
            {
                _logger.LogError(jse, "JSON file format error reading {0}", e.FullPath);
            }
        }

        private static void ReloadGameConfig(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("Changes detected in {0}, reloading", e.FullPath);
            try
            {
                if (ReloadJsonString(e, out string jsonString))
                {

                    botGameConfig = JsonSerializer.Deserialize<BOTConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
                    reloadBOTGameConfig = true;
                }
            }
            catch (JsonException jse)
            {
                _logger.LogError(jse, "JSON file format error reading {0}", e.FullPath);
            }
        }

        private static void ReloadListConfig(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("Changes detected in {0}, reloading", e.FullPath);
            try
            {
                if (ReloadJsonString(e, out string jsonString))
                {
                    botListConfig = JsonSerializer.Deserialize<BOTListConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
                    reloadBOTListConfig = true;
                }
            }
            catch (JsonException jse)
            {
                _logger.LogError(jse, "JSON file format error reading {0}", e.FullPath);
            }
        }

        private static bool ReloadJsonString(FileSystemEventArgs e, out string jsonString)
        {
            jsonString = string.Empty;
            int retries = 0;
            do
            {
                try
                {
                    jsonString = File.ReadAllText(e.FullPath);
                }
                catch (IOException ex)
                {
                    if (retries++ > 5)
                    {
                        _logger.LogError(ex, "To many File IO Exceptions on {0}, abort refresh", e.FullPath);
                        return false;
                    }
                    _logger.LogInformation("File IO Exception on {0}, retrying soon", e.FullPath);
                    Thread.Sleep(250);
                }
            } while (jsonString == string.Empty);
            return true;
        }

        public static IServiceProvider ConfigureServices(Options opt)
        {
            //services.Clear();
            LogLevel logLevel = LogLevel.Information;
            logLevel = opt.LogLevel.ToLower() switch
            {
                "none" => LogLevel.None,
                "critical" => LogLevel.Critical,
                "error" => LogLevel.Error,
                "warning" => LogLevel.Warning,
                "information" => LogLevel.Information,
                "debug" => LogLevel.Debug,
                "trace" => LogLevel.Trace,
                _ => LogLevel.Warning,
            };
            if (logLevel == LogLevel.Debug)
            {
                services.AddLogging(loggingBuilder => loggingBuilder
                  .AddSimpleConsole(options =>
                  {
                      options.IncludeScopes = true;
                      options.SingleLine = false;
                      options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.ffffff ";
                      options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                  })
                  .AddDebug()
                  .SetMinimumLevel(logLevel)
                  );
            }
            else
            {
                services.AddLogging(loggingBuilder => loggingBuilder
                .AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = false;
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.ffffff ";
                    options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                })
                 .SetMinimumLevel(logLevel)
                );
            }
            services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("MoEBot"));
            return services.BuildServiceProvider();
        }
    }
}
