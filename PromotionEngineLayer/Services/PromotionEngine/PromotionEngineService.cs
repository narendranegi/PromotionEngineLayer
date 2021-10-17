using CommonModel.Models;
using Microsoft.Extensions.Logging;
using PromotionEngine.Helpers.Contracts;
using PromotionEngine.Services.PromotionEngine.Contracts;
using PromotionEngine.Services.PromotionProxy.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine.Services.PromotionEngine
{
    public class PromotionEngineService : IPromotionEngineService
    {
        private readonly IConfigurationHelper _configurationHelper;
        private readonly Func<string, IPromotionProxyService> _serviceAccessor;
        private readonly ILogger<PromotionEngineService> _logger;

        public PromotionEngineService(IConfigurationHelper configurationHelper
            , Func<string, IPromotionProxyService> serviceAccessor
            , ILogger<PromotionEngineService> logger)
        {
            _configurationHelper = configurationHelper;
            _serviceAccessor = serviceAccessor;
            _logger = logger;
        }

        public async Task<PromotionEngineResponse> RunPromotionEngineAsync(CartRequest cartRequet)
        {
            var promotionRuleResults = new List<PromotionEngineResponse>();
            CommonModel.Models.PromotionRule rules = _configurationHelper.GetPromotionRule();

            foreach (var rule in rules.RuleList)
            {
                var promotionService = _serviceAccessor(rule);
                var result = await promotionService.GetPromotionRuleResult(cartRequet);

                if (result != null && result.IsSuccess && result.CartProductOffers.Any(x => x.IsOfferApplied))
                {
                    promotionRuleResults.Add(result);
                    cartRequet.CartProducts = result.CartProductOffers.Where(x => x.IsOfferApplied == false)
                        .Select(x => new CartProduct
                        {
                            Id = x.Id,
                            ItemCount = x.ItemCount,
                            CostPerItem = x.CostPerItem
                        }).ToList();
                }
            }

            // _logger.LogDebug("Promotion engine executed promotion rule's for orderId = {orderId}",
            //   cartItems.OrderId);
            return promotionRuleResults.Count switch
            {
                0 => GetPromotionRuleResult(cartRequet),
                1 => promotionRuleResults.FirstOrDefault(),
                _ => GetPromotionRuleResult(promotionRuleResults)
            };
        }

        private PromotionEngineResponse GetPromotionRuleResult(IEnumerable<PromotionEngineResponse> lstPromotionRuleResults)
        {
            PromotionEngineResponse promotionResult = lstPromotionRuleResults.FirstOrDefault();
            var updatedResults = lstPromotionRuleResults.Skip(1).ToList()
                .SelectMany(x => x.CartProductOffers);

            foreach (var item in promotionResult.CartProductOffers
                .Where(item => updatedResults.Any(x => x.Id == item.Id && x.IsOfferApplied)))
            {
                var result = updatedResults.Where(x => x.Id == item.Id).FirstOrDefault();
                item.TotalItemCost = result.TotalItemCost;
                item.OfferId = result.OfferId;
                item.IsOfferApplied = true;
            }

            promotionResult.TotalAmount = promotionResult.CartProductOffers.Select(x => x.TotalItemCost).Sum();
            return promotionResult;
        }

        private PromotionEngineResponse GetPromotionRuleResult(CartRequest cartRequest)
        {
            var lstProductItem = cartRequest.CartProducts.Select(product => new CartProductOffer
            {
                Id = product.Id,
                CostPerItem = product.CostPerItem,
                ItemCount = product.ItemCount,
                IsOfferApplied = false,
                TotalItemCost = product.CostPerItem * product.ItemCount
            }).ToList();

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
