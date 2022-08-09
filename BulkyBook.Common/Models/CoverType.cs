

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Common.Models
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Cover Name")]
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }

        [DisplayName("Cover Description")]
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        public virtual ICollection<Product> Products { get; set; }
    }
}
