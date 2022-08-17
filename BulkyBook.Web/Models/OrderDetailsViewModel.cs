using BulkyBook.Common.Models;

namespace BulkyBook.Web.Models
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }

        public Address Address { get; set; }
    }
}
