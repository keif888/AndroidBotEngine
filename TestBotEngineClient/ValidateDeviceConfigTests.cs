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
            Assert.IsTrue(jsonHelper.Errors.Count == 0);
        }

    }
}
