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
    }
}
