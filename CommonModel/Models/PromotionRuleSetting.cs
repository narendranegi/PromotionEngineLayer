namespace CommonModel.Models
{
    public class PromotionRuleSetting
    {
        public string OfferId { get; set; }
        public string ProductId { get; set; }
        public int OfferCount { get; set; }
        public string OfferOperation { get; set; }
        public double Value { get; set; }
        public string RuleType { get; set; }
    }
}
