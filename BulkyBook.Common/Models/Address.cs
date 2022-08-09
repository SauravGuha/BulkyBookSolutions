
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Common.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string AddressLine1 { get; set; }

        [ValidateNever]
        [MaxLength(1000)]
        public string? AddressLine2 { get; set; }

        [ValidateNever]
        [MaxLength(500)]
        public string? LandMark { get; set; }

        [Required]
        [MaxLength(500)]
        public string City { get; set; }

        [Required]
        [MaxLength(500)]
        public string State { get; set; }

        [ValidateNever]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostCode { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; }

        [ValidateNever]
        public virtual Customer Customer { get; set; }
    }
}
