using System.Collections.Generic;

namespace CommonModel.Models
{
    public class PromotionEngineResponse : BaseResult
    {
        public string OrderId { get; set; }
        public List<CartProductOffer> CartProductOffers { get; set; }
        public double TotalAmount { get; set; }
    }
}
