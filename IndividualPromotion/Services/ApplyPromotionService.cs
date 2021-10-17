using CommonModel.Models;
using IndividualPromotion.Helpers.Contracts;
using IndividualPromotion.Services.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndividualPromotion.Services
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
            double totalPrice;
            bool offerApplied = false;
            string offerId = null;

            foreach (var product in cartRequest.CartProducts)
            {
                var configOffer = rules
                    .FirstOrDefault(x => string.Equals(x.ProductId, product.Id, StringComparison.OrdinalIgnoreCase)
                                         && product.ItemCount >= x.OfferCount);
                if (configOffer != null)
                {
                    totalPrice = (product.ItemCount / configOffer.OfferCount) * configOffer.Value
                                 + (product.ItemCount % configOffer.OfferCount * product.CostPerItem);
                    offerApplied = true;
                    offerId = configOffer.OfferId;
                }
                else
                {
                    totalPrice = product.ItemCount * product.CostPerItem;
                    offerApplied = false;
                }

                lstProductItem.Add(new CartProductOffer
                {
                    Id = product.Id,
                    CostPerItem = product.CostPerItem,
                    ItemCount = product.ItemCount,
                    IsOfferApplied = offerApplied,
                    OfferId = offerId,
                    TotalItemCost = totalPrice
                });
            }

            double totalAmount = lstProductItem.Select(x => x.TotalItemCost).Sum();

            return new PromotionEngineResponse
            {
                OrderId = cartRequest.OrderId,
                CartProductOffers = lstProductItem,
                TotalAmount = totalAmount,
                IsSuccess = true
            };
        }
    }
}
