using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
    }
}
