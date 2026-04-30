using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class MedicalTipsController : Controller
    {
        public IActionResult MedicalTips()
        {
            return View();
        }
    }
}
