using System.Collections.Generic;

namespace CommonModel.Models
{
    public class CartRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string OrderId { get; set; }
        public List<CartProduct> CartProducts { get; set; }
    }
}
