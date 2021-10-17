using CommonModel.Models;
using IndividualPromotion.Helpers.Contracts;
using IndividualPromotion.Services;
using IndividualPromotionTest.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace IndividualPromotionTest.Services
{
    [TestClass]
    public class ApplyPromotionServiceTest
    {
        private Mock<IConfigurationHelper> _configurationMock;
        private Mock<ILogger<ApplyPromotionService>> _loggerMock;
        private ApplyPromotionService _applyPromotionService;

        [TestInitialize]
        public void Initialize()
        {
            _configurationMock = new Mock<IConfigurationHelper>();
            _loggerMock = new Mock<ILogger<ApplyPromotionService>>();
            _applyPromotionService = new ApplyPromotionService(_configurationMock.Object
                , _loggerMock.Object);
        }

        [DataTestMethod]
        [DynamicData(nameof(Load_CartRequest_ScenarioA), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_CartRequest_ScenarioB), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_CartRequest_ScenarioC), DynamicDataSourceType.Method)]
        public void TestApplyPromotion(CartRequest cartRequest, double expectedTotal)
        {
            MockPromotionRule(CartHelper.Get_Individual_Rule_Setting());
            var result = _applyPromotionService.ApplyPromotion(cartRequest);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.TotalAmount, expectedTotal);
        }

        #region Private Methods

        private void MockPromotionRule(IEnumerable<PromotionRuleSetting> promotionRuleSettings)
        {
            _configurationMock.Setup(x => x.GetPromotionRuleSetting()).Returns(promotionRuleSettings);
        }

        private static IEnumerable<object[]> Load_CartRequest_ScenarioA()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioA(),
                    100
                }
            };
        }

        private static IEnumerable<object[]> Load_CartRequest_ScenarioB()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioB(),
                    370
                }
            };
        }

        private static IEnumerable<object[]> Load_CartRequest_ScenarioC()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioC(),
                    285
                }
            };
        }

        #endregion
    }
}
