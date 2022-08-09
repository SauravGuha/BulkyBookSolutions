using BulkyBook.Common.Models;

namespace BulkyBook.Web.Models
{
    public class DetailsViewModel
    {
        public int ShoppingCartId { get; set; }
        public int OldCount { get; set; }
        public int Count { get; set; }
        public Product Product { get; set; }
    }
}
