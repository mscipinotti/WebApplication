using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using WebAPP.Infrastructure.Utilities;

namespace WebAPP.Controllers;
public class CultureController : Controller
{
    [HttpPost]
    public IActionResult SetCulture(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
        Program.Language = culture.ToLanguage();
        return LocalRedirect(returnUrl);
    }
}
