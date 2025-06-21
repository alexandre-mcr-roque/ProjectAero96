using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class AccountViewModel
    {
        public string Id { get; set; } = null!;

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Full Name")]
        public string FullName => string.IsNullOrEmpty(LastName)
            ? FirstName
            : $"{FirstName} {LastName}";

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;

        [Display(Name = "Phone Number (optional)")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        [Display(Name = "Phone Number")]
        public string Phone => string.IsNullOrEmpty(PhoneNumber)
            ? "None"
            : PhoneNumber;

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



        [Display(Name = "Address")]
        public string FullAddress => string.IsNullOrEmpty(Address2)
            ? $"{Address1}, {City}, {Country}"
            : $"{Address1} {Address2}, {City}, {Country}";
    }
}
