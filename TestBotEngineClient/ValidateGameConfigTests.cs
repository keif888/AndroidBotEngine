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
            Assert.IsTrue(jsonHelper.Errors.Count == 4);
            Assert.IsTrue(jsonHelper.Errors.Contains("findStrings list item \"findString\" at path $.findStrings.BadFindString.findString is of the wrong type.  Was expecting String but found System.Text.Json.Nodes.JsonArray"));
            Assert.IsTrue(jsonHelper.Errors.Contains("findStrings list item \"searchArea\" at path $.findStrings.BadSearchArea.searchArea is of the wrong type.  Was expecting Object containing a search area"));
            Assert.IsTrue(jsonHelper.Errors.Contains("findStrings list item \"textTolerance\" at path $.findStrings.BadTextTolerance.textTolerance is of the wrong type.  Was expecting Number but found String"));
            Assert.IsTrue(jsonHelper.Errors.Contains("findStrings list item \"backgroundTolerance\" at path $.findStrings.BadBackgroundTolerance.backgroundTolerance is of the wrong type.  Was expecting Number but found String"));
        }

    }
}
