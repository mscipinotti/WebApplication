using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebAPP.Infrastructure.Infrastructure
{
    public class CookieAuthenticationEvent : CookieAuthenticationEvents
    {
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            await Task.Run(() => context.Request.HttpContext.Items.Add("ExpiresUTC", context.Properties.ExpiresUtc));
        }
    }
}
