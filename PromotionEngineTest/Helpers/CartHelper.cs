using CommonModel.Models;
using System.Collections.Generic;
using CommonModel.Constants;

namespace PromotionEngineTest.Helpers
{
    public class CartHelper
    {
        public static CartRequest CartRequest()
        {
            return new CartRequest
            {
                OrderId = "FirstScenario",
                Name = "Test",
                Address = "Test",
                CartProducts = new List<CartProduct>
                {
                    new CartProduct
                    {
                        Id = "A",
                        ItemCount = 1,
                        CostPerItem = 50
                    },
                    new CartProduct
                    {
                        Id = "B",
                        ItemCount = 1,
                        CostPerItem = 30
                    },
                    new CartProduct
                    {
                        Id = "C",
                        ItemCount = 1,
                        CostPerItem = 20
                    }
                }
            };
        }

        #region CartRequest

        public static CartRequest Request_ScenarioA()
        {
            return new CartRequest
            {
                OrderId = "FirstScenario",
                Name = "Test",
                Address = "Test",
                CartProducts = new List<CartProduct>
                {
                    new CartProduct
                    {
                        Id = "A",
                        ItemCount = 1,
                        CostPerItem = 50
                    },
                    new CartProduct
                    {
                        Id = "B",
                        ItemCount = 1,
                        CostPerItem = 30
                    },
                    new CartProduct
                    {
                        Id = "C",
                        ItemCount = 1,
                        CostPerItem = 20
                    }
                }
            };
        }

        public static CartRequest Request_ScenarioB()
        {
            return new CartRequest
            {
                OrderId = "SecondScenario",
                Name = "Test",
                Address = "Test",
                CartProducts = new List<CartProduct>
                {
                    new CartProduct
                    {
                        Id = "A",
                        ItemCount = 5,
                        CostPerItem = 50
                    },
                    new CartProduct
                    {
                        Id = "B",
                        ItemCount = 5,
                        CostPerItem = 30
                    },
                    new CartProduct
                    {
                        Id = "C",
                        ItemCount = 1,
                        CostPerItem = 20
                    }
                }
            };
        }

        public static CartRequest Request_ScenarioC()
        {
            return new CartRequest
            {
                OrderId = "ThirdScenario",
                Name = "Test",
                Address = "Test",
                CartProducts = new List<CartProduct>
                {
                    new CartProduct
                    {
                        Id = "A",
                        ItemCount = 3,
                        CostPerItem = 50
                    },
                    new CartProduct
                    {
                        Id = "B",
                        ItemCount = 5,
                        CostPerItem = 30
                    },
                    new CartProduct
                    {
                        Id = "C",
                        ItemCount = 1,
                        CostPerItem = 20
                    },
                    new CartProduct
                    {
                        Id = "D",
                        ItemCount = 1,
                        CostPerItem = 15
                    }
                }
            };
        }

        #endregion

        #region CartResponse

        public static PromotionEngineResponse Response_ScenarioA()
        {
            return new PromotionEngineResponse
            {
                OrderId = "A-Scenario",
                CartProductOffers = new List<CartProductOffer>
                {
                    new CartProductOffer
                    {
                        Id = "A",
                        ItemCount = 1,
                        CostPerItem = 50,
                        IsOfferApplied = false,
                        TotalItemCost = 50
                    },
                    new CartProductOffer
                    {
                        Id = "B",
                        ItemCount = 1,
                        CostPerItem = 30,
                        IsOfferApplied = false,
                        TotalItemCost = 30
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
                TotalAmount = 100,
                IsSuccess = true
            };
        }

        public static PromotionEngineResponse Response_ScenarioB()
        {
            return new PromotionEngineResponse
            {
                OrderId = "B-Scenario",
                CartProductOffers = new List<CartProductOffer>
                {
                    new CartProductOffer
                    {
                        Id = "A",
                        ItemCount = 5,
                        CostPerItem = 50,
                        IsOfferApplied = true,
                        OfferId = "Individual_A",
                        TotalItemCost = 230
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
                    }
                },
                TotalAmount = 370,
                IsSuccess = true
            };
        }

        public static PromotionEngineResponse Response_ScenarioC()
        {
            return new PromotionEngineResponse
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
                        IsOfferApplied = true,
                        OfferId = "Combined_C&D",
                        TotalItemCost = 0
                    },
                    new CartProductOffer
                    {
                        Id = "D",
                        ItemCount = 1,
                        CostPerItem = 15,
                        IsOfferApplied = true,
                        OfferId = "Combined_C&D",
                        TotalItemCost = 30
                    }
                },
                TotalAmount = 280,
                IsSuccess = true
            };
        }

        #endregion


        #region PromotionRule

        public static PromotionRule GetPromotionRule_Both()
        {
            return new PromotionRule()
            {
                Key = "PromotionRule",
                RuleList = new List<string>
                {
                    PromotionRuleType.Individual,
                    PromotionRuleType.Combined
                }
            };
        }

        public static PromotionRule GetPromotionRule_Individual()
        {
            return new PromotionRule()
            {
                Key = "PromotionRule",
                RuleList = new List<string>
                {
                    PromotionRuleType.Individual
                }
            };
        }

        public static PromotionRule GetPromotionRule_Combine()
        {
            return new PromotionRule()
            {
                Key = "PromotionRule",
                RuleList = new List<string>
                {
                    PromotionRuleType.Combined
                }
            };
        }

        public static PromotionRule GetPromotionRule_Empty()
        {
            return new PromotionRule()
            {
                Key = "PromotionRule",
                RuleList = new List<string>()
            };
        }

        #endregion
    }
}
