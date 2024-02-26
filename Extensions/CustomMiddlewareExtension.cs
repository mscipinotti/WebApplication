using WebAPP.MiddlewareFactory;

namespace WebAPP.Extensions
{
    public static class CustomMiddlewareExtension
    {
        public static void UseCustomMiddlewareExtension(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggerMiddlleware>();
        }
    }
}
