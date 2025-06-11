using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAero96.Data.Entities
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual User User { get; set; } = null!;
        public virtual IdentityRole Role { get; set; } = null!;
    }
}
