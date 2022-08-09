

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Common.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(500)]
        public string Name { get; set; }

        [DisplayName("Category Description")]
        [MaxLength(1000)]
        public string Description { get; set; }
        
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        public virtual ICollection<Product> Products { get; set; }
    }
}
