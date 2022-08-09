using BulkyBook.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Web.Models
{
    public class ProductViewModel
    {

        public Product Product { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryLists { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CoverTypeLists { get; set; }

    }
}
