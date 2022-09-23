// <copyright file="BOTConfig.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BotEngineClient
{
    /// <summary>
    /// Class to hold the config identifier, and then all the others as optional.
    /// </summary>
    public class BOTConfigIdentifier
    {
        public string FileId { get; set; }
    }



    /// <summary>
    /// Class for json Game Config.
    /// Stores dictionaries of:
    /// The strings that identify items to look for
    /// The system actions that can be called from other actions
    /// The bot actions that can be scheduled to play the bot
    /// </summary>
    public class BOTConfig : BOTConfigIdentifier
    {
        public Dictionary<string, FindString> FindStrings { get; set; }
        public Dictionary<string, Action> SystemActions { get; set; }
        public Dictionary<string, Action> Actions { get; set; }

        /// <summary>
        /// Generic constructor for the BOTConfig class
        /// </summary>
        public BOTConfig()
        {
            FindStrings = new Dictionary<string, FindString>();
            SystemActions = new Dictionary<string, Action>();
            Actions = new Dictionary<string, Action>();
        }
    }

    /// <summary>
    /// The list of valid values for ActionType.
    /// </summary>
    public enum ValidActionType
    {
        Adhoc,
        Always,
        Daily,
        Scheduled,
        System
    }

    public class Action
    {
        /// <summary>
        /// Used to store the number of minutes between each execution when "ActionType = Scheduled"
        /// </summary>
        public int? Frequency { get; set; }
        /// <summary>
        /// Used to store what time of day a "ActionType = Daily" runs once after.
        /// </summary>
        public DateTime? DailyScheduledTime { get; set; }
        /// <summary>
        /// Used to determine what type of Action this is.
        /// Scheduled = Runs every Frequency Minutes
        /// Daily = Runs once after DailyScheduledTime
        /// Always = Means that it runs every loop.
        /// Adhoc = Runs ONCE, when set to Enabled in the Device Config
        /// System = Indicates that it is a System action, and is not scheduled, but called from other Actions.
        /// </summary>
        public string ActionType { get; set; }
        public List<Command> Commands { get; set; }
        public string? BeforeAction { get; set; }
        public string? AfterAction { get; set; }

        /// <summary>
        /// Virtual method to return the value of DateTime.Now, so that it can be tested in unit tests
        /// By Inheriting from this class, and overriding this method
        /// </summary>
        /// <returns>A DateTime value that contains the current date and time</returns>
        protected virtual DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Determines if the passed in actionActivity is due to be run.
        /// Does not take into account if the actionActivity is disabled.
        /// </summary>
        /// <param name="actionActivity">The details about when this last ran, and any overrides in place</param>
        /// <returns>true when the Action is available to be executed</returns>
        public bool ExecuteDue(ActionActivity actionActivity)
        {
            int frequency = 0;
            if (actionActivity.Frequency != null)
            {
                frequency = (int)actionActivity.Frequency;
            }
            else if (Frequency != null)
            {
                frequency = (int)Frequency;
            }

            DateTime dailyScheduledTime = DateTime.MaxValue;
            if (actionActivity.DailyScheduledTime != null)
            {
                dailyScheduledTime = (DateTime)actionActivity.DailyScheduledTime;
            }
            else if (DailyScheduledTime != null)
            {
                dailyScheduledTime = (DateTime)DailyScheduledTime;
            }
            switch (ActionType.ToLower())
            {
                case "scheduled":
                    if (Frequency != null)
                    {
                        if ((DateTimeNow() - actionActivity.LastRun).TotalMinutes > frequency)
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                case "system":
                    return false;
                case "daily":
                    if (DailyScheduledTime != null)
                    {
                        DateTime nextScheduledDateTime = DateTimeNow().Date + dailyScheduledTime.TimeOfDay;
                        if ((nextScheduledDateTime > actionActivity.LastRun && nextScheduledDateTime < DateTimeNow()) // Last time it ran was before the scheduled time, and it's after the scheduled to run time.
                            || ((nextScheduledDateTime - actionActivity.LastRun).TotalDays > 1))  // 
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                case "adhoc":
                case "anywhere":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns when this action is due next to be executed
        /// </summary>
        /// <param name="actionActivity">The details about when this last ran, and any overrides in place</param>
        /// <returns>The date time when this Action is due next</returns>
        public DateTime NextExecuteDue(ActionActivity actionActivity)
        {
            int frequency = 0;
            if (actionActivity.Frequency != null)
            {
                frequency = (int)actionActivity.Frequency;
            }
            else if (Frequency != null)
            {
                frequency = (int)Frequency;
            }

            DateTime dailyScheduledTime = DateTime.MaxValue;
            if (actionActivity.DailyScheduledTime != null)
            {
                dailyScheduledTime = (DateTime)actionActivity.DailyScheduledTime;
            }
            else if (DailyScheduledTime != null)
            {
                dailyScheduledTime = (DateTime)DailyScheduledTime;
            }

            switch (ActionType.ToLower())
            {
                case "scheduled":
                    if (Frequency != null)
                    {
                        return actionActivity.LastRun + TimeSpan.FromMinutes((int)frequency);
                    }
                    else
                        return actionActivity.LastRun;
                case "system":
                    return DateTime.MaxValue;
                case "daily":
                    if (DailyScheduledTime != null)
                    {
                        if (ExecuteDue(actionActivity))
                            return DateTimeNow().Date + dailyScheduledTime.TimeOfDay;
                        else
                            return DateTimeNow().Date + dailyScheduledTime.TimeOfDay + TimeSpan.FromDays(1);
                    }
                    else
                        return DateTime.MaxValue;
                case "anywhere":
                    return DateTimeNow();
                default:
                    return DateTime.MaxValue;
            }
            }


        public Action()
        {
            Commands = new List<Command>();
        }
    }

    public class FindString
    {
        private int? offsetY1;
        private int? offsetX1;

        public string SearchString { get; set; }
        public SearchArea SearchArea { get; set; }
        public float TextTolerance { get; set; }
        public float BackgroundTolerance { get; set; }
        public int? OffsetX { get => offsetX1 == null ? 20 : offsetX1; set => offsetX1 = value; }
        public int? OffsetY { get => offsetY1 == null ? 10 : offsetY1; set => offsetY1 = value; }
        public FindString()
        {
            this.SearchString = string.Empty;
            this.SearchArea = new SearchArea();
            this.TextTolerance = 0.2f;
            this.BackgroundTolerance = 0.2f;
        }

        public FindString(string findString, SearchArea searchArea, float textTolerance, float backgroundTolerance)
        {
            this.SearchString = findString ?? throw new ArgumentNullException(nameof(findString));
            this.SearchArea = searchArea ?? throw new ArgumentNullException(nameof(searchArea));
            this.TextTolerance = textTolerance;
            this.BackgroundTolerance = backgroundTolerance;
        }
    }

    public class Command
    {
        [JsonConstructorAttribute]
        public Command()
        {

        }
        public Command(string commandId)
        {
            CommandId = commandId;
        }

        public Command(BotEngine.ValidCommandIds selectedCommand)
        {
            CommandId = selectedCommand.ToString();
            switch (selectedCommand)
            {
                case BotEngine.ValidCommandIds.Click:
                    Location = new XYCoords(0, 0);
                    break;
                case BotEngine.ValidCommandIds.ClickWhenNotFoundInArea:
                    ImageName = String.Empty;
                    Areas = new List<SearchArea>();
                    break;
                case BotEngine.ValidCommandIds.Drag:
                    Delay = 150;
                    Swipe = new SwipeCoords();
                    break;
                case BotEngine.ValidCommandIds.Exit:
                    break;
                case BotEngine.ValidCommandIds.EnterLoopCoordinate:
                    Value = string.Empty;
                    break;
                case BotEngine.ValidCommandIds.EnterText:
                    Value = String.Empty;
                    break;
                case BotEngine.ValidCommandIds.FindClick:
                    ImageName = String.Empty;
                    IgnoreMissing = false;
                    break;
                case BotEngine.ValidCommandIds.FindClickAndWait:
                    ImageName = String.Empty;
                    TimeOut = 150;
                    break;
                case BotEngine.ValidCommandIds.IfExists:
                case BotEngine.ValidCommandIds.IfNotExists:
                    ImageName = String.Empty;
                    Commands = new List<Command>();
                    break;
                case BotEngine.ValidCommandIds.LoopCoordinates:
                    Coordinates = string.Empty;
                    Commands = new List<Command>();
                    break;
                case BotEngine.ValidCommandIds.LoopUntilFound:
                case BotEngine.ValidCommandIds.LoopUntilNotFound:
                    ImageName = String.Empty;
                    Commands = new List<Command>();
                    break;
                case BotEngine.ValidCommandIds.Restart:
                    break;
                case BotEngine.ValidCommandIds.RunAction:
                    ActionName = string.Empty;
                    break;
                case BotEngine.ValidCommandIds.Sleep:
                    Delay = 150;
                    break;
                case BotEngine.ValidCommandIds.StartGame:
                case BotEngine.ValidCommandIds.StopGame:
                    TimeOut = 150;
                    Value = string.Empty;
                    break;
                case BotEngine.ValidCommandIds.WaitFor:
                case BotEngine.ValidCommandIds.WaitForThenClick:
                    ImageName = string.Empty;
                    TimeOut = 150;
                    break;
                case BotEngine.ValidCommandIds.WaitForChange:
                case BotEngine.ValidCommandIds.WaitForNoChange:
                    TimeOut = 150;
                    ChangeDetectDifference = 0;
                    ChangeDetectArea = new SearchArea();
                    break;
                case BotEngine.ValidCommandIds.LoopCounter:
                    Value = string.Empty;
                    OverrideId = string.Empty;
                    break;
                default:
                    break;
            }
        }

        public string CommandId { get; set; }
        public int? CommandNumber { get; set; }
        public string? Coordinates { get; set; }
        public List<Command>? Commands { get; set; }
        public XYCoords? Location { get; set; }
        public SwipeCoords? Swipe { get; set; }
        public string? Value { get; set; }
        public int? Delay { get; set; }
        public string? ImageName { get; set; }
        public List<string>? ImageNames { get; set; }
        public int? TimeOut { get; set; }
        public string? ActionName { get; set; }
        public List<SearchArea>? Areas { get; set; }
        public bool? IgnoreMissing { get; set; }
        public SearchArea? ChangeDetectArea { get; set; }
        public float? ChangeDetectDifference { get; set; }

        /// <summary>
        /// Provides the Identifier that is used when applying an override to a value
        /// </summary>
        public string? OverrideId { get; set; }

        public Command ShallowCopy()
        {
            return (Command)this.MemberwiseClone();
        }
        public Command DeepCopy()
        {
            Command clone = (Command)this.MemberwiseClone();
            if (this.Commands != null)
                clone.Commands = new List<Command>(this.Commands);
            return clone;
        }
    }


    public class SearchArea
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return string.Format("({0},{1}) - ({2},{3})", X, Y, X + Width, Y + Height);
        }
    }


    public class XYCoords
    {
        public int X { get; set; }
        public int Y { get; set; }

        public XYCoords(int x, int y)
        {
            X = x;
            Y = y;
        }
        public XYCoords()
        {
            X = 0;
            Y = 0;
        }
    }

    public class SwipeCoords
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
    }

    public class BOTListConfig : BOTConfigIdentifier
    {
        public Dictionary<string, List<XYCoords>>? Coordinates { get; set; }
    }



    public class BOTDeviceConfig : BOTConfigIdentifier
    {
        public Dictionary<string, ActionActivity> LastActionTaken { get; set; }
    }

    public class ActionActivity
    {
        public DateTime LastRun { get; set; }
        public bool ActionEnabled { get; set; }
        /// <summary>
        /// Used to store the number of minutes between each execution when "ActionType = Scheduled"
        /// </summary>
        public int? Frequency { get; set; }
        /// <summary>
        /// Used to store what time of day a "ActionType = Daily" runs once after.
        /// </summary>
        public DateTime? DailyScheduledTime { get; set; }

        /// <summary>
        /// Used to store where a loop got up to, the last time it was executing.
        /// This is used to restart where the loop was at, rather than starting from the beginning.
        /// Very useful for LoopCounter, where an unexpected condition causes it to drop out.
        /// 
        /// The Key is the path to the loop.
        /// The Value is the iteration that the loop was up to.
        /// </summary>
        public Dictionary<string,string>? CommandLoopStatus { get; set; }

        /// <summary>
        /// Used to store a value that can override the "Value" field within a Command.
        /// 
        /// The Key is the path to the Value field that is being overridden.  This is useful for LoopCounter.
        /// eg.
        /// ActionName/IfExists???  ToDo: Work out the path.
        /// 
        /// The Value is the value that will be used to replace the Value field.
        /// </summary>
        public Dictionary<string,string?>? CommandValueOverride { get; set; }


        /// <summary>
        /// Virtual method to return the value of DateTime.Now, so that it can be tested in unit tests
        /// By Inheriting from this class, and overriding this method
        /// </summary>
        /// <returns>A DateTime value that contains the current date and time</returns>
        protected virtual DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        public void MarkStartupBot(int Frequency)
        {
            LastRun = DateTimeNow() - TimeSpan.FromMinutes(Frequency);
        }

        /// <summary>
        /// Used to indicate that this action has been executed successfully, and to know when that was.
        /// </summary>
        public void MarkExecuted()
        {
            LastRun = DateTimeNow();
        }

        public void SetLastExecuted(DateTime lastRun)
        {
            LastRun = lastRun;
        }

        /// <summary>
        /// Used when an Adhoc task has been run successfully.
        /// </summary>
        public void MarkDisabled()
        {
            ActionEnabled = false;
        }
    }


}
