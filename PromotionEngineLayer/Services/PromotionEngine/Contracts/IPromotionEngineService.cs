using CommonModel.Models;
using System.Threading.Tasks;

namespace PromotionEngine.Services.PromotionEngine.Contracts
{
    public interface IPromotionEngineService
    {
        Task<PromotionEngineResponse> RunPromotionEngineAsync(CartRequest cartItems);
    }
}
