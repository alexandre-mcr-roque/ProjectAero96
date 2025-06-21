using Microsoft.AspNetCore.Identity;

namespace ProjectAero96.Data.Entities
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual User User { get; set; } = null!;
        public virtual IdentityRole Role { get; set; } = null!;
    }
}
