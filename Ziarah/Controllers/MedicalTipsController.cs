using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Controllers
{
    public class MedicalTipsController : Controller
    {
        public IActionResult MedicalTips()
        {
            return View();
        }
    }
}
