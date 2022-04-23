using Microsoft.Extensions.Logging;
using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using FindTextClient;

namespace BotEngineClient
{
    public class BotEngine
    {
        private readonly ILogger _logger;

        private AdbServer adbServer;
        private Image? savedImage;
        private AdbClient adbClient;
        private DeviceData adbDevice;
        private ConsoleOutputReceiver adbReceiver;
        private FindText findText;
        private Rectangle adbScreenSize;
        private Framebuffer adbFrameBuffer;
        private Dictionary<string, FindString> FindStrings;
        private Dictionary<string, Action> NormalActions;
        private Dictionary<string, Action> SystemActions;
        private BOTListConfig ListConfig;
        private string EmulatorName;

        public enum CommandResults
        {
            Ok,
            Missing,
            Exit,
            TimeOut,
            Restart,
            InputError,
            ADBError
        }

        /// <summary>
        /// Enum of all the Valid Command Ids that are supported.
        /// </summary>
        public enum ValidCommandIds
        {
            Click,
            ClickWhenNotFoundInArea,
            Drag,
            Exit,
            EnterLoopCoordinate,
            FindClick,
            FindClickAndWait,
            IfExists,
            IfNotExists,
            LoopCoordinates,
            LoopUntilFound,
            LoopUntilNotFound,
            Restart,
            RunAction,
            Sleep,
            StartGame,
            StopGame,
            WaitFor,
            WaitForThenClick,
            WaitForChange,
            WaitForNoChange
        }


        public void ReloadFindStrings(Dictionary<string, FindString> findStrings)
        {
            FindStrings = findStrings;
        }

        public void ReloadNormalActions(Dictionary<string, Action> normalActions)
        {
            NormalActions = normalActions;
        }

        public void ReloadSystemActions(Dictionary<string, Action> systemActions)
        {
            SystemActions = systemActions;
        }

        public void ReloadListConfig(BOTListConfig listConfig)
        {
            ListConfig = listConfig;
        }

        public BotEngine(IServiceProvider ServiceProvider, string ADBPath, string ADBDeviceData, Dictionary<string, FindString> findStrings, Dictionary<string, Action> systemActions, Dictionary<string, Action> normalActions, BOTListConfig listConfig)
        {
            _logger = (ILogger)ServiceProvider.GetService(typeof(ILogger));
            using (_logger.BeginScope("BotEngine"))
            {
                adbServer = new AdbServer();
                StartServerResult result = adbServer.StartServer(ADBPath, restartServerIfNewer: true);
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
                adbClient = new AdbClient();
                adbDevice = DeviceData.CreateFromAdbData(ADBDeviceData);
                adbReceiver = new ConsoleOutputReceiver(null);
                findText = new FindText();
                System.Threading.CancellationToken cancellationToken = default;
                adbFrameBuffer = new Framebuffer(adbDevice, adbClient);
                adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                using (Image localImage = adbFrameBuffer.ToImage())
                {
                    adbScreenSize = new Rectangle(0, 0, localImage.Width, localImage.Height);
                }
                FindStrings = findStrings;
                SystemActions = systemActions;
                NormalActions = normalActions;
                ListConfig = listConfig;
                EmulatorName = adbDevice.Model;
            }
        }

        public CommandResults ExecuteAction(string actionName)
        {
            using (_logger.BeginScope(String.Format("{0}:{1}({2})", EmulatorName,Helpers.CurrentMethodName(), actionName)))
            {
                _logger.LogInformation("Starting Action");
                Action action;
                if (SystemActions.ContainsKey(actionName))
                {
                    action = SystemActions[actionName];
                }
                else if (NormalActions.ContainsKey(actionName))
                {
                    action = NormalActions[actionName];
                }
                else
                {
                    _logger.LogError("Action {0} was not found in System or Normal Actions.", actionName);
                    return CommandResults.InputError;
                }
                CommandResults result = CommandResults.Ok;
                foreach (Command item in action.Commands)
                {
                    result = ExecuteCommand(item, null);
                    if (result == CommandResults.Exit)
                    {
                        _logger.LogDebug("Resetting Exit result to Ok for Action {0}", actionName);
                        result = CommandResults.Ok;
                        break;
                    }
                    else if (result == CommandResults.Restart)
                    {
                        _logger.LogDebug("Action {0} resulted in {1}", actionName, result.ToString());
                        break;
                    }
                    else if (result != CommandResults.Ok)
                    {
                        _logger.LogWarning("Action {0} Unsuccessful with {1}", actionName, result.ToString());
                        break;
                    }
                }
                return result;
            }
        }

        private void ADBSwipe(int x1, int y1, int x2, int y2, int delay)
        {
            AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("input swipe {0} {1} {2} {3} {4}", x1, y1, x2, y2, delay), adbDevice, adbReceiver);
        }

        private void ADBClick(int x, int y)
        {
            AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("input tap {0} {1}", x, y), adbDevice, adbReceiver);
        }

        private void ADBSendKeys(string text)
        {
            AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("input keyboard text '{0}'", text), adbDevice, adbReceiver);
        }

        private CommandResults ADBStartGame(string gameString, int timeOut)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults results = CommandResults.ADBError;
                AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("am start -n {0}", gameString), adbDevice, adbReceiver);
                _logger.LogInformation(adbReceiver.ToString());
                // No Clear capability, destroy and recreate.
                adbReceiver = new ConsoleOutputReceiver(null);
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                do
                {
                    Thread.Sleep(1500);
                    AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("dumpsys window windows", gameString), adbDevice, adbReceiver);
                    string cmdResult = adbReceiver.ToString();
                    string currentFocus = cmdResult.Substring(cmdResult.IndexOf("mCurrentFocus"));
                    currentFocus = currentFocus.Substring(0, currentFocus.IndexOf("\n"));
                    string focusedApp = cmdResult.Substring(cmdResult.IndexOf("mFocusedApp"));
                    focusedApp = focusedApp.Substring(0, focusedApp.IndexOf("\n"));
                    if (currentFocus.ToLower().Contains(gameString.ToLower()))
                    {
                        _logger.LogDebug(string.Format("App {0} has been detected with focus", gameString));
                        results = CommandResults.Ok;
                        break;
                    }
                    else
                    {
                        _logger.LogError(string.Format("Wrong App detected {0}, keep waiting until timeout", currentFocus));
                    }
                    // No Clear capability, destroy and recreate.
                    adbReceiver = new ConsoleOutputReceiver(null);
                } while (stopWatch.Elapsed.TotalMilliseconds < timeOut);
                stopWatch.Stop();
                return results;
            }
        }

        private CommandResults ADBStopGame(string gameString, int timeOut)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults results = CommandResults.ADBError;
                AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("am force-stop {0}", gameString), adbDevice, adbReceiver);
                _logger.LogInformation(adbReceiver.ToString());
                // No Clear capability, destroy and recreate.
                adbReceiver = new ConsoleOutputReceiver(null);
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                do
                {
                    Thread.Sleep(500);
                    AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("dumpsys window windows", gameString), adbDevice, adbReceiver);
                    string cmdResult = adbReceiver.ToString();
                    string currentFocus = cmdResult.Substring(cmdResult.IndexOf("mCurrentFocus"));
                    currentFocus = currentFocus.Substring(0, currentFocus.IndexOf("\n"));
                    string focusedApp = cmdResult.Substring(cmdResult.IndexOf("mFocusedApp"));
                    focusedApp = focusedApp.Substring(0, focusedApp.IndexOf("\n"));
                    if (currentFocus.ToLower().Contains(gameString.ToLower()))
                    {
                        _logger.LogDebug(string.Format("App {0} has been detected with focus, keep waiting", gameString));
                    }
                    else
                    {
                        _logger.LogError(string.Format("App {0} detected, game no longer active", currentFocus));
                        results = CommandResults.Ok;
                        break;
                    }
                    // No Clear capability, destroy and recreate.
                    adbReceiver = new ConsoleOutputReceiver(null);
                } while (stopWatch.Elapsed.TotalMilliseconds < timeOut);
                stopWatch.Stop();
                return results;
            }
        }

        private CommandResults WaitFor(string searchName, FindString searchString, int timeOut)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.TimeOut;
                System.Threading.CancellationToken cancellationToken = default;
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                while (stopWatch.Elapsed.TotalMilliseconds < timeOut)
                {
                    adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                    using (Image localImage = adbFrameBuffer.ToImage())
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                    }

                    List<SearchResult>? dataresult = findText.SearchText(searchString.searchArea.X, searchString.searchArea.Y, searchString.searchArea.X+searchString.searchArea.width, searchString.searchArea.Y+searchString.searchArea.height, searchString.backgroundTolerance, searchString.textTolerance, searchString.findString, false, false, false, searchString.offsetX, searchString.offsetY);
                    if (dataresult != null)
                    {
                        result = CommandResults.Ok;
                        _logger.LogDebug("Search Successful, found {0} whilst looking for {1}", dataresult[0].Id, searchName);
                        break;
                    }
                    Thread.Sleep(50);
                }
                stopWatch.Stop();
                if (result != CommandResults.Ok)
                {
                    _logger.LogWarning("Search Unsuccessful with {0} whilst looking for {1}", result.ToString(), searchName);
                }
                return result;
            }
        }

        private CommandResults WaitForThenClick(string searchName, FindString searchString, int timeOut)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.TimeOut;
                System.Threading.CancellationToken cancellationToken = default;
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                while (stopWatch.Elapsed.TotalMilliseconds < timeOut)
                {
                    adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                    using (Image localImage = adbFrameBuffer.ToImage())
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                    }

                    List<SearchResult>? dataresult = findText.SearchText(searchString.searchArea.X, searchString.searchArea.Y, searchString.searchArea.X + searchString.searchArea.width, searchString.searchArea.Y + searchString.searchArea.height, searchString.backgroundTolerance, searchString.textTolerance, searchString.findString, false, false, false, searchString.offsetX, searchString.offsetY);
                    if (dataresult != null)
                    {
                        _logger.LogDebug("Initial Search Successful, found {0} whilst looking for {1}, Clicking", dataresult[0].Id, searchName);
                        result = CommandResults.Ok;
                        ADBClick(dataresult[0].X, dataresult[0].Y);
                        Thread.Sleep(50);
                        break;
                    }
                    Thread.Sleep(50);
                }
                stopWatch.Stop();
                if (result != CommandResults.Ok)
                {
                    _logger.LogWarning("Search Unsuccessful with {0} whilst looking for {1}", result.ToString(), searchName);
                }
                return result;
            }
        }

        private CommandResults IfExists(string searchName, FindString findString, List<Command> Commands, Object? additionalData)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.Missing;
                System.Threading.CancellationToken cancellationToken = default;
                adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                using (Image localImage = adbFrameBuffer.ToImage())
                {
                    findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                }

                List<SearchResult>? dataresult = findText.SearchText(findString.searchArea.X, findString.searchArea.Y, findString.searchArea.X+findString.searchArea.width, findString.searchArea.Y+findString.searchArea.height, findString.backgroundTolerance, findString.textTolerance, findString.findString, false, false, false, findString.offsetX, findString.offsetY);
                if (dataresult != null)
                {
                    _logger.LogDebug("Search Successful, found {0} whilst looking for {1}", dataresult[0].Id, searchName);
                    result = CommandResults.Ok;
                    foreach (Command command in Commands)
                    {
                        result = ExecuteCommand(command, additionalData);
                        if (result != CommandResults.Ok)
                            return result;
                    }

                }
                else
                {
                    _logger.LogDebug("Search Unsuccessful with {0} whilst looking for {1}", result.ToString(), searchName);
                    result = CommandResults.Ok;
                }
                return result;
            }
        }

        private CommandResults IfNotExists(string searchName, FindString findString, List<Command> Commands, Object? additionalData)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.Missing;
                System.Threading.CancellationToken cancellationToken = default;
                adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                using (Image localImage = adbFrameBuffer.ToImage())
                {
                    findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                }

                List<SearchResult>? dataresult = findText.SearchText(findString.searchArea.X, findString.searchArea.Y, findString.searchArea.X + findString.searchArea.width, findString.searchArea.Y + findString.searchArea.height, findString.backgroundTolerance, findString.textTolerance, findString.findString, false, false, false, findString.offsetX, findString.offsetY);
                if (dataresult != null)
                {
                    _logger.LogDebug("Search Successful, found {0} whilst looking for {1}", dataresult[0].Id, searchName);
                    result = CommandResults.Ok;
                }
                else
                {
                    _logger.LogDebug("Search Unsuccessful with {0} whilst looking for {1}", result.ToString(), searchName);
                    result = CommandResults.Ok;
                    foreach (Command command in Commands)
                    {
                        result = ExecuteCommand(command, additionalData);
                        if (result != CommandResults.Ok)
                            return result;
                    }
                }
                return result;
            }
        }

        private CommandResults FindClick(string searchName, FindString searchString, bool IgnoreMissing)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.Missing;
                System.Threading.CancellationToken cancellationToken = default;
                adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                using (Image localImage = adbFrameBuffer.ToImage())
                {
                    findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                }

                List<SearchResult>? dataresult = findText.SearchText(searchString.searchArea.X, searchString.searchArea.Y, searchString.searchArea.X + searchString.searchArea.width, searchString.searchArea.Y + searchString.searchArea.height, searchString.backgroundTolerance, searchString.textTolerance, searchString.findString, false, false, false, searchString.offsetX, searchString.offsetY);
                if (dataresult != null)
                {
                    _logger.LogDebug("Initial Search Successful, found {0} whilst looking for {1}, Clicking", dataresult[0].Id, searchName);
                    result = CommandResults.Ok;
                    ADBClick(dataresult[0].X, dataresult[0].Y);
                    Thread.Sleep(50);
                }
                else
                {
                    if (IgnoreMissing)
                    {
                        _logger.LogDebug("Unsuccessful with {0} whilst looking for {1}, Ignored", result.ToString(), searchName);
                        result = CommandResults.Ok;
                    }
                    else
                        _logger.LogWarning("Unsuccessful with {0} whilst looking for {1}", result.ToString(), searchName);
                }
                return result;
            }
        }


        private CommandResults FindClickAndWait(string searchName, FindString searchString, int timeOut)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                bool found = false;
                CommandResults result = CommandResults.Missing;
                System.Threading.CancellationToken cancellationToken = default;
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                while (stopWatch.Elapsed.TotalMilliseconds < timeOut)
                {
                    adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                    using (Image localImage = adbFrameBuffer.ToImage())
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                    }

                    List<SearchResult>? dataresult = findText.SearchText(searchString.searchArea.X, searchString.searchArea.Y, searchString.searchArea.X+searchString.searchArea.width, searchString.searchArea.Y+searchString.searchArea.height, searchString.backgroundTolerance, searchString.textTolerance, searchString.findString, false, false, false, searchString.offsetX, searchString.offsetY);
                    if (dataresult != null)
                    {
                        if (!found)
                        {
                            _logger.LogDebug("Initial Search Successful, found {0} whilst looking for {1}, Clicking", dataresult[0].Id, searchName);
                            result = CommandResults.TimeOut;
                            ADBClick(dataresult[0].X, dataresult[0].Y);
                            found = true;
                        }
                    }
                    else
                    {
                        if (found)
                        {
                            _logger.LogDebug("Secondary Search Unsuccessful (good) whilst looking for {0}", searchName);
                            result = CommandResults.Ok;
                            break;
                        }
                        else
                        {
                            _logger.LogDebug("Initial Search Unsuccessful, whilst looking for {0} at {1}", searchName, stopWatch.Elapsed.TotalMilliseconds);
                        }
                    }
                    Thread.Sleep(50);
                }
                stopWatch.Stop();
                if (result != CommandResults.Ok)
                {
                    _logger.LogWarning("Unsuccessful with {0} whilst looking for {1}", result.ToString(), searchName);
                }
                return result;
            }
        }

        /// <summary>
        /// Within a list of search areas, the 1st one that doesn't have the searchString image is clicked on.
        /// </summary>
        /// <param name="searchName"></param>
        /// <param name="searchString"></param>
        /// <param name="areas"></param>
        /// <returns></returns>
        // ToDo: Add optional scroll if nothing found?
        // ToSo: Add new command, which takes a big area, and a number, which is the number of regions to search within, and the 1st click point if the image isn't found?
        private CommandResults ClickWhenNotFoundInArea(string searchName, FindString searchString, List<SearchArea> areas)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.Ok;  // Return OK even when not found, as not found also means nothing to do.
                if (areas.Count > 0)
                {
                    System.Threading.CancellationToken cancellationToken = default;
                    adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                    using (Image localImage = adbFrameBuffer.ToImage())
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                        localImage.Save(@"D:\Temp\SaveImage.png");
                    }

                    int x1 = int.MaxValue, y1 = int.MaxValue, x2 = int.MinValue, y2 = int.MinValue;
                    foreach (SearchArea area in areas)
                    {
                        x1 = Math.Min(x1, area.X);
                        y1 = Math.Min(y1, area.Y);
                        x2 = Math.Max(x2, area.X + area.width);
                        y2 = Math.Max(y2, area.Y + area.height);
                    }

                    List<SearchResult>? dataresults = findText.SearchText(x1, y1, x2, y2, searchString.backgroundTolerance, searchString.textTolerance, searchString.findString, false, true, false, searchString.offsetX, searchString.offsetY);
                    if (dataresults == null)
                    {
                        _logger.LogDebug("Search Unsuccessful, found nothing at {0} whilst looking for {1}, Clicking", areas[0].ToString(), searchName);
                        int x = areas[0].X + (areas[0].width / 2);
                        int y = areas[0].Y + (areas[0].height / 2);
                        ADBClick(x, y);
                        result = CommandResults.Ok;
                    }
                    else
                    {
                        bool found = false;
                        foreach (SearchArea area in areas)
                        {
                            found = false;
                            foreach (SearchResult dataresult in dataresults)
                            {
                                if ((dataresult.X > area.X) && (dataresult.X < area.X + area.width)
                                    && (dataresult.Y > area.Y) && (dataresult.Y < area.Y + area.height))
                                {
                                    found = true;
                                    _logger.LogDebug("Partial Search Successful, whilst looking for {0} at {1}", searchName, area.ToString());
                                    break;
                                }
                            }
                            if (!found)
                            {
                                _logger.LogDebug("Partial Search Unsuccessful, Clicking, whilst looking for {0} at {1}", searchName, area.ToString());
                                int x = area.X + (area.width / 2);
                                int y = area.Y + (area.height / 2);
                                ADBClick(x, y);
                                result = CommandResults.Ok;
                                break;
                            }
                        }
                    }
                }
                return result;
            }
        }


        private CommandResults LoopCoordinates(string CoordinateName, List<Command> Commands)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), CoordinateName)))
            {
                CommandResults result;
                if (ListConfig.Coordinates == null)
                    return CommandResults.InputError;
                if (!ListConfig.Coordinates.ContainsKey(CoordinateName))
                    return CommandResults.InputError;
                List<XYCoords> coords = ListConfig.Coordinates[CoordinateName];

                foreach (XYCoords point in coords)
                {
                    foreach (Command command in Commands)
                    {
                        result = ExecuteCommand(command, point);
                        if (result != CommandResults.Ok)
                            return result;
                    }
                }
                return CommandResults.Ok;
            }
        }

        private CommandResults LoopUntilNotFound(List<string> imageNames, List<Command> Commands, int timeOut, Object? additionalData)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.Ok;
                System.Threading.CancellationToken cancellationToken = default;
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                int searchX = w, searchY = h, searchW = 0, searchH = 0;
                float textFactor = 0.0f, backFactor = 0.0f;
                string searchString = string.Empty;
                if (timeOut == -1)
                    timeOut = int.MaxValue;

                // Get the largest bounding box for searching, and the most permissive Factors.
                foreach (string item in imageNames)
                {
                    if (!FindStrings.ContainsKey(item))
                    {
                        _logger.LogError("FindString {0} is missing from json file", item);
                        return CommandResults.InputError;
                    }
                    _logger.LogDebug("Searching for {0}", item);
                    FindString findString = FindStrings[item];
                    searchString += findString.findString;
                    searchX = Math.Min(searchX, findString.searchArea.X);
                    searchY = Math.Min(searchY, findString.searchArea.Y);
                    searchW = Math.Max(searchW, findString.searchArea.X+findString.searchArea.width);
                    searchH = Math.Max(searchH, findString.searchArea.Y+findString.searchArea.height);
                    textFactor = Math.Max(textFactor, findString.textTolerance);
                    backFactor = Math.Max(backFactor, findString.backgroundTolerance);
                }

                List<SearchResult>? dataresult;
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                do
                {
                    adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                    using (Image localImage = adbFrameBuffer.ToImage())
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                    }
                    // This is searching for all results, just to be sure.
                    dataresult = findText.SearchText(searchX, searchY, searchW, searchH, backFactor, textFactor, searchString, false, true, false);
                    if (dataresult != null)
                    {
                        _logger.LogDebug("Search Successful, found {0}", dataresult[0].Id);
                        foreach (Command command in Commands)
                        {
                            result = ExecuteCommand(command, additionalData);
                            if (result != CommandResults.Ok)
                                return result;
                        }
                    }
                    Thread.Sleep(50);
                } while ((dataresult != null) && (stopWatch.ElapsedMilliseconds < timeOut));
                if (dataresult == null)
                {
                    _logger.LogDebug("Item(s) have now disappeared");
                    result = CommandResults.Ok;
                }
                else
                {
                    _logger.LogWarning("Item(s) haven't disappeared before Timeout");
                    result = CommandResults.TimeOut;
                }
                return result;
            }
        }


        private CommandResults LoopUntilFound(List<string> imageNames, List<Command> Commands, int timeOut, Object? additionalData)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.Ok;
                System.Threading.CancellationToken cancellationToken = default;
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                int searchX = w, searchY = h, searchW = 0, searchH = 0;
                float textFactor = 0.0f, backFactor = 0.0f;
                string searchString = string.Empty;
                if (timeOut == -1)
                    timeOut = int.MaxValue;
                // Get the largest bounding box for searching, and the most permissive Factors.
                foreach (string item in imageNames)
                {
                    if (!FindStrings.ContainsKey(item))
                    {
                        _logger.LogError("FindString {0} is missing from json file", item);
                        return CommandResults.InputError;
                    }
                    FindString findString = FindStrings[item];
                    searchString += findString.findString;
                    searchX = Math.Min(searchX, findString.searchArea.X);
                    searchY = Math.Min(searchY, findString.searchArea.Y);
                    searchW = Math.Max(searchW, findString.searchArea.X + findString.searchArea.width);
                    searchH = Math.Max(searchH, findString.searchArea.Y + findString.searchArea.height);
                    textFactor = Math.Max(textFactor, findString.textTolerance);
                    backFactor = Math.Max(backFactor, findString.backgroundTolerance);
                }
                List<SearchResult>? dataresult;
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                do
                {
                    adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                    using (Image localImage = adbFrameBuffer.ToImage())
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                    }
                    // This is searching for all results, just to be sure.
                    dataresult = findText.SearchText(searchX, searchY, searchW, searchH, backFactor, textFactor, searchString, false, true, false);
                    if (dataresult == null)
                    {
                        _logger.LogDebug("Search Unsuccessful, Execute Commands");
                        foreach (Command command in Commands)
                        {
                            result = ExecuteCommand(command, additionalData);
                            if (result != CommandResults.Ok)
                                return result;
                        }
                    }
                    Thread.Sleep(50);
                } while ((dataresult == null) && (stopWatch.ElapsedMilliseconds < timeOut));
                if (dataresult != null)
                    _logger.LogDebug("Item(s) have now appeared");
                else
                {
                    _logger.LogWarning("Item(s) haven't appeared before Timeout");
                    result = CommandResults.TimeOut;
                }
                return result;
            }
        }

        private CommandResults WaitForChange(SearchArea changeDetectArea, float changeDetectDifference, int timeOut)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.TimeOut;
                System.Threading.CancellationToken cancellationToken = default;
                int iterationWait = Math.Max(timeOut / 20, 50);
                

                adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                using (Bitmap savedImage = (Bitmap)adbFrameBuffer.ToImage())
                {
                    Rectangle srcRect = new Rectangle(changeDetectArea.X, changeDetectArea.Y, changeDetectArea.width, changeDetectArea.height);
                    using (Bitmap savedPart = savedImage.Clone(srcRect, savedImage.PixelFormat))
                    {
                        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                        stopWatch.Start();
                        do
                        {
                            Thread.Sleep(iterationWait);
                            adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                            using (Bitmap newImage = (Bitmap)adbFrameBuffer.ToImage())
                            {
                                using (Bitmap newPart = newImage.Clone(srcRect, newImage.PixelFormat))
                                {
                                    float difference = ImageTool.GetPercentageDifference(savedPart, newPart, 3);
                                    _logger.LogDebug("Image Difference was {0}", difference);
                                    if (difference >= changeDetectDifference)
                                    {
                                        _logger.LogDebug("Success - Image has changed enough");
                                        result = CommandResults.Ok;
                                        break;
                                    }
                                }
                            }
                        }
                        while (stopWatch.Elapsed.TotalMilliseconds < timeOut);
                        stopWatch.Stop();
                    }
                }

                return result;
            }
        }

        private CommandResults WaitForNoChange(SearchArea changeDetectArea, float changeDetectDifference, int timeOut)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.Ok;
                System.Threading.CancellationToken cancellationToken = default;
                int iterationWait = Math.Max(timeOut / 20, 50);


                adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                using (Bitmap savedImage = (Bitmap)adbFrameBuffer.ToImage())
                {
                    Rectangle srcRect = new Rectangle(changeDetectArea.X, changeDetectArea.Y, changeDetectArea.width, changeDetectArea.height);
                    using (Bitmap savedPart = savedImage.Clone(srcRect, savedImage.PixelFormat))
                    {
                        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                        stopWatch.Start();
                        do
                        {
                            Thread.Sleep(iterationWait);
                            adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                            using (Bitmap newImage = (Bitmap)adbFrameBuffer.ToImage())
                            {
                                using (Bitmap newPart = newImage.Clone(srcRect, newImage.PixelFormat))
                                {
                                    float difference = ImageTool.GetPercentageDifference(savedPart, newPart, 3);
                                    _logger.LogDebug("Image Difference was {0}", difference);
                                    if (difference <= changeDetectDifference)
                                    {
                                        _logger.LogDebug("Failure - Image has changed to much");
                                        result = CommandResults.Missing;
                                        break;
                                    }
                                }
                            }
                        }
                        while (stopWatch.Elapsed.TotalMilliseconds < timeOut);
                        stopWatch.Stop();
                    }
                }
                return result;
            }
        }


        private CommandResults ExecuteCommand(Command command, Object? additionalData)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), command.CommandId)))
            {
                List<string> imageNames = new List<string>();
                _logger.LogDebug("Starting Command Execution");
                switch (command.CommandId.ToLower())  // ToDo: Refactor to use Enum ValidCommandIds
                {
                    case "click":
                        if (command.Location == null)
                        {
                            _logger.LogError("Command {0} Error Location is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        ADBClick(command.Location.X, command.Location.Y);
                        break;
                    case "clickwhennotfoundinarea":
                        if (command.Areas == null)
                        {
                            _logger.LogError("Command {0} Error Areas is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.ImageName == null)
                        {
                            _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (!FindStrings.ContainsKey(command.ImageName))
                        {
                            _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                            return CommandResults.InputError;
                        }
                        return ClickWhenNotFoundInArea(command.ImageName, FindStrings[command.ImageName], command.Areas);
                    case "drag":
                        if (command.Swipe == null)
                        {
                            _logger.LogError("Command {0} Error Swipe is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.Delay == null)
                        {
                            _logger.LogError("Command {0} Error Delay is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        ADBSwipe(command.Swipe.X1, command.Swipe.Y1, command.Swipe.X2, command.Swipe.Y2, (int)command.Delay);
                        break;
                    case "exit":
                        return CommandResults.Exit;
                    case "enterloopcoordinate":
                        if (command.Value == null)
                        {
                            _logger.LogError("Command {0} Error Value is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (additionalData == null)
                        {
                            _logger.LogError("Command {0} Error additionalData is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (!(additionalData is XYCoords))
                        {
                            _logger.LogError("Command {0} Error additionalData is missing XYCoords", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.Value.ToLower() == "x")
                        {
                            ADBSendKeys(((XYCoords)additionalData).X.ToString());
                        }
                        else
                        {
                            ADBSendKeys(((XYCoords)additionalData).Y.ToString());
                        }
                        break;
                    case "findclick":
                        if (command.ImageName == null)
                        {
                            _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (!FindStrings.ContainsKey(command.ImageName))
                        {
                            _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                            return CommandResults.InputError;
                        }
                        bool ignoreMissing = false;
                        if (command.IgnoreMissing != null)
                            ignoreMissing = (bool)command.IgnoreMissing;
                        return FindClick(command.ImageName, FindStrings[command.ImageName], ignoreMissing);
                    case "findclickandwait":
                        if (command.ImageName == null)
                        {
                            _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (!FindStrings.ContainsKey(command.ImageName))
                        {
                            _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return FindClickAndWait(command.ImageName, FindStrings[command.ImageName], (int)command.TimeOut);
                    case "ifexists":
                        if (command.Commands == null)
                        {
                            _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.ImageName == null)
                        {
                            _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (!FindStrings.ContainsKey(command.ImageName))
                        {
                            _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                            return CommandResults.InputError;
                        }
                        return IfExists(command.ImageName, FindStrings[command.ImageName], command.Commands, additionalData);
                    case "ifnotexists":
                        if (command.ImageName == null)
                        {
                            _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.Commands == null)
                        {
                            _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (!FindStrings.ContainsKey(command.ImageName))
                        {
                            _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                            return CommandResults.InputError;
                        }
                        return IfNotExists(command.ImageName, FindStrings[command.ImageName], command.Commands, additionalData);
                    case "loopcoordinates":
                        if (command.Coordinates == null)
                        {
                            _logger.LogError("Command {0} Error Coordinates is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.Commands == null)
                        {
                            _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return LoopCoordinates(command.Coordinates, command.Commands);
                    case "loopuntilfound":
                        if ((command.ImageName == null) && (command.ImageNames == null))
                        {
                            _logger.LogError("Command {0} Error ImageName or ImageNames is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.Commands == null)
                        {
                            _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        imageNames = new List<string>();
                        if (command.ImageName != null)
                        {
                            if (!FindStrings.ContainsKey(command.ImageName))
                            {
                                _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                return CommandResults.InputError;
                            }
                            imageNames.Add(command.ImageName);
                        }
                        if (command.ImageNames != null)
                        {
                            foreach (string item in command.ImageNames)
                            {
                                if (!FindStrings.ContainsKey(item))
                                {
                                    _logger.LogError("Command {0} Error ImageNames {1} doesn't exist in FindStrings", command.CommandId, item);
                                    return CommandResults.InputError;
                                }
                            }
                            imageNames.AddRange(command.ImageNames);
                        }
                        return LoopUntilFound(imageNames, command.Commands, (int)command.TimeOut, additionalData);
                    case "loopuntilnotfound":
                        if ((command.ImageName == null) && (command.ImageNames == null))
                        {
                            _logger.LogError("Command {0} Error ImageName or ImageNames is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.Commands == null)
                        {
                            _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        imageNames = new List<string>();
                        if (command.ImageName != null)
                        {
                            if (!FindStrings.ContainsKey(command.ImageName))
                            {
                                _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                return CommandResults.InputError;
                            }
                            imageNames.Add(command.ImageName);
                        }
                        if (command.ImageNames != null)
                        {
                            foreach (string item in command.ImageNames)
                            {
                                if (!FindStrings.ContainsKey(item))
                                {
                                    _logger.LogError("Command {0} Error ImageNames {1} doesn't exist in FindStrings", command.CommandId, item);
                                    return CommandResults.InputError;
                                }
                            }
                            imageNames.AddRange(command.ImageNames);
                        }
                        return LoopUntilNotFound(imageNames, command.Commands, (int)command.TimeOut, additionalData);
                    case "restart":
                        return CommandResults.Restart;
                    case "runaction":
                        if (command.ActionName == null)
                        {
                            _logger.LogError("Command {0} Error ActionName is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return ExecuteAction(command.ActionName);
                    case "sleep":
                        if (command.Delay == null)
                            return CommandResults.InputError;
                        Thread.Sleep((int)command.Delay);
                        break;
                    case "startgame":
                        if (command.Value == null)
                        {
                            _logger.LogError("Command {0} Error Value is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return ADBStartGame(command.Value, (int)command.TimeOut);
                    case "stopgame":
                        if (command.Value == null)
                        {
                            _logger.LogError("Command {0} Error Value is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return ADBStopGame(command.Value, (int)command.TimeOut);
                    case "waitfor":
                        if (command.ImageName == null)
                        {
                            _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (!FindStrings.ContainsKey(command.ImageName))
                        {
                            _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return WaitFor(command.ImageName, FindStrings[command.ImageName], (int)command.TimeOut);
                    case "waitforthenclick":
                        if (command.ImageName == null)
                        {
                            _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (!FindStrings.ContainsKey(command.ImageName))
                        {
                            _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return WaitForThenClick(command.ImageName, FindStrings[command.ImageName], (int)command.TimeOut);
                    case "waitforchange":
                        if (command.ChangeDetectArea == null)
                        {
                            _logger.LogError("Command {0} Error ChangeDetectArea is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.ChangeDetectDifference == null)
                        {
                            _logger.LogError("Command {0} Error ChangeDetectDifference is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error TimeOut is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return WaitForChange(command.ChangeDetectArea, (float) command.ChangeDetectDifference, (int) command.TimeOut);
                    case "waitfornochange":
                        if (command.ChangeDetectArea == null)
                        {
                            _logger.LogError("Command {0} Error ChangeDetectArea is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.ChangeDetectDifference == null)
                        {
                            _logger.LogError("Command {0} Error ChangeDetectDifference is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        if (command.TimeOut == null)
                        {
                            _logger.LogError("Command {0} Error TimeOut is null", command.CommandId);
                            return CommandResults.InputError;
                        }
                        return WaitForNoChange(command.ChangeDetectArea, (float)command.ChangeDetectDifference, (int)command.TimeOut);
                    default:
                        _logger.LogError("Unrecognised Command {0}", command.CommandId);
                        throw new Exception(string.Format("Unrecognised CommandId {0}", command.CommandId));
                }
                return CommandResults.Ok;
            }
        }


    }
}
