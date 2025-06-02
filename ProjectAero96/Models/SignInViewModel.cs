using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class SignInViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Stay signed in")]
        public bool RememberMe { get; set; }
    }
}
