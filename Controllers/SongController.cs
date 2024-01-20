using Microsoft.AspNetCore.Mvc;

namespace WebAPP.Controllers
{
    public class SongController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
