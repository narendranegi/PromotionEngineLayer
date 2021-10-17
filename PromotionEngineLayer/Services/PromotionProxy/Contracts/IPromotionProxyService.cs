using System.Threading.Tasks;
using CommonModel.Models;

namespace PromotionEngine.Services.PromotionProxy.Contracts
{
    public interface IPromotionProxyService
    {
        Task<PromotionEngineResponse> GetPromotionRuleResult(CartRequest cartItems);
    }
}
