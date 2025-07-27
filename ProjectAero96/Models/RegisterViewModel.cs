using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class RegisterViewModel
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
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [Range(typeof(DateOnly), "1900-01-01", "2100-12-31",
            ErrorMessage = "Please enter a valid date between {1} and {2}.")]
        public DateOnly BirthDate { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;

        [Display(Name = "Phone Number (optional)")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

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

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; set; } = null!;

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
