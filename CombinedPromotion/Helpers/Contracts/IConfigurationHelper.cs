using CommonModel.Models;
using System.Collections.Generic;

namespace CombinedPromotion.Helpers.Contracts
{
    public interface IConfigurationHelper
    {
        IEnumerable<PromotionRuleSetting> GetPromotionRuleSetting();
    }
}
