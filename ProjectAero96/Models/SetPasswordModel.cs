using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class SetPasswordModel
    {
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; set; } = null!;

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
