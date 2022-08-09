

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Common.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Customer))]
        public string UserId { get; set; }

        [ValidateNever]
        public virtual Customer Customer { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime ShippinDate { get; set; }

        public double OrderTotal { get; set; }

        [MaxLength(200)]
        public string? OrderStatus { get; set; }

        [MaxLength(200)]
        public string? PaymentStatus    { get; set; }

        [MaxLength(200)]
        public string? TrackingNumber   { get; set; }

        [MaxLength(200)]
        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime PaymentDueDate { get; set; }

        [MaxLength(200)]
        public string? SessionId { get; set; }

        [MaxLength(200)]
        public string? PaymentIntentId { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public int AddressId { get; set; }
    }
}
