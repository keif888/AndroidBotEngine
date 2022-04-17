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
            Assert.AreEqual<int>(22, jsonHelper.Errors.Count);
            
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
            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"InvalidActiontype\" at path $.systemActions.InvalidActiontype.ActionType with value \"Invalid\" is not valid.  Was expecting one of the following \"System\", \"Scheduled\", \"Daily\", \"Always\"");

            CollectionAssert.Contains(jsonHelper.Errors, "systemActions list item \"BadCommandInCommands\" at path $.systemActions.BadCommandInCommands.Commands[0] is of the wrong type.  Was expecting Array but found String");

        }

    }
}
