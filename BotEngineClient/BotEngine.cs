// <copyright file="BotEngine.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using Microsoft.Extensions.Logging;
using AdvancedSharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using FindTextClient;
using System.Text.RegularExpressions;
using System.Linq;
using Win32FrameBufferClient;
using AdvancedSharpAdbClient.Models;
using AdvancedSharpAdbClient.Receivers;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BotEngineClient
{

    public delegate void BotEngineCallback(BotEngine.CommandResults result);

    public class BotEngine
    {
        private readonly ILogger _logger;

        private readonly AdbServer adbServer;
        private readonly Image? savedImage;
        private readonly AdbClient adbClient;
        private readonly DeviceData adbDevice;
        private ConsoleOutputReceiver adbReceiver;
        private readonly FindText findText;
        private Rectangle adbScreenSize;
        private readonly Framebuffer adbFrameBuffer;
        private Dictionary<string, FindString> FindStrings;
        private Dictionary<string, Action> NormalActions;
        private Dictionary<string, Action> SystemActions;
        private BOTListConfig ListConfig;
        private readonly string EmulatorName;
        private string ThreadActionName;
        private ActionActivity ThreadActionActivity;
        private BotEngineCallback callback;
        private CancellationToken cancellationToken;
        private bool isThreading;
        private StringBuilder activePath;
        private DateTime lastActivityTime;
        private Regex findAgeFromDumsys;
        private bool hasCheckedUserActivityThisAction;
        private bool _useWin32;
        private Win32FrameBuffer _win32FrameBuffer;

        /// <summary>
        /// Enum of all the possible results from executing a command
        /// </summary>
        public enum CommandResults
        {
            Ok,
            Missing,
            Exit,
            TimeOut,
            Restart,
            InputError,
            ADBError,
            Cancelled
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
            EnterText,
            FindClick,
            FindClickAndWait,
            IfExists,
            IfNotExists,
            LoopCoordinates,
            LoopCounter,
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

        /// <summary>
        /// Sets the command that will be executed within a called thread for the engine
        /// </summary>
        /// <param name="ActionName">The name of the action to call</param>
        /// <param name="callbackDelegate">The delegate to use to return results to the caller</param>
        /// <param name="actionActivity">The data to use when executing the action</param>
        public void SetThreadingCommand(string ActionName, BotEngineCallback callbackDelegate, ActionActivity actionActivity)
        {
            ThreadActionName = ActionName;
            ThreadActionActivity = actionActivity;
            callback = callbackDelegate;
        }

        /// <summary>
        /// This executes the action that has been confugred using SetThreadingCommand
        /// </summary>
        /// <param name="obj">The CancellationToken to enable this thread to be cancellable</param>
        public void InitiateThreadingCommand(object obj)
        {
            cancellationToken = (CancellationToken)obj;
            if (ThreadActionName != null)
            {
                isThreading = true;
                CommandResults result = ExecuteAction(ThreadActionName, ThreadActionActivity);
                if (callback != null)
                {
                    callback(result);
                }
                isThreading = false;
            }
        }

        /// <summary>
        /// Checks if a cancellation has been requested, if running in multi thread mode
        /// </summary>
        /// <returns>True if the cancellationToken has a request to cancel set</returns>
        private bool isCancelled()
        {
            if (isThreading)
            {
                if (cancellationToken != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the local variable FindStrings with a new set of strings
        /// </summary>
        /// <param name="findStrings">The FindStrings to load in</param>
        public void ReloadFindStrings(Dictionary<string, FindString> findStrings)
        {
            FindStrings = findStrings;
        }

        /// <summary>
        /// Updates the local variable NormalActions with a new set of actions
        /// </summary>
        /// <param name="normalActions">The Actions to load in</param>
        public void ReloadNormalActions(Dictionary<string, Action> normalActions)
        {
            NormalActions = normalActions;
        }

        /// <summary>
        /// Updates the local variable SystemActions with a new set of actions
        /// </summary>
        /// <param name="systemActions">The Actions to load in</param>
        public void ReloadSystemActions(Dictionary<string, Action> systemActions)
        {
            SystemActions = systemActions;
        }

        /// <summary>
        /// Updates the local variable ListConfig with a new set of config
        /// </summary>
        /// <param name="listConfig">The BOTListConfig to load in</param>
        public void ReloadListConfig(BOTListConfig listConfig)
        {
            ListConfig = listConfig;
        }

        /// <summary>
        /// Class Constructor, which sets the default class settings
        /// </summary>
        /// <param name="ServiceProvider">Logging Service Provider</param>
        /// <param name="ADBPath">The path to the adb.exe executable</param>
        /// <param name="ADBDeviceData">The string that identifies the ADB device to issue commands against</param>
        /// <param name="findStrings">The set of FindStrings to use</param>
        /// <param name="systemActions">The set of Actions that are System actions</param>
        /// <param name="normalActions">The set of Actions that are not System actions</param>
        /// <param name="listConfig">The config which contains the lists used</param>
        /// <param name="UseWin32">True when graphics collection is to be via Win32 instead of ADB</param>
        /// <param name="win32FrameBuffer">The Win32FrameBuffer that points at the window of the emulator</param>
        /// <exception cref="Exception"></exception>
        public BotEngine(IServiceProvider ServiceProvider, string ADBPath, string ADBDeviceData, Dictionary<string, FindString> findStrings, Dictionary<string, Action> systemActions, Dictionary<string, Action> normalActions, BOTListConfig listConfig, bool UseWin32,  Win32FrameBuffer win32FrameBuffer)
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
                //System.Threading.CancellationToken cancellationToken = default;
                //adbFrameBuffer = adbClient.CreateFramebuffer(adbDevice);
                //adbFrameBuffer.Data();
                //adbFrameBuffer = new Framebuffer(adbDevice, adbClient);
                //_ = adbFrameBuffer.RefreshAsync().Wait(3000);
                adbFrameBuffer = adbClient.GetFrameBuffer(adbDevice);
                using (Image localImage = adbFrameBuffer.ToImage())
                {
                    adbScreenSize = new Rectangle(0, 0, localImage.Width, localImage.Height);
                }
                FindStrings = findStrings;
                SystemActions = systemActions;
                NormalActions = normalActions;
                ListConfig = listConfig;
                EmulatorName = adbDevice.Model;
                if (UseWin32)
                {
                    _logger.LogDebug("Resize window");
                    Rectangle windowSize = win32FrameBuffer.ImageSize;
                    windowSize.Width = windowSize.Width + (windowSize.X * 2);  // This assumes that X is the width of the border of the window
                    windowSize.Height = windowSize.Height + windowSize.Y + windowSize.X;  // This assumes that Y is the height of the top of the window frame, and that the bottom border is the same width as the sides.
                    win32FrameBuffer.ResizeEmulator(0, 0, windowSize.Width, windowSize.Height, false);
                    adbScreenSize = new Rectangle(0, 0, win32FrameBuffer.ImageSize.Width, win32FrameBuffer.ImageSize.Height);
                    _useWin32 = true;
                    _win32FrameBuffer = win32FrameBuffer;
                }
            }
            isThreading = false;
            activePath = new StringBuilder();
            lastActivityTime = DateTime.Now;
            findAgeFromDumsys = new Regex("(?:MotionEvent.+age=)([0-9.]+)(?:ms)", RegexOptions.Compiled);
            hasCheckedUserActivityThisAction = false;
        }

        /// <summary>
        /// Executes the named action
        /// </summary>
        /// <param name="actionName">The name of the action to execute</param>
        /// <param name="actionActivity">The config for the action being executed</param>
        /// <returns>Ok on success, Restart if the action needs to rerun, and any other supported result if something goes wrong</returns>
        public CommandResults ExecuteAction(string actionName, ActionActivity actionActivity)
        {
            using (_logger.BeginScope(string.Format("{0}:{1}({2})", EmulatorName,Helpers.CurrentMethodName(), actionName)))
            {
                _logger.LogInformation("Starting Action");
                string currentPath = string.Format("{0}/", actionName);
                string savePath = activePath.ToString();
                activePath.Append(currentPath);
                hasCheckedUserActivityThisAction = false;
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
                // Make sure there has been no user input.
                while (isNonBotActivity() && !isCancelled())
                { 
                    Thread.Sleep(500);
                }
                if (isCancelled())
                {
                    result = CommandResults.Cancelled;
                }
                else
                {
                    foreach (Command item in action.Commands)
                    {
                        result = ExecuteCommand(item, null, actionActivity);
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
                }
                activePath.Clear();
                activePath.Append(savePath);
                return result;
            }
        }

        #region Send input or commands via ADB
        /// <summary>
        /// Issues a swipe command to the ADB controlled device, IF there hasn't been any user input
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="delay"></param>
        /// <returns>Ok where there has been no activity, and Restart where there has been user activity.</returns>
        private CommandResults ADBSwipe(int x1, int y1, int x2, int y2, int delay)
        {
            if (isNonBotActivity())
            {
                return CommandResults.Restart;
            }
            else
            {
                AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("input swipe {0} {1} {2} {3} {4}", x1, y1, x2, y2, delay), adbDevice, adbReceiver);
                lastActivityTime = DateTime.Now;
                return CommandResults.Ok;
            }
        }

        /// <summary>
        /// Issues a tap command to the ADB controlled device, IF there hasn't been any user input
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Ok where there has been no activity, and Restart where there has been user activity.</returns>
        private CommandResults ADBClick(int x, int y)
        {
            if (isNonBotActivity())
            {
                return CommandResults.Restart;
            }
            else
            {
                AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("input tap {0} {1}", x, y), adbDevice, adbReceiver);
                lastActivityTime = DateTime.Now;
                return CommandResults.Ok;
            }
        }

        /// <summary>
        /// Issues an input keyboard text command to the ADB controlled device, IF there hasn't been any user input
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Ok where there has been no activity, and Restart where there has been user activity.</returns>
        private CommandResults ADBSendKeys(string text)
        {
            if (isNonBotActivity())
            {
                return CommandResults.Restart;
            }
            else
            {
                AdbClientExtensions.ExecuteRemoteCommand(adbClient, string.Format("input keyboard text '{0}'", text), adbDevice, adbReceiver);
                lastActivityTime = DateTime.Now;
                return CommandResults.Ok;
            }
        }

        /// <summary>
        /// Starts an application on the ADB controlled device
        /// </summary>
        /// <param name="gameString">The string that identifies the app to start</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
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
                    string currentFocus = cmdResult[cmdResult.IndexOf("mCurrentFocus")..];
                    currentFocus = currentFocus.Substring(0, currentFocus.IndexOf("\n"));
                    string focusedApp = cmdResult[cmdResult.IndexOf("mFocusedApp")..];
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
                lastActivityTime = DateTime.Now;
                return results;
            }
        }

        /// <summary>
        /// Stops an application on the ADB controlled device
        /// </summary>
        /// <param name="gameString">The string that identifies the app to stop</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
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
                    string currentFocus = cmdResult[cmdResult.IndexOf("mCurrentFocus")..];
                    currentFocus = currentFocus.Substring(0, currentFocus.IndexOf("\n"));
                    string focusedApp = cmdResult[cmdResult.IndexOf("mFocusedApp")..];
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
                lastActivityTime = DateTime.Now;
                return results;
            }
        }

        /// <summary>
        /// Determines if there has been any non bot input to the emulator since the last activity.
        /// If there has been activity, then the activity must be older than 30 sceonds.
        /// </summary>
        /// <returns></returns>
        private bool isNonBotActivity()
        {
            if (!hasCheckedUserActivityThisAction)
            {
                hasCheckedUserActivityThisAction = true;
                double minimumAge = GetAgeLastActivity();
                // Now calculate how many milliseconds since Bot sent input
                double elapsed = (DateTime.Now - lastActivityTime).TotalMilliseconds;
                if (elapsed - minimumAge > 500)  // allow 1/2 a second buffer for general crap
                {
                    while (minimumAge < 30000.0)
                    {
                        // There has been input since the last activity.
                        _logger.LogInformation("There has been user input {0}ms ago, with the bots last action {1}ms ago, waiting until last user input was 30 seconds ago", minimumAge, elapsed);
                        int waitTime = (30000 - (int)minimumAge) / 10;
                        for (int i = 0; i < 11; i++)
                        {
                            Thread.Sleep(waitTime);
                            if (isCancelled())
                            {
                                return true;
                            }
                        }
                        minimumAge = GetAgeLastActivity();
                        elapsed = (DateTime.Now - lastActivityTime).TotalMilliseconds;
                    }
                    if (minimumAge > 30000.0)  // if the last user input was more than 30 seconds ago, then assume that have stopped doing stuff
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            return false;

            
            double GetAgeLastActivity()
            {
                // Get the latest input time from ADB and dumpsys input
                ConsoleOutputReceiver dumpsysReceiver = new ConsoleOutputReceiver(null);
                AdbClientExtensions.ExecuteRemoteCommand(adbClient, "dumpsys input", adbDevice, dumpsysReceiver);
                dumpsysReceiver.Flush();
                string results = dumpsysReceiver.ToString();
                // Get rid of any ANR (Application Not Responding) which can have stale MotionEvents in them
                if (results.Contains("Input Dispatcher State at time of last ANR"))
                {
                    results = results[..results.IndexOf("Input Dispatcher State at time of last ANR")];
                }
                // Use regex to find all the Ages (age=4008.8ms) after a MotionEvent.
                MatchCollection foundAges = findAgeFromDumsys.Matches(results);
                // Parse through the matches to get the minimum elapsed time
                double minimumAge = double.MaxValue;
                foreach (Match item in foundAges.Cast<Match>())
                {
                    foreach (Group group in item.Groups)
                    {
                        if (double.TryParse(group.Value, out double milliseconds))
                        {
                            if (minimumAge > milliseconds)
                            {
                                minimumAge = milliseconds;
                            }
                        }
                    }
                }

                return minimumAge;
            }
        }
        #endregion

        /// <summary>
        /// Waits until one of the images indicated by searchNames appears on the screen.
        /// </summary>
        /// <param name="searchNames"></param>
        /// <param name="ignoreMissing">Set to true if it is not a Missing state if the timeout has expired.</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        private CommandResults WaitFor(List<string> searchNames, bool ignoreMissing, int timeOut)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchNames[0])))
            {
                CommandResults result = CommandResults.TimeOut;
                System.Threading.CancellationToken cancellationToken = default;
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                while (stopWatch.Elapsed.TotalMilliseconds < timeOut)
                {
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;

                    if (!_useWin32) { adbFrameBuffer.Refresh(false); }
                    using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                    }
                    foreach (string searchName in searchNames)
                    {
                        if (!FindStrings.ContainsKey(searchName))
                        {
                            _logger.LogError("FindString {0} is missing from json file", searchName);
                            return CommandResults.InputError;
                        }
                        _logger.LogDebug("Searching for {0}", searchName);
                        FindString searchString = FindStrings[searchName];

                        List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X + searchString.SearchArea.Width, searchString.SearchArea.Y + searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, false, false, searchString.OffsetX, searchString.OffsetY);
                        if (dataresult != null)
                        {
                            result = CommandResults.Ok;
                            _logger.LogDebug("Search Successful, found {0} whilst looking for {1}", dataresult[0].Id, searchName);
                            break;
                        }
                        else
                        {
                            _logger.LogDebug("Search Unsuccessful, found nothing whilst looking for {0}", searchName);
                        }
                    }
                    if (result == CommandResults.Ok)
                    {
                        break;
                    }
                    Thread.Sleep(50);
                }
                stopWatch.Stop();
                if (result != CommandResults.Ok && !ignoreMissing)
                {
                    _logger.LogWarning("Search Unsuccessful with {0} whilst looking for {1}", result.ToString(), searchNames[0]);
                }
                else if (result != CommandResults.Ok)
                {
                    _logger.LogDebug("Search Unsuccessful with {0} whilst looking for {1}, but ignoreMissing = true", result.ToString(), searchNames[0]);
                    result = CommandResults.Ok;
                }
                return result;
            }
        }

        /// <summary>
        /// Waits until the image identified by searchString appears, and then clicks on it.  
        /// If the wait time exceeds timeOut, then the function will return TimeOut
        /// </summary>
        /// <param name="searchName">The search name used in logging</param>
        /// <param name="searchString">The FindString that is being searched for</param>
        /// <param name="timeOut">How long in ms to keep searching for</param>
        /// <returns>Ok, when found and clicked, else Timeout</returns>
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
                    if (!_useWin32) adbFrameBuffer.Refresh(false);
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                    using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                    }

                    List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X + searchString.SearchArea.Width, searchString.SearchArea.Y + searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, false, false, searchString.OffsetX, searchString.OffsetY);
                    if (dataresult != null)
                    {
                        _logger.LogDebug("Initial Search Successful, found {0} whilst looking for {1}, Clicking", dataresult[0].Id, searchName);
                        result = ADBClick(dataresult[0].X, dataresult[0].Y);
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

        /// <summary>
        /// Looks for the image described by findString, and if found executes the Commands. 
        /// </summary>
        /// <param name="searchName">Text string used for logging</param>
        /// <param name="findString">The image to search for</param>
        /// <param name="Commands">The list of Command(s) to be executed if the image is found</param>
        /// <param name="additionalData">An optional object which will be passed to all Commands that are executed if the image is found</param>
        /// <param name="actionActivity">The ActionActivity which will be passed to all Commands that are executed if the image is found</param>
        /// <returns>Ok, if all works, or the result from the 1st Command that fails</returns>
        private CommandResults IfExists(string searchName, FindString findString, List<Command> Commands, Object? additionalData, ActionActivity actionActivity)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.Missing;
                System.Threading.CancellationToken cancellationToken = default;
                if (!_useWin32) adbFrameBuffer.Refresh(false);
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                {
                    findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                }

                List<SearchResult>? dataresult = findText.SearchText(findString.SearchArea.X, findString.SearchArea.Y, findString.SearchArea.X+findString.SearchArea.Width, findString.SearchArea.Y+findString.SearchArea.Height, findString.BackgroundTolerance, findString.TextTolerance, findString.SearchString, false, false, false, findString.OffsetX, findString.OffsetY);
                if (dataresult != null)
                {
                    _logger.LogDebug("Search Successful, found {0} whilst looking for {1}", dataresult[0].Id, searchName);
                    result = CommandResults.Ok;
                    foreach (Command command in Commands)
                    {
                        result = ExecuteCommand(command, additionalData, actionActivity);
                        if (result != CommandResults.Ok)
                        {
                            return result;
                        }
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

        /// <summary>
        /// Looks for the image described by findString, and if not found executes the Commands. 
        /// </summary>
        /// <param name="searchName">Text string used for logging</param>
        /// <param name="findString">The image to search for</param>
        /// <param name="Commands">The list of Command(s) to be executed if the image is not found</param>
        /// <param name="additionalData">An optional object which will be passed to all Commands that are executed if the image is not found</param>
        /// <param name="actionActivity">The ActionActivity which will be passed to all Commands that are executed if the image is not found</param>
        /// <returns>Ok, if all works, or the result from the 1st Command that fails</returns>
        private CommandResults IfNotExists(string searchName, FindString findString, List<Command> Commands, Object? additionalData, ActionActivity actionActivity)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.Missing;
                System.Threading.CancellationToken cancellationToken = default;
                if (!_useWin32) adbFrameBuffer.Refresh(false);
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                {
                    findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                }

                List<SearchResult>? dataresult = findText.SearchText(findString.SearchArea.X, findString.SearchArea.Y, findString.SearchArea.X + findString.SearchArea.Width, findString.SearchArea.Y + findString.SearchArea.Height, findString.BackgroundTolerance, findString.TextTolerance, findString.SearchString, false, false, false, findString.OffsetX, findString.OffsetY);
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
                        result = ExecuteCommand(command, additionalData, actionActivity);
                        if (result != CommandResults.Ok)
                        {
                            return result;
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Searches for the image identified by searchString, and if found, clicks on it.
        /// </summary>
        /// <param name="searchName">Text string used for logging</param>
        /// <param name="searchString">The image to search for</param>
        /// <param name="IgnoreMissing">If the image is not there, then returns Ok.</param>
        /// <returns>Ok if found, else Missing</returns>
        private CommandResults FindClick(string searchName, FindString searchString, bool IgnoreMissing)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.Missing;
                System.Threading.CancellationToken cancellationToken = default;
                if (!_useWin32) adbFrameBuffer.Refresh(false);
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                {
                    findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                }

                List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X + searchString.SearchArea.Width, searchString.SearchArea.Y + searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, false, false, searchString.OffsetX, searchString.OffsetY);
                if (dataresult != null)
                {
                    _logger.LogDebug("Initial Search Successful, found {0} whilst looking for {1}, Clicking", dataresult[0].Id, searchName);
                    result = ADBClick(dataresult[0].X, dataresult[0].Y);
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

        /// <summary>
        /// Searches for a number of potenial targets, and will click on the 1st one found, and then wait for that image to disappear.
        /// If none of the images are found, then exit with Missing.
        /// If the image doesn't disappear, then exit with Timeout.
        /// </summary>
        /// <param name="searchName"></param>
        /// <param name="searchString"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        private CommandResults FindClickAndWait(List<string> searchNames, bool missingOk, int timeOut)
        {
            string lookingFor = searchNames[0];
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), lookingFor)))
            {
                bool found = false;
                bool stillThere = false;
                SearchResult foundAt = new SearchResult();
                CommandResults result = CommandResults.Missing;
                System.Threading.CancellationToken cancellationToken = default;
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                FindString searchString = null;
                if (timeOut == -1)
                    timeOut = int.MaxValue;

                // Find the 1st item from the list
                if (!_useWin32) adbFrameBuffer.Refresh(false);
                zx = adbScreenSize.X; zy = adbScreenSize.Y; w = adbScreenSize.Width; h = adbScreenSize.Height;
                using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                {
                    findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                }

                foreach (string item in searchNames)
                {
                    if (!FindStrings.ContainsKey(item))
                    {
                        _logger.LogError("FindString {0} is missing from json file", item);
                        return CommandResults.InputError;
                    }
                    searchString = FindStrings[item];
                    lookingFor = item;
                    _logger.LogDebug("Searching for {0}", item);
                    List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X + searchString.SearchArea.Width, searchString.SearchArea.Y + searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, false, false, searchString.OffsetX, searchString.OffsetY);
                    if (dataresult != null)
                    {
                        _logger.LogDebug("Initial Search Successful, found {0} whilst looking for {1}, Clicking", dataresult[0].Id, lookingFor);
                        result = CommandResults.TimeOut;
                        if (ADBClick(dataresult[0].X, dataresult[0].Y) != CommandResults.Ok)
                        {
                            return CommandResults.Restart;
                        }
                        found = true;
                        foundAt = new SearchResult(dataresult[0].TopLeftX, dataresult[0].TopLeftY, dataresult[0].Width, dataresult[0].Height, dataresult[0].X, dataresult[0].Y, dataresult[0].Id);
                        break;
                    }
                }

                if (found)
                {
                    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                    stopWatch.Start();
                    while (stopWatch.Elapsed.TotalMilliseconds < timeOut)
                    {
                        Thread.Sleep(50);
                        if (!_useWin32) adbFrameBuffer.Refresh(false);
                        zx = adbScreenSize.X; zy = adbScreenSize.Y; w = adbScreenSize.Width; h = adbScreenSize.Height;
                        using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                        {
                            findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                        }

                        List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X + searchString.SearchArea.Width, searchString.SearchArea.Y + searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, false, false, searchString.OffsetX, searchString.OffsetY);
                        if (dataresult != null)
                        {
                            stillThere = false;
                            foreach (SearchResult item in dataresult)
                            {
                                if ((item.X > foundAt.TopLeftX && item.X < foundAt.TopLeftX + foundAt.Width)
                                    && (item.Y > foundAt.TopLeftY && item.X < foundAt.TopLeftY + foundAt.Height))
                                {
                                    _logger.LogDebug("Secondary Search Successful (bad) whilst looking for {0}", lookingFor);
                                    stillThere = true;
                                    break;
                                }
                            }
                            if (!stillThere)
                            {
                                _logger.LogDebug("Secondary Search Unsuccessful (good) whilst looking for {0}", lookingFor);
                                result = CommandResults.Ok;
                                break;
                            }
                        }
                        else
                        {
                            _logger.LogDebug("Secondary Search Unsuccessful (good) whilst looking for {0}", lookingFor);
                            result = CommandResults.Ok;
                            break;
                        }
                    }
                    stopWatch.Stop();
                }
                if (result == CommandResults.Missing && missingOk)
                {
                    _logger.LogInformation("Unsuccessful with {0} whilst looking for {1}, but ignored due to MissingOk", result.ToString(), lookingFor);
                    result = CommandResults.Ok;
                }
                else if (result != CommandResults.Ok)
                {
                    _logger.LogWarning("Unsuccessful with {0} whilst looking for {1}", result.ToString(), lookingFor);
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
        private CommandResults ClickWhenNotFoundInArea(string searchName, FindString searchString, List<SearchArea> areas)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), searchName)))
            {
                CommandResults result = CommandResults.Ok;  // Return OK even when not found, as not found also means nothing to do.
                if (areas.Count > 0)
                {
                    System.Threading.CancellationToken cancellationToken = default;
                    if (!_useWin32) adbFrameBuffer.Refresh(false);
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                    using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                    {
                        findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                    }

                    int x1 = int.MaxValue, y1 = int.MaxValue, x2 = int.MinValue, y2 = int.MinValue;
                    foreach (SearchArea area in areas)
                    {
                        x1 = Math.Min(x1, area.X);
                        y1 = Math.Min(y1, area.Y);
                        x2 = Math.Max(x2, area.X + area.Width);
                        y2 = Math.Max(y2, area.Y + area.Height);
                    }

                    List<SearchResult>? dataresults = findText.SearchText(x1, y1, x2, y2, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, true, false, searchString.OffsetX, searchString.OffsetY);
                    if (dataresults == null)
                    {
                        _logger.LogDebug("Search Unsuccessful, found nothing at {0} whilst looking for {1}, Clicking", areas[0].ToString(), searchName);
                        int x = areas[0].X + (areas[0].Width / 2);
                        int y = areas[0].Y + (areas[0].Height / 2);
                        result = ADBClick(x, y);
                    }
                    else
                    {
                        bool found = false;
                        foreach (SearchArea area in areas)
                        {
                            found = false;
                            foreach (SearchResult dataresult in dataresults)
                            {
                                if ((dataresult.X > area.X) && (dataresult.X < area.X + area.Width)
                                    && (dataresult.Y > area.Y) && (dataresult.Y < area.Y + area.Height))
                                {
                                    found = true;
                                    _logger.LogDebug("Partial Search Successful, whilst looking for {0} at {1}", searchName, area.ToString());
                                    break;
                                }
                            }
                            if (!found)
                            {
                                _logger.LogDebug("Partial Search Unsuccessful, Clicking, whilst looking for {0} at {1}", searchName, area.ToString());
                                int x = area.X + (area.Width / 2);
                                int y = area.Y + (area.Height / 2);
                                result = ADBClick(x, y);
                                break;
                            }
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Loops though a list of coordinates and executes each command for that set of coordinates.
        /// If there is a failure, then each execution from then (until complete success) will start
        /// from the point that the previous execution failed.
        /// </summary>
        /// <param name="CoordinateName"></param>
        /// <param name="Commands"></param>
        /// <param name="actionActivity"></param>
        /// <returns></returns>
        private CommandResults LoopCoordinates(string CoordinateName, List<Command> Commands, ActionActivity actionActivity)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), CoordinateName)))
            {
                CommandResults result;
                if (ListConfig.Coordinates == null)
                    return CommandResults.InputError;
                if (!ListConfig.Coordinates.ContainsKey(CoordinateName))
                    return CommandResults.InputError;
                List<XYCoords> coords = ListConfig.Coordinates[CoordinateName];
                int startAt = GetLastKnownLoopStatusFromActionActivity(coords.Count, actionActivity);
                int counter = 0;
                foreach (XYCoords point in coords)
                {
                    if (startAt > counter)
                    {
                        counter++;
                        continue;
                    }
                    actionActivity.CommandLoopStatus[activePath.ToString()] = counter.ToString();
                    foreach (Command command in Commands)
                    {
                        result = ExecuteCommand(command, point, actionActivity);
                        if (result != CommandResults.Ok)
                        {
                            return result;
                        }
                    }
                    counter++;
                }
                return CommandResults.Ok;
            }
        }

        #region Loop Until Found / Not Found

        /// <summary>
        /// Runs the Commands list, until all of the imageNames disappear.
        /// ToDo: Refactor so it searches for each items separately, as it's finding stuff outside the bounds of the individual searches
        ///         If the 2nd search is done in an individual level.
        /// </summary>
        /// <param name="imageNames"></param>
        /// <param name="Commands"></param>
        /// <param name="timeOut"></param>
        /// <param name="additionalData"></param>
        /// <param name="actionActivity"></param>
        /// <returns></returns>
        private CommandResults LoopUntilNotFound(List<string> imageNames, bool ignoreMissing, List<Command> Commands, int timeOut, Object? additionalData, ActionActivity actionActivity, bool V2 = true)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.Ok;
                System.Threading.CancellationToken cancellationToken = default;
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                int searchX = w, searchY = h, searchW = 0, searchH = 0;
                float textFactor = 0.0f, backFactor = 0.0f;
                string searchString = string.Empty;
                FindString findString = null;
                if (timeOut == -1)
                    timeOut = int.MaxValue;

                List<SearchResult>? dataresult = null;

                if (V2 == true)
                {
                    string lookingFor = string.Empty;
                    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                    stopWatch.Start();
                    do
                    {
                        if (!_useWin32) adbFrameBuffer.Refresh(false);
                        zx = adbScreenSize.X; zy = adbScreenSize.Y; w = adbScreenSize.Width; h = adbScreenSize.Height;
                        using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                        {
                            findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                        }

                        foreach (string item in imageNames)
                        {
                            if (!FindStrings.ContainsKey(item))
                            {
                                _logger.LogError("FindString {0} is missing from json file", item);
                                return CommandResults.InputError;
                            }
                            findString = FindStrings[item];
                            lookingFor = item;
                            _logger.LogDebug("Searching for {0}", item);

                            dataresult = findText.SearchText(findString.SearchArea.X, findString.SearchArea.Y, findString.SearchArea.X + findString.SearchArea.Width, findString.SearchArea.Y + findString.SearchArea.Height, findString.BackgroundTolerance, findString.TextTolerance, findString.SearchString, false, false, false, findString.OffsetX, findString.OffsetY);
                            if (dataresult != null)
                            {
                                _logger.LogDebug("Search Successful, found {0}", dataresult[0].Id);
                                foreach (Command command in Commands)
                                {
                                    result = ExecuteCommand(command, additionalData, actionActivity);
                                    if (result != CommandResults.Ok)
                                    {
                                        return result;
                                    }
                                }
                                break;
                            }
                        }
                        Thread.Sleep(50);
                    } while ((dataresult != null) && (stopWatch.ElapsedMilliseconds < timeOut)) ;
                }
                else
                {
                    // Get the largest bounding box for searching, and the most permissive Factors.
                    foreach (string item in imageNames)
                    {
                        if (!FindStrings.ContainsKey(item))
                        {
                            _logger.LogError("FindString {0} is missing from json file", item);
                            return CommandResults.InputError;
                        }
                        _logger.LogDebug("Adding search for {0}", item);
                        findString = FindStrings[item];
                        searchString += findString.SearchString;
                        searchX = Math.Min(searchX, findString.SearchArea.X);
                        searchY = Math.Min(searchY, findString.SearchArea.Y);
                        searchW = Math.Max(searchW, findString.SearchArea.X + findString.SearchArea.Width);
                        searchH = Math.Max(searchH, findString.SearchArea.Y + findString.SearchArea.Height);
                        textFactor = Math.Max(textFactor, findString.TextTolerance);
                        backFactor = Math.Max(backFactor, findString.BackgroundTolerance);
                    }

                    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                    stopWatch.Start();
                    do
                    {
                        if (!_useWin32) adbFrameBuffer.Refresh(false);
                        using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
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
                                result = ExecuteCommand(command, additionalData, actionActivity);
                                if (result != CommandResults.Ok)
                                {
                                    return result;
                                }
                            }
                        }
                        Thread.Sleep(50);
                    } while ((dataresult != null) && (stopWatch.ElapsedMilliseconds < timeOut));
                }

                if (dataresult == null)
                {
                    _logger.LogDebug("Item(s) have now disappeared");
                    result = CommandResults.Ok;
                }
                else
                {
                    if (ignoreMissing)
                    {
                        _logger.LogInformation("Item(s) haven't disappeared before Timeout, but ignored due to ignoreMissing");
                        result = CommandResults.Ok;
                    }
                    else
                    {
                        _logger.LogWarning("Item(s) haven't disappeared before Timeout");
                        result = CommandResults.TimeOut;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Runs the Commands list, until at least one of the imageNames shows up.
        /// </summary>
        /// <param name="imageNames"></param>
        /// <param name="Commands"></param>
        /// <param name="timeOut"></param>
        /// <param name="additionalData"></param>
        /// <param name="actionActivity"></param>
        /// <returns></returns>
        private CommandResults LoopUntilFound(List<string> imageNames, bool ignoreMissing, List<Command> Commands, int timeOut, Object? additionalData, ActionActivity actionActivity, bool V2 = true)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.Ok;
                System.Threading.CancellationToken cancellationToken = default;
                int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                int searchX = w, searchY = h, searchW = 0, searchH = 0;
                float textFactor = 0.0f, backFactor = 0.0f;
                string searchString = string.Empty;
                FindString findString = null;
                if (timeOut == -1)
                    timeOut = int.MaxValue;

                List<SearchResult>? dataresult = null;

                if (V2 == true)
                {
                    string lookingFor = string.Empty;
                    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                    stopWatch.Start();
                    do
                    {
                        if (!_useWin32) adbFrameBuffer.Refresh(false);
                        zx = adbScreenSize.X; zy = adbScreenSize.Y; w = adbScreenSize.Width; h = adbScreenSize.Height;
                        using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                        {
                            findText.LoadImage(localImage, ref zx, ref zy, ref w, ref h);
                        }

                        foreach (string item in imageNames)
                        {
                            if (!FindStrings.ContainsKey(item))
                            {
                                _logger.LogError("FindString {0} is missing from json file", item);
                                return CommandResults.InputError;
                            }
                            findString = FindStrings[item];
                            lookingFor = item;
                            _logger.LogDebug("Searching for {0}", item);

                            dataresult = findText.SearchText(findString.SearchArea.X, findString.SearchArea.Y, findString.SearchArea.X + findString.SearchArea.Width, findString.SearchArea.Y + findString.SearchArea.Height, findString.BackgroundTolerance, findString.TextTolerance, findString.SearchString, false, false, false, findString.OffsetX, findString.OffsetY);
                            if (dataresult != null)
                            {
                                _logger.LogDebug("Search Successful, found {0}", dataresult[0].Id);
                                break;
                            }
                        }
                        if (dataresult == null)
                        {
                            _logger.LogDebug("Search Unsuccessful, Execute Commands");
                            foreach (Command command in Commands)
                            {
                                result = ExecuteCommand(command, additionalData, actionActivity);
                                if (result != CommandResults.Ok)
                                {
                                    return result;
                                }
                            }
                            Thread.Sleep(50);
                        }
                    } while ((dataresult == null) && (stopWatch.ElapsedMilliseconds < timeOut));
                }
                else
                {

                    // Get the largest bounding box for searching, and the most permissive Factors.
                    foreach (string item in imageNames)
                    {
                        if (!FindStrings.ContainsKey(item))
                        {
                            _logger.LogError("FindString {0} is missing from json file", item);
                            return CommandResults.InputError;
                        }
                        findString = FindStrings[item];
                        searchString += findString.SearchString;
                        searchX = Math.Min(searchX, findString.SearchArea.X);
                        searchY = Math.Min(searchY, findString.SearchArea.Y);
                        searchW = Math.Max(searchW, findString.SearchArea.X + findString.SearchArea.Width);
                        searchH = Math.Max(searchH, findString.SearchArea.Y + findString.SearchArea.Height);
                        textFactor = Math.Max(textFactor, findString.TextTolerance);
                        backFactor = Math.Max(backFactor, findString.BackgroundTolerance);
                    }
                    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                    stopWatch.Start();
                    do
                    {
                        if (!_useWin32) { adbFrameBuffer.Refresh(false); }
                        using (Image localImage = (_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
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
                                result = ExecuteCommand(command, additionalData, actionActivity);
                                if (result != CommandResults.Ok)
                                {
                                    return result;
                                }
                            }
                        }
                        Thread.Sleep(50);
                    } while ((dataresult == null) && (stopWatch.ElapsedMilliseconds < timeOut));
                }
                if (dataresult != null)
                    _logger.LogDebug("Item(s) have now appeared");
                else
                {
                    if (ignoreMissing)
                    {
                        _logger.LogInformation("Item(s) haven't appeared before Timeout, but ignored due to ignoreMissing");
                        result = CommandResults.Ok;
                    }
                    else
                    {
                        _logger.LogWarning("Item(s) haven't appeared before Timeout");
                        result = CommandResults.TimeOut;
                    }
                }
                return result;
            }
        }
        #endregion

        /// <summary>
        /// Calls the child commands in order, the number of loops specified.
        /// </summary>
        /// <param name="numberOFLoops"></param>
        /// <param name="commands"></param>
        /// <param name="additionalData"></param>
        /// <returns></returns>
        private CommandResults LoopCounter(int numberOFLoops, List<Command> commands, object additionalData, ActionActivity actionActivity)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.Ok;
                int startAt = GetLastKnownLoopStatusFromActionActivity(numberOFLoops, actionActivity);
                //if (actionActivity.CommandValueOverride != null && actionActivity.CommandValueOverride.ContainsKey(activePath.ToString()))
                //{
                //    int.TryParse(actionActivity.CommandValueOverride[activePath.ToString()], out numberOFLoops);
                //}
                for (int i = startAt; i < numberOFLoops; i++)
                {
                    _logger.LogInformation("Executing Loop {0} of {1}", i, numberOFLoops);
                    foreach (Command command in commands)
                    {
                        _logger.LogDebug("Executing Loop {0} command {1}", i, command.CommandId);
                        result = ExecuteCommand(command, additionalData, actionActivity);
                        if (result != CommandResults.Ok)
                        {
                            _logger.LogWarning("Exiting on Loop {0} command {1} due to result {2}", i, command.CommandId, result);
                            return result;
                        }
                    }
                    // Update the loop counter storage if it was successful.
                    if (result == CommandResults.Ok)
                        actionActivity.CommandLoopStatus[activePath.ToString()] = i.ToString();
                }
                // Reset the Loop Counter to 0 if the loop has completed successfully.
                if (result == CommandResults.Ok)
                {
                    actionActivity.CommandLoopStatus[activePath.ToString()] = "0";
                }
                return result;
            }
        }

        /// <summary>
        /// Retrieves the loop number from CommandLoopStatus for the current active path.
        /// </summary>
        /// <param name="numberOFLoops">Set this to MaxInt if this isn't a counter operation</param>
        /// <param name="actionActivity"></param>
        /// <returns></returns>
        private int GetLastKnownLoopStatusFromActionActivity(int numberOFLoops, ActionActivity actionActivity)
        {
            int startAt = 0;

            if (actionActivity.CommandLoopStatus == null)
            {
                actionActivity.CommandLoopStatus = new Dictionary<string, string>();
                actionActivity.CommandLoopStatus.Add(activePath.ToString(), "0");
            }
            else
            {
                if (actionActivity.CommandLoopStatus.ContainsKey(activePath.ToString()))
                {
                    if (int.TryParse(actionActivity.CommandLoopStatus[activePath.ToString()], out startAt))
                    {
                        if (startAt >= numberOFLoops - 1)  // Zero based, so subtract 1...
                        {
                            startAt = 0;
                        }
                    }
                    else
                    {
                        startAt = 0;
                    }
                }
                else
                {
                    actionActivity.CommandLoopStatus.Add(activePath.ToString(), "0");
                }
            }

            return startAt;
        }

        /// <summary>
        /// Waits for a change to happen within a defined area on the devices screen
        /// </summary>
        /// <param name="changeDetectArea">The area of the screen to watch</param>
        /// <param name="changeDetectDifference">The percentage of change that needs to happen</param>
        /// <param name="timeOut">How long to wait for a change</param>
        /// <returns>Ok, if changes happen, or TimeOut if no change detected.</returns>
        private CommandResults WaitForChange(SearchArea changeDetectArea, float changeDetectDifference, int timeOut)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.TimeOut;
                System.Threading.CancellationToken cancellationToken = default;
                int iterationWait = Math.Max(timeOut / 20, 50);


                if (!_useWin32) adbFrameBuffer.Refresh(false);
                using (Bitmap savedImage = (Bitmap)(_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                {
                    Rectangle srcRect = new Rectangle(changeDetectArea.X, changeDetectArea.Y, changeDetectArea.Width, changeDetectArea.Height);
                    using (Bitmap savedPart = savedImage.Clone(srcRect, savedImage.PixelFormat))
                    {
                        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                        stopWatch.Start();
                        do
                        {
                            Thread.Sleep(iterationWait);
                            if (!_useWin32) adbFrameBuffer.Refresh(false);
                            using (Bitmap newImage = (Bitmap)(_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
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

        /// <summary>
        /// If there are changes within the changeDetectArea that exceed the changeDetectDifference, then return Missing.
        /// Use case is where you want to carry out an action if nothing has changed.
        /// </summary>
        /// <param name="changeDetectArea"></param>
        /// <param name="changeDetectDifference"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        private CommandResults WaitForNoChange(SearchArea changeDetectArea, float changeDetectDifference, int timeOut)
        {
            using (_logger.BeginScope(Helpers.CurrentMethodName()))
            {
                CommandResults result = CommandResults.Ok;
                System.Threading.CancellationToken cancellationToken = default;
                int iterationWait = Math.Max(timeOut / 20, 50);


                if (!_useWin32) adbFrameBuffer.Refresh(false);
                using (Bitmap savedImage = (Bitmap)(_useWin32 ? _win32FrameBuffer.ToImage() : adbFrameBuffer.ToImage()))
                {
                    Rectangle srcRect = new Rectangle(changeDetectArea.X, changeDetectArea.Y, changeDetectArea.Width, changeDetectArea.Height);
                    using (Bitmap savedPart = savedImage.Clone(srcRect, savedImage.PixelFormat))
                    {
                        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                        stopWatch.Start();
                        do
                        {
                            Thread.Sleep(iterationWait);
                            if (!_useWin32) adbFrameBuffer.Refresh(false);
                            using (Bitmap newImage = (Bitmap) (_useWin32 ? _win32FrameBuffer.ToImage() :adbFrameBuffer.ToImage()))
                            {
                                using (Bitmap newPart = newImage.Clone(srcRect, newImage.PixelFormat))
                                {
                                    float difference = ImageTool.GetPercentageDifference(savedPart, newPart, 3);
                                    _logger.LogDebug("Image Difference was {0}", difference);
                                    if (difference >= changeDetectDifference)
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

        /// <summary>
        /// Controller to call all the various commands, handling all the validation etc.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="additionalData"></param>
        /// <param name="actionActivity"></param>
        /// <returns></returns>
        private CommandResults ExecuteCommand(Command command, Object? additionalData, ActionActivity actionActivity)
        {
            using (_logger.BeginScope(String.Format("{0}({1})", Helpers.CurrentMethodName(), command.CommandId)))
            {
                string currentPath = string.Empty;
                CommandResults results = CommandResults.Ok;

                List<string> imageNames = new List<string>();
                bool ignoreMissing = false;
                if (isCancelled())
                {
                    _logger.LogWarning("Command {0} has been cancelled by thread request", command.CommandId);
                    return CommandResults.Cancelled;
                }
                _logger.LogDebug("Starting Command Execution");
                currentPath = string.Format("{0}.{1}/", command.CommandId, command.CommandNumber);
                string savePath = activePath.ToString();
                activePath.Append(currentPath);

                if (Enum.TryParse(command.CommandId, true, out ValidCommandIds validCommandIds))
                {
                    switch (validCommandIds)
                    {
                        case ValidCommandIds.Click:
                            if (command.Location == null)
                            {
                                _logger.LogError("Command {0} Error Location is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = ADBClick(command.Location.X, command.Location.Y);
                            }
                            break;
                        case ValidCommandIds.ClickWhenNotFoundInArea:
                            if (command.Areas == null)
                            {
                                _logger.LogError("Command {0} Error Areas is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.ImageName == null)
                            {
                                _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (!FindStrings.ContainsKey(command.ImageName))
                            {
                                _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = ClickWhenNotFoundInArea(command.ImageName, FindStrings[command.ImageName], command.Areas);
                            }
                            break;
                        case ValidCommandIds.Drag:
                            if (command.Swipe == null)
                            {
                                _logger.LogError("Command {0} Error Swipe is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.Delay == null)
                            {
                                _logger.LogError("Command {0} Error Delay is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = ADBSwipe(command.Swipe.X1, command.Swipe.Y1, command.Swipe.X2, command.Swipe.Y2, (int)command.Delay);
                            }
                            break;
                        case ValidCommandIds.Exit:
                            return CommandResults.Exit;
                        case ValidCommandIds.EnterLoopCoordinate:
                            if (command.Value == null)
                            {
                                _logger.LogError("Command {0} Error Value is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (additionalData == null)
                            {
                                _logger.LogError("Command {0} Error additionalData is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (!(additionalData is XYCoords))
                            {
                                _logger.LogError("Command {0} Error additionalData is missing XYCoords", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.Value.ToLower() == "x")
                            {
                                results = ADBSendKeys(((XYCoords)additionalData).X.ToString());
                            }
                            else
                            {
                                results = ADBSendKeys(((XYCoords)additionalData).Y.ToString());
                            }
                            break;
                        case ValidCommandIds.EnterText:
                            if (command.Value == null)
                            {
                                _logger.LogError("Command {0} Error Value is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            string commandValue = command.Value;
                            if (actionActivity.CommandValueOverride != null && command.OverrideId != null)
                            {
                                if (actionActivity.CommandValueOverride.ContainsKey(command.OverrideId))
                                {
                                    if (actionActivity.CommandValueOverride[command.OverrideId] != null)
                                        commandValue = actionActivity.CommandValueOverride[command.OverrideId];
                                }
                            }
                            results = ADBSendKeys(commandValue);
                            break;
                        case ValidCommandIds.FindClick:
                            if (command.ImageName == null)
                            {
                                _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (!FindStrings.ContainsKey(command.ImageName))
                            {
                                _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                if (command.IgnoreMissing != null)
                                {
                                    ignoreMissing = (bool)command.IgnoreMissing;
                                }
                                results = FindClick(command.ImageName, FindStrings[command.ImageName], ignoreMissing);
                            }
                            break;
                        case ValidCommandIds.FindClickAndWait:
                            if ((command.ImageName == null) && (command.ImageNames == null))
                            {
                                _logger.LogError("Command {0} Error ImageName or ImageNames is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                imageNames = new List<string>();
                                if (command.ImageName != null)
                                {
                                    if (!FindStrings.ContainsKey(command.ImageName))
                                    {
                                        _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                        results = CommandResults.InputError;
                                        break;
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
                                            results = CommandResults.InputError;
                                            break;
                                        }
                                    }
                                    imageNames.AddRange(command.ImageNames);
                                }
                                if (results == CommandResults.Ok)
                                {
                                    results = FindClickAndWait(imageNames, ignoreMissing, (int)command.TimeOut);
                                }
                            }
                            break;
                        case ValidCommandIds.IfExists:
                            if (command.Commands == null)
                            {
                                _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.ImageName == null)
                            {
                                _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (!FindStrings.ContainsKey(command.ImageName))
                            {
                                _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = IfExists(command.ImageName, FindStrings[command.ImageName], command.Commands, additionalData, actionActivity);
                            }
                            break;
                        case ValidCommandIds.IfNotExists:
                            if (command.ImageName == null)
                            {
                                _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.Commands == null)
                            {
                                _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (!FindStrings.ContainsKey(command.ImageName))
                            {
                                _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = IfNotExists(command.ImageName, FindStrings[command.ImageName], command.Commands, additionalData, actionActivity);
                            }
                            break;
                        case ValidCommandIds.LoopCoordinates:
                            if (command.Coordinates == null)
                            {
                                _logger.LogError("Command {0} Error Coordinates is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.Commands == null)
                            {
                                _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = LoopCoordinates(command.Coordinates, command.Commands, actionActivity);
                            }
                            break;
                        case ValidCommandIds.LoopUntilFound:
                            if (command.IgnoreMissing == null)
                            {
                                ignoreMissing = false;
                            }
                            else
                            {
                                if ((bool)command.IgnoreMissing)
                                {
                                    ignoreMissing = true;
                                }
                                else
                                {
                                    ignoreMissing = false;
                                }
                            }
                            if ((command.ImageName == null) && (command.ImageNames == null))
                            {
                                _logger.LogError("Command {0} Error ImageName or ImageNames is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.Commands == null)
                            {
                                _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                imageNames = new List<string>();
                                if (command.ImageName != null)
                                {
                                    if (!FindStrings.ContainsKey(command.ImageName))
                                    {
                                        _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                        results = CommandResults.InputError;
                                        break;
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
                                            results = CommandResults.InputError;
                                            break;
                                        }
                                    }
                                    imageNames.AddRange(command.ImageNames);
                                }
                                if (results == CommandResults.Ok)
                                {
                                    results = LoopUntilFound(imageNames, ignoreMissing, command.Commands, (int)command.TimeOut, additionalData, actionActivity);
                                }
                            }
                            break;
                        case ValidCommandIds.LoopUntilNotFound:
                            if (command.IgnoreMissing == null)
                            {
                                ignoreMissing = false;
                            }
                            else
                            {
                                if ((bool)command.IgnoreMissing)
                                {
                                    ignoreMissing = true;
                                }
                                else
                                {
                                    ignoreMissing = false;
                                }
                            }
                            if ((command.ImageName == null) && (command.ImageNames == null))
                            {
                                _logger.LogError("Command {0} Error ImageName or ImageNames is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.Commands == null)
                            {
                                _logger.LogError("Command {0} Error Commands is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                imageNames = new List<string>();
                                if (command.ImageName != null)
                                {
                                    if (!FindStrings.ContainsKey(command.ImageName))
                                    {
                                        _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                        results = CommandResults.InputError;
                                        break;
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
                                            results = CommandResults.InputError;
                                            break;
                                        }
                                    }
                                    imageNames.AddRange(command.ImageNames);
                                }
                                if (results == CommandResults.Ok)
                                {
                                    results = LoopUntilNotFound(imageNames, ignoreMissing, command.Commands, (int)command.TimeOut, additionalData, actionActivity);
                                }
                            }
                            break;
                        case ValidCommandIds.Restart:
                            results = CommandResults.Restart;
                            break;
                        case ValidCommandIds.RunAction:
                            if (command.ActionName == null)
                            {
                                _logger.LogError("Command {0} Error ActionName is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = ExecuteAction(command.ActionName, actionActivity);
                            }
                            break;
                        case ValidCommandIds.Sleep:
                            if (command.Delay == null)
                            {
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                Thread.Sleep((int)command.Delay);
                            }
                            break;
                        case ValidCommandIds.StartGame:
                            if (command.Value == null)
                            {
                                _logger.LogError("Command {0} Error Value is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = ADBStartGame(command.Value, (int)command.TimeOut);
                            }
                            break;
                        case ValidCommandIds.StopGame:
                            if (command.Value == null)
                            {
                                _logger.LogError("Command {0} Error Value is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = ADBStopGame(command.Value, (int)command.TimeOut);
                            }
                            break;
                        case ValidCommandIds.WaitFor:
                            if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                if (command.IgnoreMissing != null)
                                    ignoreMissing = (bool)command.IgnoreMissing;
                                imageNames = new List<string>();
                                if (command.ImageName != null)
                                {
                                    if (!FindStrings.ContainsKey(command.ImageName))
                                    {
                                        _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                        results = CommandResults.InputError;
                                        break;
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
                                            results = CommandResults.InputError;
                                            break;
                                        }
                                    }
                                    imageNames.AddRange(command.ImageNames);
                                }
                                if (results == CommandResults.Ok)
                                {
                                    results = WaitFor(imageNames, ignoreMissing, (int)command.TimeOut);
                                }
                            }
                            break;
                        case ValidCommandIds.WaitForThenClick:
                            if (command.ImageName == null)
                            {
                                _logger.LogError("Command {0} Error ImageName is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (!FindStrings.ContainsKey(command.ImageName))
                            {
                                _logger.LogError("Command {0} Error ImageName {1} doesn't exist in FindStrings", command.CommandId, command.ImageName);
                                results = CommandResults.InputError;
                            }
                            else if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error Timeout is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = WaitForThenClick(command.ImageName, FindStrings[command.ImageName], (int)command.TimeOut);
                            }
                            break;
                        case ValidCommandIds.WaitForChange:
                            if (command.ChangeDetectArea == null)
                            {
                                _logger.LogError("Command {0} Error ChangeDetectArea is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.ChangeDetectDifference == null)
                            {
                                _logger.LogError("Command {0} Error ChangeDetectDifference is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error TimeOut is null", command.CommandId);
                                return CommandResults.InputError;
                            }
                            else
                            {
                                results = WaitForChange(command.ChangeDetectArea, (float)command.ChangeDetectDifference, (int)command.TimeOut);
                            }
                            break;
                        case ValidCommandIds.WaitForNoChange:
                            if (command.ChangeDetectArea == null)
                            {
                                _logger.LogError("Command {0} Error ChangeDetectArea is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.ChangeDetectDifference == null)
                            {
                                _logger.LogError("Command {0} Error ChangeDetectDifference is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else if (command.TimeOut == null)
                            {
                                _logger.LogError("Command {0} Error TimeOut is null", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                results = WaitForNoChange(command.ChangeDetectArea, (float)command.ChangeDetectDifference, (int)command.TimeOut);
                            }
                            break;
                        case ValidCommandIds.LoopCounter:
                            if (string.IsNullOrEmpty(command.Value))
                            {
                                _logger.LogError("Command {0} Error Value is null or empty", command.CommandId);
                                results = CommandResults.InputError;
                            }
                            else
                            {
                                int NumberOFLoops = 0;
                                if (!int.TryParse(command.Value, out NumberOFLoops))
                                {
                                    _logger.LogError("Command {0} Error Value {1} is not an integer", command.CommandId, command.Value);
                                    results = CommandResults.InputError;
                                }
                                if (actionActivity.CommandValueOverride != null && command.OverrideId != null)
                                {
                                    if (actionActivity.CommandValueOverride.ContainsKey(command.OverrideId))
                                    {
                                        if (actionActivity.CommandValueOverride[command.OverrideId] != null)
                                            if (!int.TryParse(actionActivity.CommandValueOverride[command.OverrideId], out NumberOFLoops))
                                            {
                                                _logger.LogError("CommandValueOverride {0} Value {1} is not an integer", command.OverrideId, actionActivity.CommandValueOverride[command.OverrideId]);
                                                results = CommandResults.InputError;
                                            }
                                    }
                                }
                                if (NumberOFLoops > 0)
                                {
                                    results = LoopCounter(NumberOFLoops, command.Commands, additionalData, actionActivity);
                                }
                            }
                            break;
                        default:
                            _logger.LogError("Valid but unhandled Command {0}", command.CommandId);
                            throw new Exception(string.Format("Valid but unhandled CommandId {0}", command.CommandId));
                    }
                }
                else
                {
                    _logger.LogError("Unrecognised Command {0}", command.CommandId);
                    throw new Exception(string.Format("Unrecognised CommandId {0}", command.CommandId));
                }
                activePath.Clear();
                activePath.Append(savePath);
                return results;
            }
        }
    }
}
