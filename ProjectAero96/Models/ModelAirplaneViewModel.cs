using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class ModelAirplaneViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Model Name")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string ModelName { get; set; } = null!;

        [Display(Name = "Shortened Model Name (optional)")]
        [MaxLength(20, ErrorMessage = "This field must have {0} characters or less.")]
        public string? ModelNameShort { get; set; }

        [Required]
        [Display(Name = "Price per Hour")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        [Range(1, 1000, ErrorMessage = "The price per hour must be between {1} and {2}.")]
        public decimal PricePerTime { get; set; }

        [Required]
        [Display(Name = "Maximum amount of Seats")]
        [Range(1, 1000, ErrorMessage = "The maximum amount of seats must be between {1} and {2}.")]
        public ushort MaxSeats { get; set; }

        [Display(Name = "Is Disabled?")]
        public bool Deleted { get; set; }
    }
}
