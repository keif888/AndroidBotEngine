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
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"findStrings\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"systemActions\" missing."));
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
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"findStrings\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"systemActions\" missing."));
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
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"findStrings\" missing."));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"systemActions\" missing."));
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
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"findStrings\" is of the wrong type.  Expecting an Object with one or more named objects of FindStrings data"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"systemActions\" is of the wrong type.  Expecting an Object with one or more named objects of Action data"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required root field \"actions\" is of the wrong type.  Expecting an Object with one or more named objects of Action data"));
        }

        [TestMethod]
        public void TestValidateGameConfig_LotsaErrors()
        {
            string fileName = @".\TestData\InvalidGameConfig_EverythingElse.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateGameConfigStructure(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            // ToDo: Reinstate this when all the tests are completed.
            Assert.AreEqual<int>(57, jsonHelper.Errors.Count);
            
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadFindString\" at path $.findStrings.BadFindString.findString is of the wrong type.  Was expecting String but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"MissingFindString\" at path $.findStrings.MissingFindString is missing required field \"findString\"");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchArea\" at path $.findStrings.BadSearchArea.searchArea is of the wrong type.  Was expecting Object containing a search area");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"MissingSearchArea\" at path $.findStrings.MissingSearchArea is missing required field \"searchArea\"");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadTextTolerance\" at path $.findStrings.BadTextTolerance.textTolerance is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"MissingTextTolerance\" at path $.findStrings.MissingTextTolerance is missing required field \"textTolerance\"");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadBackgroundTolerance\" at path $.findStrings.BadBackgroundTolerance.backgroundTolerance is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"MissingBackgroundTolerance\" at path $.findStrings.MissingBackgroundTolerance is missing required field \"backgroundTolerance\"");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchAreaXMissing\" at path $.findStrings.BadSearchAreaXMissing.searchArea is missing required field \"X\"");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchAreaYMissing\" at path $.findStrings.BadSearchAreaYMissing.searchArea is missing required field \"Y\"");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchAreaWMissing\" at path $.findStrings.BadSearchAreaWMissing.searchArea is missing required field \"width\"");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchAreaHMissing\" at path $.findStrings.BadSearchAreaHMissing.searchArea is missing required field \"height\"");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchAreaX\" at path $.findStrings.BadSearchAreaX.searchArea.X is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchAreaY\" at path $.findStrings.BadSearchAreaY.searchArea.Y is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchAreaW\" at path $.findStrings.BadSearchAreaW.searchArea.width is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "findStrings list item \"BadSearchAreaH\" at path $.findStrings.BadSearchAreaH.searchArea.height is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonObject");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"MissingActionType\" at path $.systemActions.MissingActionType is missing required field \"ActionType\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"MissingCommands\" at path $.systemActions.MissingCommands is missing required field \"Commands\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"BadActiontype\" at path $.systemActions.BadActiontype.ActionType is of the wrong type.  Was expecting String but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"BadCommands\" at path $.systemActions.BadCommands.Commands is of the wrong type.  Was expecting Array but found System.Text.Json.Nodes.JsonObject");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"InvalidActiontype\" at path $.systemActions.InvalidActiontype.ActionType with value \"Invalid\" is not valid.  Was expecting \"System\"");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"BadCommandInCommands\" at path $.systemActions.BadCommandInCommands.Commands[0] is of the wrong type.  Was expecting Array but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"CommandIdMissing\" at path $.systemActions.CommandIdMissing.Commands[0] is missing required field \"CommandId\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"CommandIdWrongType\" at path $.systemActions.CommandIdWrongType.Commands[0].CommandId is of the wrong type.  Was expecting String but found Number");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"BadCommandId\" at path $.systemActions.BadCommandId.Commands[0].CommandId with value \"IAmNotValid\" is not valid.  Was expecting a Command like one of the following \"WaitFor\", \"Click\", \"IfExists\", \"FindClickAndWait\"");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickXMissing\" at path $.systemActions.ClickXMissing.Commands[0].Location is missing required field \"X\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickYMissing\" at path $.systemActions.ClickYMissing.Commands[0].Location is missing required field \"Y\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickXWrongType\" at path $.systemActions.ClickXWrongType.Commands[0].Location.X is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickYWrongType\" at path $.systemActions.ClickYWrongType.Commands[0].Location.Y is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonArray");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaMissingAreas\" at path $.systemActions.ClickWhenNotFoundInAreaMissingAreas.Commands[0] is missing required field \"Areas\"");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[0] is missing required field \"X\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[0] is missing required field \"Y\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[0] is missing required field \"width\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[0] is missing required field \"height\"");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[1].X is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[1].Y is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonObject");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[1].width is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[1].height is of the wrong type.  Was expecting Number but found True");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"ClickWhenNotFoundInAreaErrors\" at path $.systemActions.ClickWhenNotFoundInAreaErrors.Commands[0].Areas[2] is of the wrong type.  Was expecting JsonObject with X/Y/width/height objects but found String");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragDelayMissing\" at path $.systemActions.DragDelayMissing.Commands[0] is missing required field \"Delay\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragDelayWrongType\" at path $.systemActions.DragDelayWrongType.Commands[0].Delay is of the wrong type.  Was expecting Number but found String");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeMissing\" at path $.systemActions.DragSwipeMissing.Commands[0] is missing required field \"Swipe\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeWrongType\" at path $.systemActions.DragSwipeWrongType.Commands[0].Swipe is of the wrong type.  Was expecting JsonObject with X1/Y1/X2/Y2 objects but found System.Text.Json.Nodes.JsonArray");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeValuesMissing\" at path $.systemActions.DragSwipeValuesMissing.Commands[0].Swipe is missing required field \"X1\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeValuesMissing\" at path $.systemActions.DragSwipeValuesMissing.Commands[0].Swipe is missing required field \"Y1\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeValuesMissing\" at path $.systemActions.DragSwipeValuesMissing.Commands[0].Swipe is missing required field \"X2\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeValuesMissing\" at path $.systemActions.DragSwipeValuesMissing.Commands[0].Swipe is missing required field \"Y2\"");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeValuesWrongType\" at path $.systemActions.DragSwipeValuesWrongType.Commands[0].Swipe.X1 is of the wrong type.  Was expecting Number but found String");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeValuesWrongType\" at path $.systemActions.DragSwipeValuesWrongType.Commands[0].Swipe.Y1 is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonArray");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeValuesWrongType\" at path $.systemActions.DragSwipeValuesWrongType.Commands[0].Swipe.X2 is of the wrong type.  Was expecting Number but found True");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"DragSwipeValuesWrongType\" at path $.systemActions.DragSwipeValuesWrongType.Commands[0].Swipe.Y2 is of the wrong type.  Was expecting Number but found System.Text.Json.Nodes.JsonObject");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"EnterLoopCoordinatesMissing\" at path $.systemActions.EnterLoopCoordinatesMissing.Commands[0] is missing required field \"Value\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"EnterLoopCoordinatesBadType\" at path $.systemActions.EnterLoopCoordinatesBadType.Commands[0].Value is of the wrong type.  Was expecting String but found Number");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"IfExistsImageNameMissing\" at path $.systemActions.IfExistsImageNameMissing.Commands[0] is missing required field \"ImageName\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"IfExistsImageNameWrongType\" at path $.systemActions.IfExistsImageNameWrongType.Commands[0].ImageName is of the wrong type.  Was expecting String but found False");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"IfNotExistsCommandsMissing\" at path $.systemActions.IfNotExistsCommandsMissing.Commands[0] is missing required field \"Commands\"");
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"IfExistsCommandsWrongType\" at path $.systemActions.IfExistsCommandsWrongType.Commands[0].Commands is of the wrong type.  Was expecting JsonArray with one or more Command objects but found String");

        }

    }
}
