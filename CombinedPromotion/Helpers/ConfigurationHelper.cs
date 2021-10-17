using CombinedPromotion.Helpers.Contracts;
using CommonModel.Constants;
using CommonModel.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CombinedPromotion.Helpers
{
    public class ConfigurationHelper : IConfigurationHelper
    {
        private readonly ILogger<ConfigurationHelper> _logger;
        private List<PromotionRuleSetting> _promotionRuleSettings;

        public ConfigurationHelper(ILogger<ConfigurationHelper> logger)
        {
            _logger = logger;
            InitializeCommonConfigurations();
        }

        public IEnumerable<PromotionRuleSetting> GetPromotionRuleSetting()
        {
            return _promotionRuleSettings.Where(x => string.Equals(x.RuleType, PromotionRuleType.Combined
                , StringComparison.OrdinalIgnoreCase));
        }

        private void InitializeCommonConfigurations()
        {
            // it should fetch from DB
            _promotionRuleSettings = new List<PromotionRuleSetting>
            {
                new PromotionRuleSetting
                {
                    OfferId = "Individual_A",
                    ProductId = "A",
                    OfferCount = 3,
                    Value = 130,
                    OfferOperation = "Sum",
                    RuleType = "Individual"
                },
                new PromotionRuleSetting
                {
                    OfferId = "Individual_B",
                    ProductId = "B",
                    OfferCount = 2,
                    Value = 45,
                    OfferOperation = "Sum",
                    RuleType = "Individual"
                },
                new PromotionRuleSetting
                {
                    OfferId = "Combined_C&D",
                    ProductId = "C,D",
                    OfferCount = 1,
                    Value = 30,
                    OfferOperation = "Sum",
                    RuleType = "Combined"
                }
            };
        }
    }
}
