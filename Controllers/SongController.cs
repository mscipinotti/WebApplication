using Microsoft.AspNetCore.Mvc;

namespace WebAPP.Infrastructure.Controllers
{
    public class SongController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
