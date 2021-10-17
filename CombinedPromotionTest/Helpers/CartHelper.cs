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
    }
}
