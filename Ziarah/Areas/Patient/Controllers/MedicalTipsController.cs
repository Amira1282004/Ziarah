using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class MedicalTipsController : Controller
    {
        public IActionResult MedicalTips()
        {
            return View();
        }
    }
}
