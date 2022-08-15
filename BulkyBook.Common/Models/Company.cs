

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Common.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Company Name")]
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }

        [MaxLength(1000)]
        [DisplayName("Street Address")]
        public string StreetAddress { get; set; }


        [MaxLength(1000)]        
        public string City { get; set; }


        [MaxLength(1000)]
        public string State { get; set; }


        [MaxLength(1000)]
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }


        [MaxLength(500)]
        public string Country { get; set; }


        [ValidateNever]
        [MaxLength(1000)]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [ValidateNever]
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
