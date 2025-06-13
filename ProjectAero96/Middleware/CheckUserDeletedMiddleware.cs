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
                var userId = userManager.GetUserId(context.User);
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user == null || user.Deleted) // user no longer exists or is marked as deleted
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
