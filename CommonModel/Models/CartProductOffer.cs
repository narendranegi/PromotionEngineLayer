namespace CommonModel.Models
{
    public class CartProductOffer : CartProduct
    {
        public double TotalItemCost { get; set; }
        public bool IsOfferApplied { get; set; }
        public string OfferId { get; set; }
    }
}
