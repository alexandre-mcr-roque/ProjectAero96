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
        public DateOnly BirthDate { get; set; }

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
