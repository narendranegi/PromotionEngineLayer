using CommonModel.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PromotionEngine.Helpers;
using System;
using System.Linq;

namespace PromotionEngineTest.Helpers
{
    [TestClass]
    public class ConfigurationHelperTest
    {
        private Mock<ILogger<ConfigurationHelper>> _loggerMock;
        private ConfigurationHelper _configurationHelper;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger<ConfigurationHelper>>();
            _configurationHelper = new ConfigurationHelper(_loggerMock.Object);
            Environment.SetEnvironmentVariable("ENGINE_INDIVIDUAL_URI", "https://individualengine.com");
            Environment.SetEnvironmentVariable("ENGINE_COMBINED_URI", "https://combinedengine.com");
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(PromotionRuleType.Individual, "https://individualengine.com")]
        [DataRow(PromotionRuleType.Combined, "https://combinedengine.com")]
        public void TestGetURI(string promotionType, string uri)
        {
            var result = _configurationHelper.GetUri(promotionType);
            Assert.AreEqual(result, uri);
        }

        [TestMethod]
        public void TestGetPromotionRule()
        {
            var result = _configurationHelper.GetPromotionRule();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.RuleList.Any(x=>x.Equals(PromotionRuleType.Individual)));
            Assert.IsTrue(result.RuleList.Any(x=>x.Equals(PromotionRuleType.Combined)));
            
        }
    }
}
