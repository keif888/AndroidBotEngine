// <copyright file="BotEngine.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

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

        public void SetThreadingCommand(string ActionName, BotEngineCallback callbackDelegate, ActionActivity actionActivity)
        {
            ThreadActionName = ActionName;
            ThreadActionActivity = actionActivity;
            callback = callbackDelegate;
        }

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
            isThreading = false;
            activePath = new StringBuilder();
        }

        public CommandResults ExecuteAction(string actionName, ActionActivity actionActivity)
        {
            using (_logger.BeginScope(string.Format("{0}:{1}({2})", EmulatorName,Helpers.CurrentMethodName(), actionName)))
            {
                _logger.LogInformation("Starting Action");
                string currentPath = string.Format("{0}/", actionName);
                string savePath = activePath.ToString();
                activePath.Append(currentPath);
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
                activePath.Clear();
                activePath.Append(savePath);
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
                return results;
            }
        }

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
                    adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                    using (Image localImage = adbFrameBuffer.ToImage())
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

                    List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X + searchString.SearchArea.Width, searchString.SearchArea.Y + searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, false, false, searchString.OffsetX, searchString.OffsetY);
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

        private CommandResults IfExists(string searchName, FindString findString, List<Command> Commands, Object? additionalData, ActionActivity actionActivity)
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

        private CommandResults IfNotExists(string searchName, FindString findString, List<Command> Commands, Object? additionalData, ActionActivity actionActivity)
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

                List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X + searchString.SearchArea.Width, searchString.SearchArea.Y + searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, false, false, searchString.OffsetX, searchString.OffsetY);
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
                bool stillThere = false;
                SearchResult foundAt = new SearchResult();
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

                    List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X+searchString.SearchArea.Width, searchString.SearchArea.Y+searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, false, false, searchString.OffsetX, searchString.OffsetY);
                    if (dataresult != null)
                    {
                        if (!found)
                        {
                            _logger.LogDebug("Initial Search Successful, found {0} whilst looking for {1}, Clicking", dataresult[0].Id, searchName);
                            result = CommandResults.TimeOut;
                            ADBClick(dataresult[0].X, dataresult[0].Y);
                            found = true;
                            foundAt = new SearchResult(dataresult[0].TopLeftX, dataresult[0].TopLeftY, dataresult[0].Width, dataresult[0].Height, dataresult[0].X, dataresult[0].Y, dataresult[0].Id);
                        }
                        else
                        {
                            stillThere = false;
                            foreach (SearchResult item in dataresult)
                            {
                                if ((item.X > foundAt.TopLeftX && item.X < foundAt.TopLeftX + foundAt.Width)
                                    && (item.Y > foundAt.TopLeftY && item.X < foundAt.TopLeftY + foundAt.Height))
                                {
                                    _logger.LogDebug("Secondary Search Successful (bad) whilst looking for {0}", searchName);
                                    stillThere = true;
                                    break;
                                }
                            }
                            if (!stillThere)
                            {
                                _logger.LogDebug("Secondary Search Unsuccessful (good) whilst looking for {0}", searchName);
                                result = CommandResults.Ok;
                                break;
                            }
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
        /// </summary>
        /// <param name="imageNames"></param>
        /// <param name="Commands"></param>
        /// <param name="timeOut"></param>
        /// <param name="additionalData"></param>
        /// <param name="actionActivity"></param>
        /// <returns></returns>
        private CommandResults LoopUntilNotFound(List<string> imageNames, bool ignoreMissing, List<Command> Commands, int timeOut, Object? additionalData, ActionActivity actionActivity)
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
                    searchString += findString.SearchString;
                    searchX = Math.Min(searchX, findString.SearchArea.X);
                    searchY = Math.Min(searchY, findString.SearchArea.Y);
                    searchW = Math.Max(searchW, findString.SearchArea.X+findString.SearchArea.Width);
                    searchH = Math.Max(searchH, findString.SearchArea.Y+findString.SearchArea.Height);
                    textFactor = Math.Max(textFactor, findString.TextTolerance);
                    backFactor = Math.Max(backFactor, findString.BackgroundTolerance);
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
                            result = ExecuteCommand(command, additionalData, actionActivity);
                            if (result != CommandResults.Ok)
                            {
                                return result;
                            }
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
        private CommandResults LoopUntilFound(List<string> imageNames, bool ignoreMissing, List<Command> Commands, int timeOut, Object? additionalData, ActionActivity actionActivity)
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
                    searchString += findString.SearchString;
                    searchX = Math.Min(searchX, findString.SearchArea.X);
                    searchY = Math.Min(searchY, findString.SearchArea.Y);
                    searchW = Math.Max(searchW, findString.SearchArea.X + findString.SearchArea.Width);
                    searchH = Math.Max(searchH, findString.SearchArea.Y + findString.SearchArea.Height);
                    textFactor = Math.Max(textFactor, findString.TextTolerance);
                    backFactor = Math.Max(backFactor, findString.BackgroundTolerance);
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
                            result = ExecuteCommand(command, additionalData, actionActivity);
                            if (result != CommandResults.Ok)
                            {
                                return result;
                            }
                        }
                    }
                    Thread.Sleep(50);
                } while ((dataresult == null) && (stopWatch.ElapsedMilliseconds < timeOut));
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
                    Rectangle srcRect = new Rectangle(changeDetectArea.X, changeDetectArea.Y, changeDetectArea.Width, changeDetectArea.Height);
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


                adbFrameBuffer.RefreshAsync(cancellationToken).Wait(3000);
                using (Bitmap savedImage = (Bitmap)adbFrameBuffer.ToImage())
                {
                    Rectangle srcRect = new Rectangle(changeDetectArea.X, changeDetectArea.Y, changeDetectArea.Width, changeDetectArea.Height);
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
                                ADBClick(command.Location.X, command.Location.Y);
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
                                ADBSwipe(command.Swipe.X1, command.Swipe.Y1, command.Swipe.X2, command.Swipe.Y2, (int)command.Delay);
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
                                ADBSendKeys(((XYCoords)additionalData).X.ToString());
                            }
                            else
                            {
                                ADBSendKeys(((XYCoords)additionalData).Y.ToString());
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
                            ADBSendKeys(commandValue);
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
                                results = FindClickAndWait(command.ImageName, FindStrings[command.ImageName], (int)command.TimeOut);
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
