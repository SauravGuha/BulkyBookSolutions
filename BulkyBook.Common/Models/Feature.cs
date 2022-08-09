

using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Common.Models
{
    public class Feature
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ControllerName { get; set; }

    }
}
