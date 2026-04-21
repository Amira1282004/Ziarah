using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
    }
}
