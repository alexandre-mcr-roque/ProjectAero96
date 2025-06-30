using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public interface ISeatConfigurationModel
    {
        ushort MaxSeats { get; }

        [Required]
        [Display(Name = "Number of Seat Rows")]
        [Range(1, 1000, ErrorMessage = "The maximum amount of seats must be between {1} and {2}.")]
        ushort SeatRows { get; }

        [Required]
        [Display(Name = "Number of Seat Columns")]
        [Range(1, 12, ErrorMessage = "The maximum amount of seat columns must be between {1} and {2}.")]
        byte SeatColumns { get; }

        [Required]
        [Display(Name = "Number of Seats per Window")]
        [Range(1, 4, ErrorMessage = "The maximum amount of seats must be between {1} and {2}.")]
        byte WindowSeats { get; }
    }
}
