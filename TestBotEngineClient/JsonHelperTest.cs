using BotEngineClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace TestBotEngineClient
{
    [TestClass]
    public class JsonHelperTest
    {
        [TestMethod]
        public void TestValidateListConfig_ParseValidData()
        {
            string fileName = @".\TestData\ValidListConfig.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsTrue(jsonHelper.ValidateListConfig(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 0);
        }

        [TestMethod]
        public void TestValidateListConfig_ParseInvalidFileIdType()
        {
            string fileName = @".\TestData\InvalidListConfig_FileIdWrongType.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfig(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            Assert.IsTrue(jsonHelper.Errors.Contains(string.Format("\"FileId\" value is of incorrect type.  Expecting a String but got {0}", JsonValueKind.Number)));
        }

        [TestMethod]
        public void TestValidateListConfig_WrongConfigFile()
        {
            string fileName = @".\TestData\ValidDevice.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfig(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 2);
            Assert.IsTrue(jsonHelper.Errors.Contains("\"FileId\" indicates that this is not \"ListConfig\" but DeviceConfig"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required field \"Coordinates\" missing."));
        }

        [TestMethod]
        public void TestValidateListConfig_FileIdNotValue()
        {
            string fileName = @".\TestData\InvalidListConfig_FileIdNotValue.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfig(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            Assert.IsTrue(jsonHelper.Errors.Contains("Required field \"FileId\" is of the wrong type.  Expecting a String Value"));
        }

        [TestMethod]
        public void TestValidateListConfig_OneMissingX()
        {
            string fileName = @".\TestData\InvalidListConfig_MissingOneX.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfig(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            Assert.IsTrue(jsonHelper.Errors.Contains("Required field \"X\" is missing at json path Coordinates\\StaticBalls[1]"));
        }

        [TestMethod]
        public void TestValidateListConfig_OneMissingY()
        {
            string fileName = @".\TestData\InvalidListConfig_MissingOneY.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfig(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            Assert.IsTrue(jsonHelper.Errors.Contains("Required field \"Y\" is missing at json path Coordinates\\BouncingBalls[2]"));
        }

        [TestMethod]
        public void TestValidateListConfig_MultipleErrors()
        {
            string fileName = @".\TestData\InvalidListConfig_MissingMultipleXandY.json";
            JsonHelper jsonHelper = new JsonHelper();

            Assert.IsFalse(jsonHelper.ValidateListConfig(fileName));
            Assert.IsNotNull(jsonHelper.Errors);
            Assert.IsTrue(jsonHelper.Errors.Count == 1);
            Assert.IsTrue(jsonHelper.Errors.Contains("Required field \"Y\" is missing at json path Coordinates\\BouncingBalls[1]"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required field \"X\" is missing at json path Coordinates\\StaticBalls[1]"));
            Assert.IsTrue(jsonHelper.Errors.Contains("Required field \"X\" is missing at json path Coordinates\\StaticBalls[2]"));
        }
    }
}
