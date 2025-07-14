using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class FlightBookingViewModel
    {
        public int FlightId { get; set; } // Flight ID
        public DateTime FlightDate { get; set; }
        public ICollection<FlightTicket> Tickets { get; set; } = [];

        public class FlightTicket
        {
            [Required]
            [Display(Name = "First Name")]
            [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
            public string FirstName { get; set; } = null!;

            [Required]
            [Display(Name = "Last Name")]
            [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
            public string LastName { get; set; } = null!;

            [Required]
            [Range(0, 200, ErrorMessage = "Please insert a valid age.")]
            public int Age { get; set; }

            [Required]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email Address")]
            public string Email { get; set; } = null!;
            public string? SeatNumber { get; set; }
        }

        // Properties for anonymous booking (automatically filled and hidden if user is authenticated)
        [Required]
        [Display(Name = "First Name")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string LastName { get; set; } = null!;

        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1900-01-01", "2100-12-31",
            ErrorMessage = "Please enter a valid date between {1} and {2}.")]
        public DateTime BirthDate { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;
    }
}
