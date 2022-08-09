using BulkyBook.Common.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Web.Models
{
    public class OrderSummaryViewModel
    {
        public string UserId { get; set; }
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
        public double OrderTotal { get; set; }

        public DateTime ShippinDate { get; set; } = DateTime.UtcNow.AddDays(4);

        public int AddressId { get; set; }

        public IEnumerable<SelectListItem> SelectAddressItem { get; internal set; }
        public IEnumerable<Address> Addresses { get; set; }
    }
}
