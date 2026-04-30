using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ziarah.Areas.Guest.Models;
using Ziarah.Data;

namespace Ziarah.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class ProviderProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProviderProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ProviderProfile(int id, string type = "doctor")
        {
            if (string.Equals(type, "nurse", StringComparison.OrdinalIgnoreCase))
            {
                var nurse = await _context.Nurses
                    .AsNoTracking()
                    .Include(n => n.User)
                    .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

                if (nurse is null)
                {
                    return NotFound();
                }

                var nurseVm = new ProviderProfileViewModel
                {
                    Id = nurse.Id,
                    Type = "nurse",
                    FullName = nurse.User.FullName,
                    Specialty = "تمريض منزلي",
                    Bio = "مقدم خدمة تمريض منزلي لمتابعة الحالات المزمنة والطارئة داخل المنزل.",
                    Price = 250,
                    Rating = 4.3m,
                    ExperienceYears = 5,
                    TotalRatings = 30,
                    ImageUrl = string.IsNullOrWhiteSpace(nurse.User?.Photo)
                        ? string.Empty
                        : nurse.User.Photo!,
                    Address = "بني سويف والفيوم"
                };

                return View(nurseVm);
            }

            var doctor = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .Include(d => d.CreatedByNavigation)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (doctor is null)
            {
                return NotFound();
            }

            var vm = new ProviderProfileViewModel
            {
                Id = doctor.Id,
                Type = "doctor",
                FullName = doctor.User.FullName,
                Specialty = doctor.Specialization.Name,
                Bio = string.IsNullOrWhiteSpace(doctor.Bio)
                    ? $"طبيب متخصص في {doctor.Specialization.Name} يقدم خدمة الزيارات المنزلية."
                    : doctor.Bio,
                Price = doctor.ConsultationPrice,
                Rating = doctor.Rating,
                ExperienceYears = doctor.ExperienceYears,
                TotalRatings = doctor.TotalRatings,
                ImageUrl = string.IsNullOrWhiteSpace(doctor.User.Photo)
                    ? string.Empty
                    : doctor.User.Photo!,
                Address = "بني سويف والفيوم"
            };

            return View(vm);
        }
    }
}
