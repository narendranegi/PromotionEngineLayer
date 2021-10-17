using System;
using System.Collections.Generic;
using System.Linq;
using CombinedPromotion.Helpers.Contracts;
using CombinedPromotion.Services.Contracts;
using CommonModel.Models;
using Microsoft.Extensions.Logging;

namespace CombinedPromotion.Services
{
    public class ApplyPromotionService : IApplyPromotionService
    {
        private readonly IConfigurationHelper _configuration;
        private readonly ILogger<ApplyPromotionService> _logger;

        public ApplyPromotionService(IConfigurationHelper configuration
            , ILogger<ApplyPromotionService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public PromotionEngineResponse ApplyPromotion(CartRequest cartRequest)
        {
            var lstProductItem = new List<CartProductOffer>();
            var rules = _configuration.GetPromotionRuleSetting();
            var combineProducts = getCombineList(cartRequest.CartProducts);
            foreach (var product in combineProducts)
            {
                var configOffer = rules?.Where(x => string.Equals(x.ProductId, product, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (configOffer != null)
                {
                    GetCombineProductTotalWithOffer(product, cartRequest.CartProducts, configOffer, lstProductItem);
                }
            }

            var result = cartRequest.CartProducts.Where(x => !lstProductItem.Exists(y => x.Id == y.Id));
            lstProductItem.AddRange(result.Select(item => new CartProductOffer()
            {
                Id = item.Id,
                CostPerItem = item.CostPerItem,
                ItemCount = item.ItemCount,
                IsOfferApplied = false,
                TotalItemCost = item.ItemCount * item.CostPerItem
            }));

            double totalAmount = lstProductItem.Select(x => x.TotalItemCost).Sum();
            return new PromotionEngineResponse
            {
                OrderId = cartRequest.OrderId,
                CartProductOffers = lstProductItem,
                TotalAmount = totalAmount,
                IsSuccess = true
            };
        }


        private void GetCombineProductTotalWithOffer(string product, List<CartProduct> items
            , PromotionRuleSetting offer
            , List<CartProductOffer> result)
        {
            double totalPrice;
            var firstProduct = items
                .FirstOrDefault(x => string.Equals(x.Id, product.Split(",")[0], StringComparison.OrdinalIgnoreCase));
            var secondProduct = items
                .FirstOrDefault(x => string.Equals(x.Id, product.Split(",")[1], StringComparison.OrdinalIgnoreCase));
            int firstProductCount = firstProduct?.ItemCount ?? 0;
            int secondProductCount = secondProduct?.ItemCount ?? 0;
            int commonCount = firstProductCount > secondProductCount ? secondProductCount : firstProductCount;

            if (firstProductCount - secondProductCount == 0)
            {
                totalPrice = commonCount * offer.Value;
            }
            else
            {
                totalPrice = (commonCount * offer.Value)
                             + (firstProductCount - commonCount) * firstProduct.CostPerItem
                             + (secondProductCount - commonCount) * secondProduct.CostPerItem;
            }

            result.Add(new CartProductOffer
            {
                Id = firstProduct.Id,
                CostPerItem = firstProduct.CostPerItem,
                ItemCount = firstProduct.ItemCount,
                IsOfferApplied = true,
                OfferId = offer.OfferId,
                TotalItemCost = 0
            });
            result.Add(new CartProductOffer
            {
                Id = secondProduct.Id,
                CostPerItem = secondProduct.CostPerItem,
                ItemCount = secondProduct.ItemCount,
                IsOfferApplied = true,
                OfferId = offer.OfferId,
                TotalItemCost = totalPrice
            });
        }

        private IEnumerable<string> getCombineList(List<CartProduct> items)
        {
            var result = new List<string>();
            double count = Math.Pow(2, items.Count);

            for (int i = 1; i <= count - 1; i++)
            {
                var combineItem = string.Empty;
                var rule = Convert.ToString(i, 2).PadLeft(items.Count, '0');
                for (int j = 0; j < rule.Length; j++)
                {
                    if (rule[j] == '1')
                    {
                        combineItem = combineItem.Equals(string.Empty) ? items[j].Id : string.Join(",", combineItem, items[j].Id);
                    }
                }
                if (combineItem.Split(",").Length == 2)
                    result.Add(combineItem);
            }

            return result;
        }
    }
}
