
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Common.Models
{
    /// <summary>
    /// Represents each item in shopping cart for the user
    /// </summary>
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        public int Count { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ValidateNever]
        public virtual Product Product { get; set; }

        [ForeignKey("Customer")]
        public string UserId { get; set; }

        [ValidateNever]
        public virtual Customer Customer { get; set; }

        [NotMapped]
        public double Price { get; set; }
    }
}
