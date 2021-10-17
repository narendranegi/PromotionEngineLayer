using CommonModel.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PromotionEngine.Helpers.Contracts;
using PromotionEngine.Services.PromotionEngine;
using PromotionEngine.Services.PromotionProxy.Contracts;
using PromotionEngineTest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngineTest.Services.PromotionEngine
{
    [TestClass]
    public class PromotionEngineServiceTest
    {
        private Mock<IConfigurationHelper> _configurationMock;
        private Mock<Func<string, IPromotionProxyService>> _serviceAccessorMock;
        private Mock<ILogger<PromotionEngineService>> _loggerMock;
        private PromotionEngineService _promotionEngineService;

        [TestInitialize]
        public void Initialize()
        {
            _configurationMock = new Mock<IConfigurationHelper>();
            _serviceAccessorMock = new Mock<Func<string, IPromotionProxyService>>();
            _loggerMock = new Mock<ILogger<PromotionEngineService>>();
            _promotionEngineService = new PromotionEngineService(_configurationMock.Object
                , _serviceAccessorMock.Object
                , _loggerMock.Object);
        }

        [DataTestMethod]
        [DynamicData(nameof(Load_Request_ScenarioA), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_Request_ScenarioB), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_Request_ScenarioC), DynamicDataSourceType.Method)]
        public async Task TestRunPromotionEngineAsync_Both_Rules(CartRequest cartRequest, string scenario
            , PromotionEngineResponse cartResponse, double expectedTotal)
        {
            MockPromotionRule(CartHelper.GetPromotionRule_Both());
            MockPromotionRuleResult(cartResponse, scenario);
            var result = await _promotionEngineService.RunPromotionEngineAsync(cartRequest);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.TotalAmount, expectedTotal);
        }

        [DataTestMethod]
        [DynamicData(nameof(Load_Request_ScenarioA), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_Request_ScenarioB), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_Individual_Request_ScenarioC), DynamicDataSourceType.Method)]
        public async Task TestRunPromotionEngineAsync_Individual_Rule(CartRequest cartRequest, string scenario
            , PromotionEngineResponse cartResponse, double expectedTotal)
        {
            MockPromotionRule(CartHelper.GetPromotionRule_Individual());
            MockPromotionRuleResult(cartResponse, scenario);
            var result = await _promotionEngineService.RunPromotionEngineAsync(cartRequest);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.TotalAmount, expectedTotal);
        }

        [DataTestMethod]
        [DynamicData(nameof(Load_Request_ScenarioA), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_Combine_Request_ScenarioB), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_Combine_Request_ScenarioC), DynamicDataSourceType.Method)]
        public async Task TestRunPromotionEngineAsync_Combine_Rule(CartRequest cartRequest, string scenario
            , PromotionEngineResponse cartResponse, double expectedTotal)
        {
            MockPromotionRule(CartHelper.GetPromotionRule_Combine());
            MockPromotionRuleResult(cartResponse, scenario);
            var result = await _promotionEngineService.RunPromotionEngineAsync(cartRequest);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.TotalAmount, expectedTotal);
        }

        [DataTestMethod]
        [DynamicData(nameof(Load_Request_ScenarioA), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_NoRule_Request_ScenarioB), DynamicDataSourceType.Method)]
        [DynamicData(nameof(Load_NoRule_Request_ScenarioC), DynamicDataSourceType.Method)]
        public async Task TestRunPromotionEngineAsync_Empty_Rule(CartRequest cartRequest, string scenario
            , PromotionEngineResponse cartResponse, double expectedTotal)
        {
            MockPromotionRule(CartHelper.GetPromotionRule_Empty());
            MockPromotionRuleResult(cartResponse, scenario);
            var result = await _promotionEngineService.RunPromotionEngineAsync(cartRequest);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.CartProductOffers.All(x => x.IsOfferApplied == false));
            Assert.AreEqual(result.TotalAmount, expectedTotal);
        }

        #region Private Methods

        private void MockPromotionRule(CommonModel.Models.PromotionRule promotionRule)
        {
            _configurationMock.Setup(x => x.GetPromotionRule()).Returns(promotionRule);
        }

        private void MockPromotionRuleResult(PromotionEngineResponse response, string scenario)
        {
            if (scenario.Equals("ScenarioB", StringComparison.OrdinalIgnoreCase))
            {
                var secondRuleResponse = new PromotionEngineResponse()
                {
                    OrderId = "test",
                    CartProductOffers = new List<CartProductOffer>
                    {
                        new CartProductOffer
                            {
                                Id = "C",
                                ItemCount = 1,
                                CostPerItem = 20,
                                IsOfferApplied = false,
                                TotalItemCost = 20
                            }
                        },
                    TotalAmount = 20,
                    IsSuccess = true
                };
                _serviceAccessorMock.Setup(x => x(It.IsAny<string>())
                        .GetPromotionRuleResult(It.Is<CartRequest>(x => x.CartProducts.Any(i => !i.Id.Contains("A")))))
                    .ReturnsAsync(secondRuleResponse);

                _serviceAccessorMock.Setup(x => x(It.IsAny<string>())
                        .GetPromotionRuleResult(It.Is<CartRequest>(x => x.CartProducts.Any(i => i.Id.Contains("A")))))
                    .ReturnsAsync(response);
            }
            else if (scenario.Equals("ScenarioC", StringComparison.OrdinalIgnoreCase))
            {
                var secondRuleResponse = new PromotionEngineResponse()
                {
                    OrderId = "test",
                    CartProductOffers = new List<CartProductOffer>
                    {
                        new CartProductOffer
                        {
                            Id = "C",
                            ItemCount = 1,
                            CostPerItem = 20,
                            IsOfferApplied = true,
                            OfferId = "Combine_C&D",
                            TotalItemCost = 0
                        },
                        new CartProductOffer
                        {
                            Id = "D",
                            ItemCount = 1,
                            CostPerItem = 15,
                            IsOfferApplied = true,
                            OfferId = "Combine_C&D",
                            TotalItemCost = 30
                        }
                    },
                    TotalAmount = 30,
                    IsSuccess = true
                };
                _serviceAccessorMock.Setup(x => x(It.IsAny<string>())
                        .GetPromotionRuleResult(It.Is<CartRequest>(x => x.CartProducts.Any(i => !i.Id.Contains("A")))))
                    .ReturnsAsync(secondRuleResponse);

                _serviceAccessorMock.Setup(x => x(It.IsAny<string>())
                        .GetPromotionRuleResult(It.Is<CartRequest>(x => x.CartProducts.Any(i => i.Id.Contains("A")))))
                    .ReturnsAsync(response);
            }
            else
                _serviceAccessorMock.Setup(x => x(It.IsAny<string>()).GetPromotionRuleResult(It.IsAny<CartRequest>()))
                    .ReturnsAsync(response);
        }

        private static IEnumerable<object[]> Load_Request_ScenarioA()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioA(),
                    "ScenarioA",
                    CartHelper.Response_ScenarioA(),
                    100
                }
            };
        }

        private static IEnumerable<object[]> Load_Request_ScenarioB()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioB(),
                    "ScenarioB",
                    CartHelper.Response_ScenarioB(),
                    370
                }
            };
        }

        private static IEnumerable<object[]> Load_Request_ScenarioC()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioC(),
                    "ScenarioC",
                    new PromotionEngineResponse
                    {
                        OrderId = "C-Scenario",
                        CartProductOffers = new List<CartProductOffer>
                        {
                            new CartProductOffer
                            {
                                Id = "A",
                                ItemCount = 3,
                                CostPerItem = 50,
                                IsOfferApplied = true,
                                OfferId = "Individual_A",
                                TotalItemCost = 130
                            },
                            new CartProductOffer
                            {
                                Id = "B",
                                ItemCount = 5,
                                CostPerItem = 30,
                                IsOfferApplied = true,
                                OfferId = "Individual_B",
                                TotalItemCost = 120
                            },
                            new CartProductOffer
                            {
                                Id = "C",
                                ItemCount = 1,
                                CostPerItem = 20,
                                IsOfferApplied = false,
                                TotalItemCost = 0
                            },
                            new CartProductOffer
                            {
                                Id = "D",
                                ItemCount = 1,
                                CostPerItem = 15,
                                IsOfferApplied = false,
                                TotalItemCost = 30
                            }
                        },
                        TotalAmount = 280,
                        IsSuccess = true
                    },
                    280
                }
            };
        }

        private static IEnumerable<object[]> Load_Individual_Request_ScenarioC()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioC(),
                    "ScenarioC - Individual Rule",
                    new PromotionEngineResponse
                    {
                        OrderId = "C-Scenario",
                        CartProductOffers = new List<CartProductOffer>
                        {
                            new CartProductOffer
                            {
                                Id = "A",
                                ItemCount = 3,
                                CostPerItem = 50,
                                IsOfferApplied = true,
                                OfferId = "Individual_A",
                                TotalItemCost = 130
                            },
                            new CartProductOffer
                            {
                                Id = "B",
                                ItemCount = 5,
                                CostPerItem = 30,
                                IsOfferApplied = true,
                                OfferId = "Individual_B",
                                TotalItemCost = 120
                            },
                            new CartProductOffer
                            {
                                Id = "C",
                                ItemCount = 1,
                                CostPerItem = 20,
                                IsOfferApplied = false,
                                TotalItemCost = 20
                            },
                            new CartProductOffer
                            {
                                Id = "D",
                                ItemCount = 1,
                                CostPerItem = 15,
                                IsOfferApplied = false,
                                TotalItemCost = 15
                            }
                        },
                        TotalAmount = 285,
                        IsSuccess = true
                    },
                    285
                }
            };
        }

        private static IEnumerable<object[]> Load_Combine_Request_ScenarioC()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioC(),
                    "ScenarioC - Combine Rule",
                    new PromotionEngineResponse
                    {
                        OrderId = "C-Scenario",
                        CartProductOffers = new List<CartProductOffer>
                        {
                            new CartProductOffer
                            {
                                Id = "A",
                                ItemCount = 3,
                                CostPerItem = 50,
                                IsOfferApplied = true,
                                OfferId = "Individual_A",
                                TotalItemCost = 150
                            },
                            new CartProductOffer
                            {
                                Id = "B",
                                ItemCount = 5,
                                CostPerItem = 30,
                                IsOfferApplied = true,
                                OfferId = "Individual_B",
                                TotalItemCost = 150
                            },
                            new CartProductOffer
                            {
                                Id = "C",
                                ItemCount = 1,
                                CostPerItem = 20,
                                IsOfferApplied = false,
                                TotalItemCost = 0
                            },
                            new CartProductOffer
                            {
                                Id = "D",
                                ItemCount = 1,
                                CostPerItem = 15,
                                IsOfferApplied = false,
                                TotalItemCost = 30
                            }
                        },
                        TotalAmount = 330,
                        IsSuccess = true
                    },
                    330
                }
            };
        }

        private static IEnumerable<object[]> Load_Combine_Request_ScenarioB()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioB(),
                    "ScenarioB - Combine Rule",
                    new PromotionEngineResponse
                    {
                        OrderId = "B-Scenario",
                        CartProductOffers = new List<CartProductOffer>
                        {
                            new CartProductOffer
                            {
                                Id = "A",
                                ItemCount = 3,
                                CostPerItem = 50,
                                IsOfferApplied = true,
                                OfferId = "Individual_A",
                                TotalItemCost = 150
                            },
                            new CartProductOffer
                            {
                                Id = "B",
                                ItemCount = 5,
                                CostPerItem = 30,
                                IsOfferApplied = true,
                                OfferId = "Individual_B",
                                TotalItemCost = 150
                            },
                            new CartProductOffer
                            {
                                Id = "C",
                                ItemCount = 1,
                                CostPerItem = 20,
                                IsOfferApplied = false,
                                TotalItemCost = 20
                            }
                        },
                        TotalAmount = 320,
                        IsSuccess = true
                    },
                    320
                }
            };
        }

        private static IEnumerable<object[]> Load_NoRule_Request_ScenarioB()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioB(),
                    "ScenarioB - No Rule",
                    new PromotionEngineResponse
                    {
                        OrderId = "B-Scenario",
                        CartProductOffers = new List<CartProductOffer>
                        {
                            new CartProductOffer
                            {
                                Id = "A",
                                ItemCount = 5,
                                CostPerItem = 50,
                                IsOfferApplied = false,
                                OfferId = "Individual_A",
                                TotalItemCost = 250
                            },
                            new CartProductOffer
                            {
                                Id = "B",
                                ItemCount = 5,
                                CostPerItem = 30,
                                IsOfferApplied = false,
                                OfferId = "Individual_B",
                                TotalItemCost = 150
                            },
                            new CartProductOffer
                            {
                                Id = "C",
                                ItemCount = 1,
                                CostPerItem = 20,
                                IsOfferApplied = false,
                                TotalItemCost = 20
                            }
                        },
                        TotalAmount = 420,
                        IsSuccess = true
                    },
                    420
                }
            };
        }

        private static IEnumerable<object[]> Load_NoRule_Request_ScenarioC()
        {
            return new[]
            {
                new object[]
                {
                    CartHelper.Request_ScenarioC(),
                    "ScenarioC - No Rule",
                    new PromotionEngineResponse
                    {
                        OrderId = "C-Scenario",
                        CartProductOffers = new List<CartProductOffer>
                        {
                            new CartProductOffer
                            {
                                Id = "A",
                                ItemCount = 3,
                                CostPerItem = 50,
                                IsOfferApplied = false,
                                OfferId = "Individual_A",
                                TotalItemCost = 150
                            },
                            new CartProductOffer
                            {
                                Id = "B",
                                ItemCount = 5,
                                CostPerItem = 30,
                                IsOfferApplied = false,
                                OfferId = "Individual_B",
                                TotalItemCost = 150
                            },
                            new CartProductOffer
                            {
                                Id = "C",
                                ItemCount = 1,
                                CostPerItem = 20,
                                IsOfferApplied = false,
                                TotalItemCost = 20
                            },
                            new CartProductOffer
                            {
                                Id = "D",
                                ItemCount = 1,
                                CostPerItem = 20,
                                IsOfferApplied = false,
                                TotalItemCost = 15
                            }
                        },
                        TotalAmount = 335,
                        IsSuccess = true
                    },
                    335
                }
            };
        }

        #endregion
    }
}
