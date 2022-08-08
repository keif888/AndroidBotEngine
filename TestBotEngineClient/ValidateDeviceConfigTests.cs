// <copyright file="ValidateDeviceConfigTests.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using BotEngineClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace TestBotEngineClient
{
    [TestClass]
    public class ValidateDeviceConfigTests
    {
        [TestMethod]
        public void TestValidateDeviceConfig_ParseValidData()
        {
            string fileName = @".\TestData\ValidDevice.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsTrue(jsonHelper.ValidateDeviceConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.AreEqual<int>(0, jsonHelper.Errors.Count);
        }


        [TestMethod]
        public void TestValidateDeviceConfig_WrongConfigFile()
        {
            string fileName = @".\TestData\ValidGameConfig.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateDeviceConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.AreEqual<int>(2, jsonHelper.Errors.Count);
            CollectionAssert.Contains(jsonHelper.Errors, "\"FileId\" indicates that this is not \"DeviceConfig\" but GameConfig");
            CollectionAssert.Contains(jsonHelper.Errors, "Required field \"LastActionTaken\" missing.");
        }


        [TestMethod]
        public void TestValidateDeviceConfig_InvalidConfigFile()
        {
            string fileName = @".\TestData\InvalidDeviceConfig_VariousErrors.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateDeviceConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.AreEqual<int>(3, jsonHelper.Errors.Count);
            CollectionAssert.Contains(jsonHelper.Errors, "LastActionTaken list item \"TransferBread\" at path $.LastActionTaken.TransferBread.DailyScheduledTime is of the wrong type.  Was expecting String but found Number");
            CollectionAssert.Contains(jsonHelper.Errors, "LastActionTaken list item \"TransferBread\" at path $.LastActionTaken.TransferBread.CommandValueOverride.CoordX is of the wrong type.  Was expecting String but found Number");
            CollectionAssert.Contains(jsonHelper.Errors, "LastActionTaken list item \"TransferBread\" at path $.LastActionTaken.TransferBread.CommandLoopStatus.TransferBread is of the wrong type.  Was expecting String but found Number");
        }
    }
}
