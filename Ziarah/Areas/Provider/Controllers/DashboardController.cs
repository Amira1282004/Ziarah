using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ziarah.Areas.Provider.Controllers;

[Area("Provider")]
[Authorize(Roles = "Doctor,Nurse")]
public sealed class DashboardController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
