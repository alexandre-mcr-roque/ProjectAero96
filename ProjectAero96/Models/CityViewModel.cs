using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class CityViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(255, ErrorMessage = "This field must have {0} characters or less.")]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(255, ErrorMessage = "This field must have {0} characters or less.")]
        public string Country { get; set; } = null!;

        [Display(Name = "Is Disabled?")]
        public bool Deleted { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Country}";
        }
    }
}
