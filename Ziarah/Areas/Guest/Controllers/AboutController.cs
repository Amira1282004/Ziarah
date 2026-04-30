using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}
