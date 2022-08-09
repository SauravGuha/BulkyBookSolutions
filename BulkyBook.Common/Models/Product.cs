

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Common.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        [DisplayName("Book Name")]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        [DisplayName("Book Description")]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("ISBN")]
        public string ISBN { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Book Author")]
        public string Author { get; set; }

        [Required]
        [Range(1,10000)]
        public double ListPrice { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price50 { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price100 { get; set; }

        [MaxLength(1000)]
        [DisplayName("Image Path")]
        [ValidateNever]
        public string ImageUrl { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        [ValidateNever]
        public virtual Category Category { get; set; }

        [ForeignKey(nameof(CoverType))]
        public int CoverId { get; set; }

        [ValidateNever]
        public virtual CoverType CoverType { get; set; }
    }
}
