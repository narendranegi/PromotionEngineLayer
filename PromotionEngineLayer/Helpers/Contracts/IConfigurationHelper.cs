using CommonModel.Models;

namespace PromotionEngine.Helpers.Contracts
{
    public interface IConfigurationHelper
    {
        PromotionRule GetPromotionRule();
        string GetUri(string promotionRuleType);
    }
}
