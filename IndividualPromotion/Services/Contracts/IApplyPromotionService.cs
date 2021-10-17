using CommonModel.Models;

namespace IndividualPromotion.Services.Contracts
{
    public interface IApplyPromotionService
    {
        PromotionEngineResponse ApplyPromotion(CartRequest cartItems);
    }
}
