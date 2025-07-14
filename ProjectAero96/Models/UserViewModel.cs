using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class UserViewModel : AccountViewModel
    {
        [Display(Name = "Bypass Age Check")]
        public bool BypassAgeCheck { get; set; }
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
