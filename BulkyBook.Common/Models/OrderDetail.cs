

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Common.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(OrderHeader))]
        public int OrderId { get; set; }

        [ValidateNever]
        public virtual OrderHeader OrderHeader { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [ValidateNever]
        public virtual Product Product { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }
    }
}
