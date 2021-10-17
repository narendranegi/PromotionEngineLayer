using CommonModel.Constants;
using CommonModel.Models;
using Microsoft.Extensions.Logging;
using PromotionEngine.Helpers.Contracts;
using System;
using System.Collections.Generic;

namespace PromotionEngine.Helpers
{
    public class ConfigurationHelper : IConfigurationHelper
    {
        private readonly ILogger<ConfigurationHelper> _logger;
        private PromotionRule _promotionRule;

        public ConfigurationHelper(ILogger<ConfigurationHelper> logger)
        {
            _logger = logger;
            InitializeCommonConfigurations();
        }

        public PromotionRule GetPromotionRule()
        {
            return _promotionRule;
        }

        public string GetUri(string promotionRuleType)
        {
            return promotionRuleType switch
            {
                PromotionRuleType.Individual => Environment.GetEnvironmentVariable("ENGINE_INDIVIDUAL_URI"),
                PromotionRuleType.Combined => Environment.GetEnvironmentVariable("ENGINE_COMBINED_URI"),
                _ => null
            };
        }

        private void InitializeCommonConfigurations()
        {
            //it should fetch from DB
            _promotionRule = new PromotionRule
            {
                Key = "PromotionRules",
                RuleList = new List<string>
                {
                    PromotionRuleType.Individual,
                    PromotionRuleType.Combined
                }
            };
        }
    }
}
