// <copyright file="ValidateListConfigTests.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using BotEngineClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace TestBotEngineClient
{
    [TestClass]
    public class ValidateListConfigTests
    {
        [TestMethod]
        public void TestValidateListConfig_ParseValidData()
        {
            string fileName = @".\TestData\ValidListConfig.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsTrue(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 0);
        }

        [TestMethod]
        public void TestValidateListConfig_ParseInvalidFileIdType()
        {
            string fileName = @".\TestData\InvalidListConfig_FileIdWrongType.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            CollectionAssert.Contains(jsonHelper.Errors, string.Format("\"FileId\" value is of incorrect type.  Expecting a String but got {0}", JsonValueKind.Number));
        }

        [TestMethod]
        public void TestValidateListConfig_WrongConfigFile()
        {
            string fileName = @".\TestData\ValidDevice.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 2);
            CollectionAssert.Contains(jsonHelper.Errors, "\"FileId\" indicates that this is not \"ListConfig\" but DeviceConfig");
            CollectionAssert.Contains(jsonHelper.Errors, "Required field \"Coordinates\" missing.");
        }

        [TestMethod]
        public void TestValidateListConfig_FileIdNotValue()
        {
            string fileName = @".\TestData\InvalidListConfig_FileIdNotValue.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            CollectionAssert.Contains(jsonHelper.Errors, "Required field \"FileId\" is of the wrong type.  Expecting a String Value");
        }

        [TestMethod]
        public void TestValidateListConfig_CoordinatesNotObject()
        {
            string fileName = @".\TestData\InvalidListConfig_CoordinatesNotObject.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            CollectionAssert.Contains(jsonHelper.Errors, "Required field \"Coordinates\" is of the wrong type.  Expecting an Object with one or more named arrays of X/Y value pairs");
        }

        [TestMethod]
        public void TestValidateListConfig_CoordinatesItemNotArray()
        {
            string fileName = @".\TestData\InvalidListConfig_CoordinateItemNotArray.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates item BouncingBalls at path $.Coordinates.BouncingBalls is of the wrong type.  Was expecting Array, but found String");
        }

        [TestMethod]
        public void TestValidateListConfig_CoordinatesListItemNotObject()
        {
            string fileName = @".\TestData\InvalidListConfig_CoordinateListItemNotObject.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 4);
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"X\" at path $.Coordinates.BouncingBalls[0] is of the wrong type.  Was expecting Object, but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item 10 at path $.Coordinates.BouncingBalls[1] is of the wrong type.  Was expecting Object, but found Number");
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"Y\" at path $.Coordinates.BouncingBalls[2] is of the wrong type.  Was expecting Object, but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item 10 at path $.Coordinates.BouncingBalls[3] is of the wrong type.  Was expecting Object, but found Number");
        }

        [TestMethod]
        public void TestValidateListConfig_OneMissingX()
        {
            string fileName = @".\TestData\InvalidListConfig_MissingOneX.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"StaticBalls\" at path $.Coordinates.StaticBalls[0] is missing required field \"X\"");
        }

        [TestMethod]
        public void TestValidateListConfig_XWrongType()
        {
            string fileName = @".\TestData\InvalidListConfig_XWrongType.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"BouncingBalls\" at path $.Coordinates.BouncingBalls[0].X is of the wrong type.  Was expecting Number but found String");
        }

        [TestMethod]
        public void TestValidateListConfig_OneMissingY()
        {
            string fileName = @".\TestData\InvalidListConfig_MissingOneY.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"BouncingBalls\" at path $.Coordinates.BouncingBalls[1] is missing required field \"Y\"");
        }

        [TestMethod]
        public void TestValidateListConfig_YWrongType()
        {
            string fileName = @".\TestData\InvalidListConfig_YWrongType.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"StaticBalls\" at path $.Coordinates.StaticBalls[1].Y is of the wrong type.  Was expecting Number but found String");
        }

        [TestMethod]
        public void TestValidateListConfig_MultipleErrors()
        {
            string fileName = @".\TestData\InvalidListConfig_MissingMultipleXandY.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 3);
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"BouncingBalls\" at path $.Coordinates.BouncingBalls[0] is missing required field \"Y\"");
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"StaticBalls\" at path $.Coordinates.StaticBalls[0] is missing required field \"X\"");
            CollectionAssert.Contains(jsonHelper.Errors, "Coordinates list item \"StaticBalls\" at path $.Coordinates.StaticBalls[1] is missing required field \"X\"");
        }
    }
}
