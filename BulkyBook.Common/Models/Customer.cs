

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Common.Models
{
    public class Customer : IdentityUser
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        [ValidateNever]
        public int? CompanyId { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
    }
}
