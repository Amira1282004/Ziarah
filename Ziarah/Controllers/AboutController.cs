using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}
