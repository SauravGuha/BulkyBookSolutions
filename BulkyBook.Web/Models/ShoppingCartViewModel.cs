using BulkyBook.Common.Models;

namespace BulkyBook.Web.Models
{
    public class ShoppingCartViewModel
    {
        public double CartTotal { get; set; }

        public List<ShoppingCart> ShoppingCarts { get; set; }
    }
}
