using Microsoft.AspNetCore.Authorization;

namespace ProjectAero96.Helpers
{
    public class EnumAuthorizeAttribute : AuthorizeAttribute
    {
        public EnumAuthorizeAttribute(Roles roles) : base()
        {
            Roles = roles.ToString();
        }
    }
}