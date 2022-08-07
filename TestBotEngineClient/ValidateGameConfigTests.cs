// <copyright file="ValidateGameConfigTests.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using BotEngineClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace TestBotEngineClient
{
    [TestClass]
    public class ValidateGameConfigTests
    {
        [TestMethod]
        public void TestValidateGameConfig_ParseValidData()
        {
            string fileName = @".\TestData\ValidGameConfig.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsTrue(jsonHelper.ValidateGameConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 0);
        }

        [TestMethod]
        public void TestValidateGameConfig_ParseInvalidFileIdType()
        {
            string fileName = @".\TestData\InvalidListConfig_FileIdWrongType.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateGameConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 4);
            Assert.IsTrue(jsonHelper.Errors.Contains(string.Format("\"FileId\" value is of incorrect type.  Expecting a String but got {0}", JsonValueKind.Number)));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"FindStrings\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"SystemActions\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"actions\" missing."));
        }

        [TestMethod]
        public void TestValidateGameConfig_WrongConfigFile()
        {
            string fileName = @".\TestData\ValidDevice.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateGameConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 4);
            Assert.IsTrue(jsonHelper.Errors.Contains("\"FileId\" indicates that this is not \"GameConfig\" but DeviceConfig"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"FindStrings\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"SystemActions\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"actions\" missing."));
        }

        [TestMethod]
        public void TestValidateGameConfig_FileIdNotValue()
        {
            string fileName = @".\TestData\InvalidListConfig_FileIdNotValue.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateGameConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 4);
            Assert.IsTrue(jsonHelper.Errors.Contains("Required field \"FileId\" is of the wrong type.  Expecting a String Value"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"FindStrings\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"SystemActions\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"actions\" missing."));
        }

        [TestMethod]
        public void TestValidateGameConfig_RootItemsNotObject()
        {
            string fileName = @".\TestData\InvalidGameConfig_RootItemsNotObjects.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateGameConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 3);
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"FindStrings\" is of the wrong type.  Expecting an Object with one or more named objects of FindStrings data"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"SystemActions\" is of the wrong type.  Expecting an Object with one or more named objects of Action data"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"actions\" is of the wrong type.  Expecting an Object with one or more named objects of Action data"));
        }

        [TestMethod]
        public void TestValidateGameConfig_LotsaErrors()
        {
            string fileName = @".\TestData\InvalidGameConfig_EverythingElse.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateGameConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.AreEqual<int>(58, jsonHelper.Errors.Count);
            
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadFindString\" at path $.FindStrings.BadFindString.SearchString is of the wrong type.  Was expecting String but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"MissingFindString\" at path $.FindStrings.MissingFindString is missing required field \"SearchString\"");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchArea\" at path $.FindStrings.BadSearchArea.searchArea is of the wrong type.  Was expecting JsonObject containing a search area but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"MissingSearchArea\" at path $.FindStrings.MissingSearchArea is missing required field \"searchArea\"");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadTextTolerance\" at path $.FindStrings.BadTextTolerance.textTolerance is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"MissingTextTolerance\" at path $.FindStrings.MissingTextTolerance is missing required field \"textTolerance\"");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadBackgroundTolerance\" at path $.FindStrings.BadBackgroundTolerance.backgroundTolerance is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"MissingBackgroundTolerance\" at path $.FindStrings.MissingBackgroundTolerance is missing required field \"backgroundTolerance\"");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchAreaXMissing\" at path $.FindStrings.BadSearchAreaXMissing.searchArea is missing required field \"X\"");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchAreaYMissing\" at path $.FindStrings.BadSearchAreaYMissing.searchArea is missing required field \"Y\"");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchAreaWMissing\" at path $.FindStrings.BadSearchAreaWMissing.searchArea is missing required field \"width\"");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchAreaHMissing\" at path $.FindStrings.BadSearchAreaHMissing.searchArea is missing required field \"height\"");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchAreaX\" at path $.FindStrings.BadSearchAreaX.searchArea.X is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchAreaY\" at path $.FindStrings.BadSearchAreaY.searchArea.Y is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchAreaW\" at path $.FindStrings.BadSearchAreaW.searchArea.width is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "FindStrings list item \"BadSearchAreaH\" at path $.FindStrings.BadSearchAreaH.searchArea.height is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonObject");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"MissingActionType\" at path $.SystemActions.MissingActionType is missing required field \"ActionType\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"MissingCommands\" at path $.SystemActions.MissingCommands is missing required field \"Commands\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"BadActiontype\" at path $.SystemActions.BadActiontype.ActionType is of the wrong type.  Was expecting String but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"BadCommands\" at path $.SystemActions.BadCommands.Commands is of the wrong type.  Was expecting Array but found System.Text.Json.Nodes.JsonObject");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"InvalidActiontype\" at path $.SystemActions.InvalidActiontype.ActionType with value \"Invalid\" is not valid.  Was expecting \"System\"");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"BadCommandInCommands\" at path $.SystemActions.BadCommandInCommands.Commands[0] is of the wrong type.  Was expecting Array but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"CommandIdMissing\" at path $.SystemActions.CommandIdMissing.Commands[0] is missing required field \"CommandId\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"CommandIdWrongType\" at path $.SystemActions.CommandIdWrongType.Commands[0].CommandId is of the wrong type.  Was expecting String but found Number");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"BadCommandId\" at path $.SystemActions.BadCommandId.Commands[0].CommandId with value \"IAmNotValid\" is not valid.  Was expecting a Command like one of the following \"WaitFor\", \"Click\", \"IfExists\", \"FindClickAndWait\"");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickXMissing\" at path $.SystemActions.ClickXMissing.Commands[0].Location is missing required field \"X\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickYMissing\" at path $.SystemActions.ClickYMissing.Commands[0].Location is missing required field \"Y\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickXWrongType\" at path $.SystemActions.ClickXWrongType.Commands[0].Location.X is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickYWrongType\" at path $.SystemActions.ClickYWrongType.Commands[0].Location.Y is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonArray");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaMissingAreas\" at path $.SystemActions.ClickWhenNotFoundInAreaMissingAreas.Commands[0] is missing required field \"Areas\"");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[0] is missing required field \"X\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[0] is missing required field \"Y\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[0] is missing required field \"width\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[0] is missing required field \"height\"");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[1].X is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[1].Y is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonObject");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[1].width is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[1].height is of the wrong type.  Was expecting Number but found True");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.SystemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[2] is of the wrong type.  Was expecting JsonObject with X/Y/width/height objects but found String");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragDelayMissing\" at path $.SystemActions.DragDelayMissing.Commands[0] is missing required field \"Delay\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragDelayWrongType\" at path $.SystemActions.DragDelayWrongType.Commands[0].Delay is of the wrong type.  Was expecting Number but found String");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeMissing\" at path $.SystemActions.DragSwipeMissing.Commands[0] is missing required field \"Swipe\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeWrongType\" at path $.SystemActions.DragSwipeWrongType.Commands[0].Swipe is of the wrong type.  Was expecting JsonObject with X1/Y1/X2/Y2 objects but found System.Text.Json.Nodes.JsonArray");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeValuesMissing\" at path $.SystemActions.DragSwipeValuesMissing.Commands[0].Swipe is missing required field \"X1\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeValuesMissing\" at path $.SystemActions.DragSwipeValuesMissing.Commands[0].Swipe is missing required field \"Y1\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeValuesMissing\" at path $.SystemActions.DragSwipeValuesMissing.Commands[0].Swipe is missing required field \"X2\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeValuesMissing\" at path $.SystemActions.DragSwipeValuesMissing.Commands[0].Swipe is missing required field \"Y2\"");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeValuesWrongType\" at path $.SystemActions.DragSwipeValuesWrongType.Commands[0].Swipe.X1 is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeValuesWrongType\" at path $.SystemActions.DragSwipeValuesWrongType.Commands[0].Swipe.Y1 is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeValuesWrongType\" at path $.SystemActions.DragSwipeValuesWrongType.Commands[0].Swipe.X2 is of the wrong type.  Was expecting Number but found True");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"DragSwipeValuesWrongType\" at path $.SystemActions.DragSwipeValuesWrongType.Commands[0].Swipe.Y2 is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonObject");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"EnterLoopCoordinatesMissing\" at path $.SystemActions.EnterLoopCoordinatesMissing.Commands[0] is missing required field \"Value\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"EnterLoopCoordinatesBadType\" at path $.SystemActions.EnterLoopCoordinatesBadType.Commands[0].Value is of the wrong type.  Was expecting String but found Number");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"IfExistsImageNameMissing\" at path $.SystemActions.IfExistsImageNameMissing.Commands[0] is missing required field \"ImageName\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"IfExistsImageNameWrongType\" at path $.SystemActions.IfExistsImageNameWrongType.Commands[0].ImageName is of the wrong type.  Was expecting String but found False");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"IfNotExistsCommandsMissing\" at path $.SystemActions.IfNotExistsCommandsMissing.Commands[0] is missing required field \"Commands\"");
            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"IfExistsCommandsWrongType\" at path $.SystemActions.IfExistsCommandsWrongType.Commands[0].Commands is of the wrong type.  Was expecting JsonArray with one or more Command objects but found String");

            CollectionAssert.Contains(jsonHelper.Errors, "SystemActions list item \"EnterTextWrongType\" at path $.SystemActions.EnterTextWrongType.Commands[0].Value is of the wrong type.  Was expecting String but found Number");

        }

    }
}
