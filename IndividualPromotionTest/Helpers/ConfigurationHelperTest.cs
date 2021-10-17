using System.Linq;
using CommonModel.Constants;
using IndividualPromotion.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IndividualPromotionTest.Helpers
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
        }

        [TestMethod]
        public void TestGetPromotionRuleSetting()
        {
            var result = _configurationHelper.GetPromotionRuleSetting();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.All(x=>x.RuleType.Equals(PromotionRuleType.Individual)));
        }
    }
}
