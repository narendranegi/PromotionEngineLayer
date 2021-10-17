using CommonModel.Models;

namespace CombinedPromotion.Services.Contracts
{
    public interface IApplyPromotionService
    {
        PromotionEngineResponse ApplyPromotion(CartRequest cartItems);
    }
}
