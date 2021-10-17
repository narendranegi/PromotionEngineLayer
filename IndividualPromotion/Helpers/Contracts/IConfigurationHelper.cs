using CommonModel.Models;
using System.Collections.Generic;

namespace IndividualPromotion.Helpers.Contracts
{
    public interface IConfigurationHelper
    {
        IEnumerable<PromotionRuleSetting> GetPromotionRuleSetting();
    }
}
