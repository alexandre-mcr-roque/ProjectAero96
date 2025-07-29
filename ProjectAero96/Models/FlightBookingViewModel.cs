using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class FlightBookingViewModel
    {
        public int FlightId { get; set; } // Flight ID
        public ICollection<FlightTicket> Tickets { get; set; } = [new FlightTicket()];

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

            [MinLength(2, ErrorMessage = "Seat {0} is invalid.")]
            [MaxLength(6, ErrorMessage = "Seat {0} is invalid.")]
            public string? SeatNumber { get; set; }

            public string SeatNumberDisplay => string.IsNullOrWhiteSpace(SeatNumber)
                ? "None"
                : SeatNumber;
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
        public DateTime BirthDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Address Line 1")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string Address1 { get; set; } = null!;

        [Display(Name = "Address Line 2 (optional)")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string? Address2 { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "This field must have {0} characters or less.")]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(50, ErrorMessage = "This field must have {0} characters or less.")]
        public string Country { get; set; } = null!;
    }
}
