using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}
