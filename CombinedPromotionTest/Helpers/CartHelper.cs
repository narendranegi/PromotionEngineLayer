using CommonModel.Models;
using System.Collections.Generic;

namespace CombinedPromotionTest.Helpers
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

        public static List<PromotionRuleSetting> Get_Combine_Rule_Setting()
        {
            return new List<PromotionRuleSetting>
            {
                new PromotionRuleSetting
                {
                    OfferId = "Combine_C&D",
                    ProductId = "C,D",
                    OfferCount = 1,
                    Value = 30,
                    OfferOperation = "Sum",
                    RuleType = "Combine"
                }
            };
        }

    }
}
