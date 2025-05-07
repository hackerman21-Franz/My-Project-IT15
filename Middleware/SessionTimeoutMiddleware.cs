using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyProjectIT15.Middleware
{
    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionTimeoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Update the session with the current timestamp on each authenticated request
                context.Session.SetString("LastActivity", DateTime.UtcNow.ToString());
            }

            await _next(context);
        }
    }

    // Extension method to make middleware registration easier
    public static class SessionTimeoutMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionTimeout(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionTimeoutMiddleware>();
        }
    }
}
