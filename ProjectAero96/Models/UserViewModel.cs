using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class UserViewModel
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
        public string FullName => $"{FirstName} {LastName}";

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

        public string? Roles { get; set; } = null!;
        public bool Deleted { get; set; }

        ///<summary>Used in CreateUser</summary>
        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }
        ///<summary>Used in CreateUser</summary>
        [Display(Name = "Employee")]
        public bool IsEmployee { get; set; }
        ///<summary>Used in CreateUser</summary>
        [Display(Name = "Client")]
        public bool IsClient { get; set; }

        [AllowedValues(true, ErrorMessage = "Please select at least 1 role.")]
        public bool HasRole => IsAdmin || IsEmployee || IsClient;
    }
}
