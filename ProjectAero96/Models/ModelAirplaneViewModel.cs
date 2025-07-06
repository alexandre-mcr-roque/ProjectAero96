using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class ModelAirplaneViewModel : ISeatConfigurationModel
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
        [Display(Name = "Price per Hour ($)")]
        [Range(1, 1000, ErrorMessage = "The price per hour must be between {1} and {2}.")]
        public decimal PricePerHour { get; set; }

        [Required]
        [Display(Name = "Maximum amount of Seats")]
        [Range(1, 1000, ErrorMessage = "The maximum amount of seats must be between {1} and {2}.")]
        public ushort MaxSeats { get; set; } = 100;

        [Required]
        [Display(Name = "Number of Seat Rows")]
        [Range(1, 1000, ErrorMessage = "The maximum amount of seats must be between {1} and {2}.")]
        public ushort SeatRows { get; set; } = 1;

        [Required]
        [Display(Name = "Number of Seat Columns")]
        [Range(1, 12, ErrorMessage = "The maximum amount of seat columns must be between {1} and {2}.")]
        public byte SeatColumns { get; set; } = 1;

        [Required]
        [Display(Name = "Number of Seats per Window")]
        [Range(1, 4, ErrorMessage = "The maximum amount of seats must be between {1} and {2}.")]
        public byte WindowSeats { get; set; } = 1;

        [Display(Name = "Is Disabled?")]
        public bool Deleted { get; set; }

        public override string ToString()
        {
            return ModelNameShort != null
                ? $"{ModelNameShort} - {SeatRows * SeatColumns} seats"
                : $"{ModelName} - {SeatRows * SeatColumns} seats";
        }
    }
}
