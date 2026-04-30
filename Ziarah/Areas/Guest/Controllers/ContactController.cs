using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
    }
}
