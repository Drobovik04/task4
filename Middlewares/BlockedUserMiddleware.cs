using Microsoft.AspNetCore.Identity;
using task4.Models;

namespace task4.Middlewares
{
    public class BlockedUserMiddleware
    {
        private readonly RequestDelegate _next;

        public BlockedUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(context.User);

                if (user != null && user.Status=="Blocked")
                {
                    await signInManager.SignOutAsync();
                    context.Response.Redirect("/Account/Login?blocked=true");
                    return;
                }
            }

            await _next(context);
        }
    }

}
