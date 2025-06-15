namespace ProjectAero96.Middleware
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using ProjectAero96.Data.Entities;

    public class CheckUserDeletedMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate next = next;

        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var uid = userManager.GetUserId(context.User);
                if (!string.IsNullOrEmpty(uid))
                {
                    var user = await userManager.FindByIdAsync(uid);
                    // Invalid user (deleted/disabled or requires password change)
                    // TODO Remove RequiresPasswordChange and EmailConfirmed checks if I don't add a way for admins to modify them
                    if (user == null || user.Deleted || user.RequiresPasswordChange || !user.EmailConfirmed)
                    {
                        await signInManager.SignOutAsync();
                        context.Response.Redirect("/signin");
                        return;
                    }
                }
            }

            await next(context);
        }
    }
}
