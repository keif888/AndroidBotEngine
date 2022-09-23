using Microsoft.VisualStudio.TestTools.UnitTesting;
using BotEngineClient;
// <copyright file="ActionTests.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace BotEngineClient.Tests
{
    public class stubAction : Action
    {
        private readonly DateTime _dateTime;

        public stubAction(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        protected override DateTime DateTimeNow()
        {
            return _dateTime;
        }
    }


    public class stubActionActivity : ActionActivity
    {
        private readonly DateTime _dateTime;

        public stubActionActivity(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        protected override DateTime DateTimeNow()
        {
            return _dateTime;
        }
    }


    [TestClass()]
    public class ActionTests
    {

        

        [TestMethod()]
        public void ExecuteDueTest_DailyOverriden()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 05, 01);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Daily.ToString();
            action.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 10, 00);

            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 22, 10, 10, 30);
            actionActivity.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 05, 00);
            actionActivity.Frequency = null;
            if (!action.ExecuteDue(actionActivity))
                Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteDueTest_DailyOverriden_Negative()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 25, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Daily.ToString();
            action.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 10, 00);

            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 05, 00);
            actionActivity.LastRun = new DateTime(2022, 09, 23, 10, 10, 10);
            actionActivity.Frequency = null;
            if (action.ExecuteDue(actionActivity))
                Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteDueTest_DailyStandard()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 10, 01);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Daily.ToString();
            action.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 10, 00);

            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 22, 10, 10, 30);
            actionActivity.Frequency = null;
            if (!action.ExecuteDue(actionActivity))
                Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteDueTest_DailyStandard_Negative()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 15, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Daily.ToString();
            action.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 10, 00);

            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 10, 10, 10);
            actionActivity.Frequency = null;
            if (action.ExecuteDue(actionActivity))
                Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteDueTest_ScheduledOverridden()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 05, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Scheduled.ToString();
            action.Frequency = 60;

            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 09, 30, 00);
            actionActivity.Frequency = 30;
            if (!action.ExecuteDue(actionActivity))
                Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteDueTest_ScheduledOverriden_Negative()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 09, 35, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Scheduled.ToString();
            action.Frequency = 60;

            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 09, 30, 00);
            actionActivity.Frequency = 30;
            if (action.ExecuteDue(actionActivity))
                Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteDueTest_ScheduledStandard()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 05, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Scheduled.ToString();
            action.Frequency = 60;

            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 08, 05, 00);
            actionActivity.Frequency = null;
            if (!action.ExecuteDue(actionActivity))
                Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteDueTest_ScheduledStandard_Negative()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 09, 04, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Scheduled.ToString();
            action.Frequency = 60;

            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 08, 05, 00);
            actionActivity.Frequency = null;
            if (action.ExecuteDue(actionActivity))
                Assert.Fail();
        }



        [TestMethod()]
        public void NextExecuteDueTest_DailyStandard()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 15, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Daily.ToString();
            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 10, 10, 05);
            action.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 10, 00);
            DateTime actual = action.NextExecuteDue(actionActivity);
            DateTime expected = new DateTime(2022, 09, 24, 10, 10, 00);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NextExecuteDueTest_DailyOverriden()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 45, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Daily.ToString();
            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 10, 10, 05);
            action.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 10, 00);
            actionActivity.DailyScheduledTime = new DateTime(1900, 01, 01, 10, 30, 00);
            DateTime actual = action.NextExecuteDue(actionActivity);
            DateTime expected = new DateTime(2022, 09, 23, 10, 30, 00);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NextExecuteDueTest_ScheduledStandard()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 15, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Scheduled.ToString();
            action.Frequency = 60;
            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 10, 10, 05);
            DateTime actual = action.NextExecuteDue(actionActivity);
            DateTime expected = new DateTime(2022, 09, 23, 11, 10, 05);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NextExecuteDueTest_ScheduledOverriden()
        {
            DateTime dateTime = new DateTime(2022, 09, 23, 10, 15, 00);
            stubAction action = new stubAction(dateTime);
            action.ActionType = ValidActionType.Scheduled.ToString();
            action.Frequency = 60;
            stubActionActivity actionActivity = new stubActionActivity(dateTime);
            actionActivity.ActionEnabled = true;
            actionActivity.Frequency = 30;
            actionActivity.LastRun = new DateTime(2022, 09, 23, 10, 10, 05);
            DateTime actual = action.NextExecuteDue(actionActivity);
            DateTime expected = new DateTime(2022, 09, 23, 10, 40, 05);
            Assert.AreEqual(expected, actual);
        }
    
    }
}