// <copyright file="Program.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using BotEngineClient;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AdvancedSharpAdbClient;
using Win32FrameBufferClient;
using AdvancedSharpAdbClient.Models;

namespace BotEngine
{
    partial class Program
    {
        public static ServiceProvider ServiceProvider { get; set; }
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
        private static bool exitRequested;
        private static FileSystemWatcher gameConfigWatcher;
        private static string gameConfigFileName;
        private static FileSystemWatcher listWatcher;
        private static string listConfigFileName;
        private static FileSystemWatcher deviceWatcher;
        private static string deviceConfigFileName;
        private static ServiceCollection services;

        private static BotEngineClient.BotEngine.CommandResults threadResult;
        private static CancellationTokenSource threadCTS;

        private static bool UseWin32;

        private static Win32FrameBuffer _win32FrameBuffer;

        private enum ConsoleKeyPressEnum
        {
            Nothing,
            Pause,
            Exit
        }

        // ToDo: Add Exception Handling SharpAdbClient.Exceptions.AdbException


        #region Main Control Logic

        /// <summary>
        /// The entry point to the entire 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {
            int result = 0;
            cancelRequested = false;
            exitRequested = false;
            reloadBOTListConfig = false;
            reloadBOTDeviceConfig = false;
            reloadBOTGameConfig = false;
            botGameConfig = new BOTConfig();
            botListConfig = new BOTListConfig();
            botDeviceConfig = new BOTDeviceConfig();
            botDeviceConfigNew = new BOTDeviceConfig();
            threadCTS = null;

            UseWin32 = false;
            _win32FrameBuffer = null;

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

                if (options != null && !options.ListDevices)
                {
                    SaveDeviceConfigJson();
                }
            }
            if (deviceWatcher != null)
            {
                deviceWatcher.EnableRaisingEvents = false;
                deviceWatcher.Dispose();
            }

            if (ServiceProvider != null)
            {
                try
                {
                    ILoggerFactory loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
                    loggerFactory.Dispose();
                }
                catch (Exception)
                {
                    // Who cares, we are trying to get out of here anyway.
                }
                ServiceProvider.Dispose();
            }
            return result;
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
                if (o.ListDevices)
                {
                    exitCode = ListDevicesThatADBCanSee(o, exitCode);
                }
                else
                {
                    JsonHelper jsonHelper = new JsonHelper();
                    JsonHelper.ConfigFileType fileType;

                    #region Setup Win32

                    if (!string.IsNullOrEmpty(o.ProcessName) && !string.IsNullOrEmpty(o.WindowName))
                    {
                        try
                        {
                            _win32FrameBuffer = new Win32FrameBuffer(o.ProcessName, o.WindowName);

                            _win32FrameBuffer.ImageSize = new System.Drawing.Rectangle(o.TopLeftX, o.TopLeftY, o.Width, o.Height);
                            UseWin32 = _win32FrameBuffer.IsValid();
                        }
                        catch (Exception ex)
                        {
                            UseWin32 = false;
                            _win32FrameBuffer = null;
                            _logger.LogError("Unable to initialise Win32 Graphics Capture, falling back to ADB. Exception was:\r\n{0}", ex.Message);
                        }
                    }

                    #endregion

                    #region Validate Files
                    _logger.LogDebug("Ensure that files exist");
                    if (!File.Exists(o.ConfigFileName))
                    {
                        _logger.LogError("Game Config file {0} does not exist.  Exiting.", o.ConfigFileName);
                        return -2;
                    }
                    else
                    {
                        fileType = jsonHelper.GetFileType(o.ConfigFileName);
                        if (fileType != JsonHelper.ConfigFileType.GameConfig)
                        {
                            _logger.LogError("Game Config file {0} is not a GameConfig, but a {1}.  Exiting.", o.ConfigFileName, fileType);
                            return -3;
                        }
                        else
                        {
                            if (!jsonHelper.ValidateGameConfigStructure(o.ConfigFileName))
                            {
                                _logger.LogError("Game Config file {0} has errors as follows:", o.ConfigFileName);
                                foreach (string item in jsonHelper.Errors)
                                {
                                    _logger.LogError(item);
                                }
                                return -4;
                            }
                        }
                    }
                    if (!File.Exists(o.ListConfigFileName))
                    {
                        _logger.LogError("List Config file {0} does not exist.  Exiting.", o.ListConfigFileName);
                        return -2;
                    }
                    else
                    {
                        fileType = jsonHelper.GetFileType(o.ListConfigFileName);
                        if (fileType != JsonHelper.ConfigFileType.ListConfig)
                        {
                            _logger.LogError("List Config file {0} is not a ListConfig, but a {1}.  Exiting.", o.ListConfigFileName, fileType);
                            return -3;
                        }
                        else
                        {
                            if (!jsonHelper.ValidateListConfigStructure(o.ListConfigFileName))
                            {
                                _logger.LogError("List Config file {0} has errors as follows:", o.ListConfigFileName);
                                foreach (string item in jsonHelper.Errors)
                                {
                                    _logger.LogError(item);
                                }
                                return -4;
                            }
                        }
                    }
                    if (File.Exists(o.DeviceFileName))
                    {
                        fileType = jsonHelper.GetFileType(o.DeviceFileName);
                        if (fileType != JsonHelper.ConfigFileType.DeviceConfig)
                        {
                            _logger.LogError("Device Config file {0} is not a DeviceConfig, but a {1}.  Exiting.", o.ConfigFileName, fileType);
                            return -3;
                        }
                        else
                        {
                            if (!jsonHelper.ValidateDeviceConfigStructure(o.DeviceFileName))
                            {
                                _logger.LogError("Device Config file {0} has errors as follows:", o.DeviceFileName);
                                foreach (string item in jsonHelper.Errors)
                                {
                                    _logger.LogError(item);
                                }
                                return -4;
                            }
                        }
                    }
                    #endregion

                    #region Startup ADB
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
                    IEnumerable<DeviceData> devices = new List<DeviceData>();
                    for (int i = 0; i < 5; i++)
                    {
                        devices = client.GetDevices();
                        if (devices.Count() > 0)
                            break;
                        _logger.LogWarning("No ADB Clients Found");
                        Thread.Sleep(1500);
                    }
                    #endregion

                    if (devices.Count() == 0)
                    {
                        _logger.LogError("No ADB Clients Found");
                        exitCode = -2;
                    }
                    else
                    {
                        int returnValue = 0;
                        
                        returnValue = LoadGameConfig(o);
                        if (returnValue != 0)
                            return returnValue;
                        returnValue = LoadListConfig(o);
                        if (returnValue != 0)
                            return returnValue;
                        returnValue = LoadOrBuildDeviceConfig(o);
                        if (returnValue != 0)
                            return returnValue;

                        ValidateAndUpdateDeviceConfig(botDeviceConfig, botGameConfig);

                        #region Setup File Watchers

                        gameConfigWatcher = new FileSystemWatcher(Path.GetDirectoryName(o.ConfigFileName)) {
                            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.Attributes,
                            Filter = "*.json"
                        };
                        gameConfigFileName = o.ConfigFileName;
                        gameConfigWatcher.Changed += ReloadJSONConfig;
                        gameConfigWatcher.Renamed += ReloadJSONConfig;
                        gameConfigWatcher.EnableRaisingEvents = true;

                        listWatcher = new FileSystemWatcher(Path.GetDirectoryName(o.ListConfigFileName)) {
                            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.Attributes,
                            Filter = Path.GetFileName(o.ListConfigFileName)
                        };
                        listConfigFileName = o.ListConfigFileName;
                        listWatcher.Changed += ReloadJSONConfig;
                        listWatcher.Renamed += ReloadJSONConfig;
                        listWatcher.EnableRaisingEvents = true;

                        deviceWatcher = new FileSystemWatcher(Path.GetDirectoryName(o.DeviceFileName)) {
                            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.Attributes,
                            Filter = Path.GetFileName(o.DeviceFileName)
                        };
                        deviceConfigFileName = o.DeviceFileName;
                        deviceWatcher.Changed += ReloadJSONConfig;
                        deviceWatcher.Renamed += ReloadJSONConfig;
                        deviceWatcher.EnableRaisingEvents = true;
                        #endregion

                        #region Validate the loaded Actions
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
                        #endregion

                        BotEngineClient.BotEngine bot = new BotEngineClient.BotEngine(ServiceProvider, o.ADBPath, GetDeviceId(o, devices), botGameConfig.FindStrings, botGameConfig.SystemActions, botGameConfig.Actions, botListConfig, UseWin32, _win32FrameBuffer);
                        BotEngineClient.BotEngine.CommandResults cr = BotEngineClient.BotEngine.CommandResults.Ok;
                        DateTime tenMinuteDateTime = DateTime.Now;
                        bool paused = false;
                        threadCTS = new CancellationTokenSource();
                        Thread botThread;

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
                                        if (((validActionType == ValidActionType.Scheduled) && (botDeviceConfig.LastActionTaken[item.Key].ActionEnabled))
                                            || ((validActionType == ValidActionType.Daily) && (botDeviceConfig.LastActionTaken[item.Key].ActionEnabled))
                                            || ((validActionType == ValidActionType.Adhoc) && (botDeviceConfig.LastActionTaken[item.Key].ActionEnabled))
                                            )
                                        {
                                            if (item.Value.ExecuteDue(botDeviceConfig.LastActionTaken[item.Key]))
                                            {
                                                if (item.Value.BeforeAction != null)
                                                {
                                                    bot.SetThreadingCommand(item.Value.BeforeAction, ResultCallback, botDeviceConfig.LastActionTaken[item.Key]);
                                                    botThread = new Thread(bot.InitiateThreadingCommand);
                                                    botThread.Start(threadCTS.Token);
                                                    botThread.Join();
                                                    cr = threadResult;
                                                    if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngineClient.BotEngine.CommandResults.Cancelled)
                                                        break;
                                                }
                                                _ = HandleKeyboard(Actions, botDeviceConfig.LastActionTaken, ref paused);
                                                if (cancelRequested || paused)
                                                    break;

                                                bot.SetThreadingCommand(item.Key, ResultCallback, botDeviceConfig.LastActionTaken[item.Key]);
                                                botThread = new Thread(bot.InitiateThreadingCommand);
                                                botThread.Start(threadCTS.Token);
                                                botThread.Join();
                                                cr = threadResult;

                                                _logger.LogInformation(string.Format("Action result was {0}", cr));
                                                _ = HandleKeyboard(Actions, botDeviceConfig.LastActionTaken, ref paused);
                                                if (cancelRequested || paused)
                                                    break;
                                                if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngineClient.BotEngine.CommandResults.Cancelled)
                                                    break;
                                                if (cr == BotEngineClient.BotEngine.CommandResults.Ok)
                                                {
                                                    botDeviceConfig.LastActionTaken[item.Key].MarkExecuted();
                                                    if (validActionType == ValidActionType.Adhoc)
                                                        botDeviceConfig.LastActionTaken[item.Key].MarkDisabled();
                                                }
                                                if (item.Value.AfterAction != null)
                                                {
                                                    bot.SetThreadingCommand(item.Value.AfterAction, ResultCallback, botDeviceConfig.LastActionTaken[item.Key]);
                                                    botThread = new Thread(bot.InitiateThreadingCommand);
                                                    botThread.Start(threadCTS.Token);
                                                    botThread.Join();
                                                    cr = threadResult;
                                                    if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngineClient.BotEngine.CommandResults.Cancelled)
                                                        break;
                                                }
                                            }

                                            // Run all the Always actions
                                            foreach (KeyValuePair<string, BotEngineClient.Action> alwaysItems in Actions)
                                            {
                                                if (Enum.TryParse(item.Value.ActionType, true, out ValidActionType alwaysValidActionType))
                                                    if (alwaysValidActionType == ValidActionType.Always && botDeviceConfig.LastActionTaken[alwaysItems.Key].ActionEnabled)
                                                    {
                                                        bot.SetThreadingCommand(alwaysItems.Key, ResultCallback, botDeviceConfig.LastActionTaken[alwaysItems.Key]);
                                                        botThread = new Thread(bot.InitiateThreadingCommand);
                                                        botThread.Start(threadCTS.Token);
                                                        botThread.Join();
                                                        cr = threadResult;
                                                        if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngineClient.BotEngine.CommandResults.Cancelled)
                                                            break;
                                                    }
                                            }
                                        }
                                        else if (validActionType == ValidActionType.Always && botDeviceConfig.LastActionTaken[item.Key].ActionEnabled)
                                        {
                                            bot.SetThreadingCommand(item.Key, ResultCallback, botDeviceConfig.LastActionTaken[item.Key]);
                                            botThread = new Thread(bot.InitiateThreadingCommand);
                                            botThread.Start(threadCTS.Token);
                                            botThread.Join();
                                            cr = threadResult;
                                            if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngineClient.BotEngine.CommandResults.Cancelled)
                                                break;
                                        }
                                    }
                                }
                            }
                            // On Cancel detected, cleanup, show status, and go into pause mode.
                            if (cancelRequested)
                            {
                                cancelRequested = false;
                                threadCTS.Dispose();
                                threadCTS = new CancellationTokenSource();
                                paused = true;
                                ShowBotStatus(Actions, botDeviceConfig.LastActionTaken);
                            }
                            if (!exitRequested)
                                Thread.Sleep(options.SleepTime);
                        }
                        while ((cr != BotEngineClient.BotEngine.CommandResults.ADBError) && (exitRequested == false));
                    }
                }
                return exitCode;
            }
        }

        


        #region GetDeviceId
        /// <summary>
        /// Creates the string that identifies the device, or gets it from the passed in parameters.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        private static string GetDeviceId(Options o, IEnumerable<DeviceData> devices)
        {
            string deviceId;
            if (o.DeviceString is null)
            {
                DeviceData device = devices.FirstOrDefault();
                string deviceState = device.State == DeviceState.Online ? "device" : device.State.ToString().ToLower();
                deviceId = String.Format("{0} {1} product:{2} model:{3} device:{4} features:{5}  transport_id:{6}", device.Serial, deviceState, device.Product, device.Model, device.Name, device.Features, device.TransportId);
            }
            else
            {
                deviceId = o.DeviceString;
            }

            return deviceId;
        }
        #endregion

        #region ListDevicesThatADBCanSee
        /// <summary>
        /// Lists all the devices that ADB has visibility of
        /// </summary>
        /// <param name="o"></param>
        /// <param name="exitCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static int ListDevicesThatADBCanSee(Options o, int exitCode)
        {
            // List out all the devices that ADB can see
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
            IEnumerable<DeviceData> devices = new List<DeviceData>();
            for (int i = 0; i < 5; i++)
            {
                devices = client.GetDevices();
                if (devices.Count() > 0)
                    break;
                _logger.LogWarning("No ADB Clients Found");
                Thread.Sleep(1500);
            }
            if (devices.Count() == 0)
            {
                _logger.LogError("No ADB Clients Found");
                exitCode = -2;
            }
            else
            {
                Console.WriteLine("Start Device Strings found");
                Console.WriteLine("-----------------------------------------------------------------------");
                foreach (DeviceData device in devices)
                {
                    string deviceState = device.State == DeviceState.Online ? "device" : device.State.ToString().ToLower();
                    string deviceId = String.Format("{0} {1} product:{2} model:{3} device:{4} features:{5}  transport_id:{6}", device.Serial, deviceState, device.Product, device.Model, device.Name, device.Features, device.TransportId);
                    Console.WriteLine("\"{0}\"", deviceId);
                }
                Console.WriteLine("-----------------------------------------------------------------------");
                Console.WriteLine("End Device Strings found");
            }

            return exitCode;
        }
        #endregion

        #endregion




        //ToDo: Remove this if it's not used
        //private static ActionActivity? GetOverride(Dictionary<string, ActionActivity> actionActivity, string key)
        //{
        //    if (actionActivity != null && actionActivity.ContainsKey(key))
        //    {
        //        return actionActivity[key];
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        #region User Intitiated Action Handling
        /// <summary>
        /// Display the status of all Daily and Schedules Actions.
        /// </summary>
        /// <param name="Actions"></param>
        /// <param name="lastActionTaken"></param>
        private static void ShowBotStatus(Dictionary<string, BotEngineClient.Action> Actions, Dictionary<string, ActionActivity> lastActionTaken)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            foreach (KeyValuePair<string, BotEngineClient.Action> item in Actions)
            {
                if (Enum.TryParse(item.Value.ActionType, true, out ValidActionType validActionType))
                {
                    switch (validActionType)
                    {
                        case ValidActionType.Adhoc:
                            break;
                        case ValidActionType.Always:
                            break;
                        case ValidActionType.Daily:
                        case ValidActionType.Scheduled:
                            sb.AppendFormat("Action {0} is {1} and due at {2}", item.Key, lastActionTaken[item.Key].ActionEnabled ? "Enabled" : "Disabled", item.Value.NextExecuteDue(lastActionTaken[item.Key]));
                            sb.AppendLine();
                            break;
                        case ValidActionType.System:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    _logger.LogError("Action {0} has invalid ActionType {1}", item.Key, item.Value.ActionType);
                }
            }
            // Log as Warning, so that it will show by default.
            _logger.LogWarning(sb.ToString());
        }



        /// <summary>
        /// Allow the user to choose which Adhoc action is to be enabled to be run.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="lastActionTaken"></param>
        private static void ChooseAdhocAction(Dictionary<string, BotEngineClient.Action> Actions, Dictionary<string, ActionActivity> lastActionTaken)
        {
            StringBuilder sb = new StringBuilder();
            List<string> adhocActions = new List<string>();
            int i = 0;
            string inputText;
            sb.AppendLine();
            sb.AppendLine("Choose the action to activate from the list below");
            foreach (KeyValuePair<string, BotEngineClient.Action> item in Actions)
            {
                if (Enum.TryParse(item.Value.ActionType, true, out ValidActionType validActionType))
                {
                    switch (validActionType)
                    {
                        case ValidActionType.Adhoc:
                            adhocActions.Add(item.Key);
                            sb.AppendFormat("[{0}] - {1}", i++, item.Key);
                            sb.AppendLine();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    _logger.LogError("Action {0} has invalid ActionType {1}", item.Key, item.Value.ActionType);
                }
            }
            sb.AppendLine("Enter the number of the Action to activate, and press enter");
            sb.AppendLine("Any other text will return to normal operation");
            // Log as Warning, so that it will show by default.
            _logger.LogWarning(sb.ToString());
            inputText = Console.ReadLine();
            int selectedOption;
            if (int.TryParse(inputText, out selectedOption))
            {
                if (selectedOption >= 0 && selectedOption < i)
                {
                    if (lastActionTaken.ContainsKey(adhocActions[selectedOption]))
                    {
                        // Allow changing of Overrides if there
                        if (lastActionTaken[adhocActions[selectedOption]].CommandValueOverride != null)
                        {
                            Dictionary<int, string> cvoIdentifiers = new Dictionary<int, string>();
                            int selectedOverride = 0;
                            i = DefineAdhocOverrideMessage(lastActionTaken, sb, adhocActions, selectedOption, cvoIdentifiers);
                            bool exitWhile = false;
                            selectedOverride = -1;
                            do
                            {
                                // Log as Warning, so that it will show by default.
                                _logger.LogWarning(sb.ToString());
                                inputText = Console.ReadLine();
                                if (int.TryParse(inputText, out selectedOverride))
                                {
                                    if (selectedOverride == 0)
                                    {
                                        exitWhile = true;
                                    }
                                    else if (cvoIdentifiers.ContainsKey(selectedOverride))
                                    {
                                        _logger.LogWarning("Enter new value for {0}: ", cvoIdentifiers[selectedOverride]);
                                        inputText = Console.ReadLine();
                                        lastActionTaken[adhocActions[selectedOption]].CommandValueOverride[cvoIdentifiers[selectedOverride]] = inputText;
                                        i = DefineAdhocOverrideMessage(lastActionTaken, sb, adhocActions, selectedOption, cvoIdentifiers);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Value {0} is not in list, try again:", selectedOverride);
                                    }
                                }
                                else
                                {
                                    selectedOverride = -1;
                                    exitWhile = true;
                                }
                            }
                            while (!exitWhile);
                            if (selectedOverride == 0)
                            {
                                // Don't execute this, just get out of here.
                                return;
                            }
                        }
                        //Reset the CommandLoopStatus values to 0, just in case it was cancelled before.
                        if (lastActionTaken[adhocActions[selectedOption]].CommandLoopStatus != null)
                        {
                            lastActionTaken[adhocActions[selectedOption]].CommandLoopStatus.Keys.ToList().ForEach(k => lastActionTaken[adhocActions[selectedOption]].CommandLoopStatus[k] = "0");
                        }
                        lastActionTaken[adhocActions[selectedOption]].ActionEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Generates the message to show when updating Overrides.
        /// </summary>
        /// <param name="lastActionTaken"></param>
        /// <param name="sb"></param>
        /// <param name="adhocActions"></param>
        /// <param name="selectedOption"></param>
        /// <param name="cvoIdentifiers"></param>
        /// <returns></returns>
        private static int DefineAdhocOverrideMessage(Dictionary<string, ActionActivity> lastActionTaken, StringBuilder sb, List<string> adhocActions, int selectedOption, Dictionary<int, string> cvoIdentifiers)
        {
            int i;
            sb.Clear();
            cvoIdentifiers.Clear();
            sb.AppendLine("Choose the Override to edit from the list below:");
            sb.AppendLine("[0] - Abort Adhoc Execution");
            i = 1;
            foreach (KeyValuePair<string, string?> cvoItem in lastActionTaken[adhocActions[selectedOption]].CommandValueOverride)
            {
                cvoIdentifiers.Add(i, cvoItem.Key);
                sb.AppendFormat("[{0}] - {1} = {2}", i++, cvoItem.Key, cvoItem.Value);
                sb.AppendLine();  // add the newline
            }
            sb.AppendLine("Enter the number of the Override to edit, and press enter");
            sb.AppendLine("Any other text will start the Adhoc Action");
            return i;
        }

        /// <summary>
        /// Allow the user to choose which action is to be Disabled from running.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="lastActionTaken"></param>
        private static void ChooseDisableAction(Dictionary<string, BotEngineClient.Action> Actions, Dictionary<string, ActionActivity> lastActionTaken)
        {
            StringBuilder sb = new StringBuilder();
            List<string> activeActions = new List<string>();
            int i = 0;
            string inputText;
            sb.AppendLine();
            sb.AppendLine("Choose the action to Disable from the list below");
            foreach (KeyValuePair<string, BotEngineClient.Action> item in Actions)
            {
                if (lastActionTaken.ContainsKey(item.Key))
                {
                    if (lastActionTaken[item.Key].ActionEnabled == true)
                    {
                        activeActions.Add(item.Key);
                        sb.AppendFormat("[{0}] - {1}", i++, item.Key);
                        sb.AppendLine();
                    }
                }
            }
            sb.AppendLine("Enter the number of the Action to Disable, and press enter");
            sb.AppendLine("Any other text will return to normal operation");
            // Log as Warning, so that it will show by default.
            _logger.LogWarning(sb.ToString());
            inputText = Console.ReadLine();
            int selectedOption;
            if (int.TryParse(inputText, out selectedOption))
            {
                if (selectedOption >= 0 && selectedOption < i)
                {
                    if (lastActionTaken.ContainsKey(activeActions[selectedOption]))
                    {
                        lastActionTaken[activeActions[selectedOption]].ActionEnabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Allow the user to choose which Disabled action is to be enabled to be run.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="lastActionTaken"></param>
        private static void ChooseEnableAction(Dictionary<string, BotEngineClient.Action> Actions, Dictionary<string, ActionActivity> lastActionTaken)
        {
            StringBuilder sb = new StringBuilder();
            List<string> activeActions = new List<string>();
            int i = 0;
            string inputText;
            sb.AppendLine();
            sb.AppendLine("Choose the action to Enable from the list below");
            foreach (KeyValuePair<string, BotEngineClient.Action> item in Actions)
            {
                if (lastActionTaken.ContainsKey(item.Key))
                {
                    if (lastActionTaken[item.Key].ActionEnabled == false)
                    {
                        activeActions.Add(item.Key);
                        sb.AppendFormat("[{0}] - {1}", i++, item.Key);
                        sb.AppendLine();
                    }
                }
            }
            sb.AppendLine("Enter the number of the Action to Enable, and press enter");
            sb.AppendLine("Any other text will return to normal operation");
            // Log as Warning, so that it will show by default.
            _logger.LogWarning(sb.ToString());
            inputText = Console.ReadLine(); // ToDo: Add ability to override values.
            int selectedOption;
            if (int.TryParse(inputText, out selectedOption))
            {
                if (selectedOption >= 0 && selectedOption < i)
                {
                    if (lastActionTaken.ContainsKey(activeActions[selectedOption]))
                    {
                        lastActionTaken[activeActions[selectedOption]].ActionEnabled = true;
                    }
                }
            }
        }
        #endregion

        #region Config Processing
        /// <summary>
        /// Recurse through the command to get all the overrides available.
        /// </summary>
        /// <param name="overrides"></param>
        /// <param name="command"></param>
        private static void GatherOverrides(Dictionary<string, string?> overrides, Command command)
        {
            if (command.OverrideId != null)
            {
                overrides.Add(command.OverrideId, null);
            }
            if (command.Commands != null)
                foreach (Command commandItem in command.Commands)
                {
                    GatherOverrides(overrides, commandItem);
                }
        }
        
        /// <summary>
        /// Updates a device config file to match the latest available from the game config file.
        /// </summary>
        /// <param name="botDeviceConfig"></param>
        /// <param name="botGameConfig"></param>
        private static void ValidateAndUpdateDeviceConfig(BOTDeviceConfig botDeviceConfig, BOTConfig botGameConfig)
        {
            foreach (KeyValuePair<string, BotEngineClient.Action> item in botGameConfig.Actions)
            {
                if (Enum.TryParse(item.Value.ActionType, true, out ValidActionType validActionType))
                {
                    // For all but System, setup/update the ActionActivities
                    if (validActionType != ValidActionType.System)
                    {
                        ActionActivity actionActivty = new ActionActivity {
                            ActionEnabled = false,  // Default all to disabled on 1st execution
                            LastRun = DateTime.MinValue,
                            CommandValueOverride = new Dictionary<string, string?>()
                        };
                        if (validActionType == ValidActionType.Always)
                        {
                            actionActivty.ActionEnabled = true;  //  Except for Always, which by default, always run.
                        }
                        if (!botDeviceConfig.LastActionTaken.ContainsKey(item.Key))
                        {
                            if (item.Value.Commands != null)
                                foreach (Command commandItem in item.Value.Commands)
                                {
                                    GatherOverrides(actionActivty.CommandValueOverride, commandItem);
                                }
                            if (actionActivty.CommandValueOverride.Count == 0)
                            {
                                actionActivty.CommandValueOverride = null;
                            }
                            botDeviceConfig.LastActionTaken.Add(item.Key, actionActivty);
                        }
                        else
                        {
                            actionActivty = botDeviceConfig.LastActionTaken[item.Key];
                            Dictionary<string, string?> commandValueOverride = new Dictionary<string, string?>();
                            // Update CommandValueOverride
                            if (item.Value.Commands != null)
                                foreach (Command commandItem in item.Value.Commands)
                                {
                                    GatherOverrides(commandValueOverride, commandItem);
                                }
                            if (commandValueOverride.Count == 0)
                            {
                                actionActivty.CommandValueOverride = null;
                            }
                            else
                            {
                                if (actionActivty.CommandValueOverride == null)
                                {
                                    actionActivty.CommandValueOverride = commandValueOverride;
                                }
                                else
                                {
                                    Dictionary<string, string?> replacementCommandValueOverride = new Dictionary<string, string?>();
                                    foreach (KeyValuePair<string, string?> cvoItem in commandValueOverride)
                                    {
                                        if (actionActivty.CommandValueOverride.ContainsKey(cvoItem.Key))
                                        {
                                            replacementCommandValueOverride.Add(cvoItem.Key, actionActivty.CommandValueOverride[cvoItem.Key]);
                                        }
                                        else
                                        {
                                            replacementCommandValueOverride.Add(cvoItem.Key, null);
                                        }
                                    }
                                    actionActivty.CommandValueOverride = replacementCommandValueOverride;
                                }
                                botDeviceConfig.LastActionTaken[item.Key] = actionActivty;
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogError("For {0} the ActionType of {1} is not valid", item.Key, item.Value.ActionType);
                }
            }
        }

        /// <summary>
        /// Takes the in flight device config, and the on disk device config, and merges them together
        /// Result is the latest of each...
        /// </summary>
        /// <param name="botDeviceConfig"></param>
        /// <param name="botDeviceConfigNew"></param>
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
        #endregion

        #region File handling

        #region Device Files

        /// <summary>
        /// Loads an existing Device Config file if it exists, or creates a new one in memory.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static int LoadOrBuildDeviceConfig(Options o)
        {
            string jsonString = string.Empty;
            if (!File.Exists(o.DeviceFileName))
            {
                botDeviceConfig.FileId = "DeviceConfig";
                botDeviceConfig.LastActionTaken = new Dictionary<string, ActionActivity>();
                foreach (KeyValuePair<string, BotEngineClient.Action> item in botGameConfig.Actions)
                {
                    if (Enum.TryParse(item.Value.ActionType, true, out ValidActionType validActionType))
                    {
                        switch (validActionType)
                        {
                            case ValidActionType.Adhoc:
                            case ValidActionType.Daily:
                            case ValidActionType.Scheduled:
                                ActionActivity adhocActionActivity = new ActionActivity {
                                    LastRun = DateTime.MinValue,
                                    ActionEnabled = false,
                                    CommandValueOverride = new Dictionary<string, string?>()
                                };
                                if (item.Value.Commands != null)
                                    foreach (Command commandItem in item.Value.Commands)
                                    {
                                        GatherOverrides(adhocActionActivity.CommandValueOverride, commandItem);
                                    }
                                if (adhocActionActivity.CommandValueOverride.Count == 0)
                                {
                                    adhocActionActivity.CommandValueOverride = null;
                                }
                                botDeviceConfig.LastActionTaken.Add(item.Key, adhocActionActivity);
                                break;
                            case ValidActionType.Always:
                                ActionActivity dailyActionActivity = new ActionActivity {
                                    LastRun = DateTime.MinValue,
                                    ActionEnabled = true,
                                    CommandValueOverride = new Dictionary<string, string?>()
                                };
                                if (item.Value.Commands != null)
                                    foreach (Command commandItem in item.Value.Commands)
                                    {
                                        GatherOverrides(dailyActionActivity.CommandValueOverride, commandItem);
                                    }
                                if (dailyActionActivity.CommandValueOverride.Count == 0)
                                {
                                    dailyActionActivity.CommandValueOverride = null;
                                }
                                botDeviceConfig.LastActionTaken.Add(item.Key, dailyActionActivity);
                                break;
                            case ValidActionType.System:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        _logger.LogError("Action {0} has an incorrect ActionType of {1}", item.Key, item.Value.ActionType);
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
            return 0;
        }



        /// <summary>
        /// Saves the device config file, so that the latest times of execution etc are saved for later
        /// </summary>
        private static void SaveDeviceConfigJson()
        {
            if (botDeviceConfig != null && botDeviceConfig.FileId != null && botDeviceConfig.LastActionTaken != null)
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
            else
            {
                _logger.LogWarning("Attempt to save device config file {0}, when it hasn't been loaded.", options.DeviceFileName);
            }
        }
        #endregion

        /// <summary>
        /// Loads the List Config into memory
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static int LoadListConfig(Options o)
        {
            string jsonString = string.Empty;
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
            return 0;
        }

        /// <summary>
        /// Loads the Game Config into memory
        /// </summary>
        /// <param name="o"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        private static int LoadGameConfig(Options o)
        {
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
            return 0;
        }

        #endregion

        #region Keyboard Handling
        /// <summary>
        /// General keyboard handler 
        /// </summary>
        /// <param name="Actions"></param>
        /// <param name="lastActionTaken"></param>
        /// <param name="paused"></param>
        /// <returns></returns>
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
                    exitRequested = true;
                }
            }

            return consoleStatus;
        }

        /// <summary>
        /// Action the key pressed on the keyboard
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="lastActionTaken"></param>
        /// <returns></returns>
        private static ConsoleKeyPressEnum HandleConsoleKeyBoard(Dictionary<string, BotEngineClient.Action> actions, Dictionary<string, ActionActivity> lastActionTaken)
        {
            ConsoleKeyPressEnum result = ConsoleKeyPressEnum.Nothing;
            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case 'a':
                    case 'A':
                        ChooseAdhocAction(actions, lastActionTaken);
                        result = ConsoleKeyPressEnum.Nothing;
                        break;
                    case 'd':
                    case 'D':
                        ChooseDisableAction(actions, lastActionTaken);
                        result = ConsoleKeyPressEnum.Nothing;
                        break;
                    case 'e':
                    case 'E':
                        ChooseEnableAction(actions, lastActionTaken);
                        result = ConsoleKeyPressEnum.Nothing;
                        break;
                    case 'h':
                    case 'H':
                        _logger.LogWarning("In Console Help:\r\na/A : Adhoc\r\nd/D : Disable\r\ne/E : Enable\r\nh/H : Help\r\np/P : Pause\r\ns/S : Execution Status\r\nx/X : Exit");
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
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for Ctrl-C or Break.
        /// Sets that a cancel has been requested, and prevents the program from auto exiting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected static void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            _logger.LogCritical("Control C Captured");
            if (threadCTS != null)
            {
                threadCTS.Cancel();
            }
            args.Cancel = true;
            cancelRequested = true;
        }

        
        /// <summary>
        /// Call back from a bot thread with the results of the bot action.
        /// </summary>
        /// <param name="result"></param>
        public static void ResultCallback(BotEngineClient.BotEngine.CommandResults result)
        {
            threadResult = result;
        }

        #region Reloading Config Files via Events
        /// <summary>
        /// Event Handler which is fired when changes happen to Config files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Reloads the Device config file, when changes are detected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Reloads the Game config file, when changes are detected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Reloads the List config file, when changes are detected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Loads the content of the text file that is referenced in the event arguments into 
        /// a json string, which is returned
        /// </summary>
        /// <param name="e"></param>
        /// <param name="jsonString">will contain the contents of the file that was changed</param>
        /// <returns></returns>
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

        #endregion

        #endregion

        #region Logging
        /// <summary>
        /// Ensures that the logging is done as per the command line requests
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        private static int ReconfigureLogging(Options opt)
        {
            ServiceProvider = ConfigureServices(opt);
            _logger = (ILogger)ServiceProvider.GetService(typeof(ILogger));
            return 0;
        }

        /// <summary>
        /// Configures the logging settings, so that debug etc. can be avaialble.
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static ServiceProvider ConfigureServices(Options opt)
        {
            //services.Clear();
            LogLevel logLevel = LogLevel.Warning;
            if (!opt.ListDevices)
            {
                _ = Enum.TryParse(opt.LogLevel, true, out logLevel);
            }
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
            services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("AndroidBot"));
            return services.BuildServiceProvider();
        }
        #endregion
    }
}
