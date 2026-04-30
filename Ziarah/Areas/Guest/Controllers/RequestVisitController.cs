using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ziarah.Areas.Guest.Models;
using Ziarah.Areas.Guest.Services;
using Ziarah.Data;
using Ziarah.Models;

namespace Ziarah.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class RequestVisitController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public RequestVisitController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> RequestVisit()
        {
            const string doctorServiceArea = "بني سويف والفيوم";
            var doctorsRaw = await (from d in _context.Doctors.AsNoTracking()
                                    join u in _context.Users.AsNoTracking() on d.UserId equals u.Id
                                    join s in _context.Specializations.AsNoTracking() on d.SpecializationId equals s.Id
                                    where !d.IsDeleted
                                    orderby d.Rating descending
                                    select new
                                    {
                                        d.Id,
                                        ProviderName = u.FullName,
                                        SpecializationName = s.Name,
                                        d.ConsultationPrice,
                                        d.Rating,
                                        u.Photo
                                    }).ToListAsync();

            var doctors = doctorsRaw
                .Select(d => new ProviderCardViewModel
                {
                    Id = d.Id,
                    Type = "doctor",
                    Name = d.ProviderName,
                    Spec = d.SpecializationName,
                    Price = d.ConsultationPrice,
                    Rating = d.Rating,
                    ImageUrl = d.Photo ?? string.Empty,
                    HasCustomPhoto = !string.IsNullOrWhiteSpace(d.Photo),
                    Address = doctorServiceArea,
                    ProfileUrl = Url.Action("ProviderProfile", "ProviderProfile", new { area = "Guest", id = d.Id }) ?? "#"
                })
                .ToList();

            var nursesRaw = await (from n in _context.Nurses.AsNoTracking()
                                   join u in _context.Users.AsNoTracking() on n.UserId equals u.Id
                                   join hs in _context.HomeCareServices.AsNoTracking()
                                       on n.HomeCareServiceId equals hs.Id into hsJoin
                                   from hs in hsJoin.DefaultIfEmpty()
                                   where !n.IsDeleted
                                   orderby n.Id
                                   select new
                                   {
                                       n.Id,
                                       Name = u.FullName,
                                       u.Photo,
                                       Address = hs != null ? hs.ServiceLocation : null
                                   }).ToListAsync();

            var nurses = nursesRaw
                .Select(n => new ProviderCardViewModel
                {
                    Id = n.Id,
                    Type = "nurse",
                    Name = n.Name,
                    Spec = "تمريض منزلي",
                    Price = 0,
                    Rating = 0,
                    ImageUrl = n.Photo ?? string.Empty,
                    HasCustomPhoto = !string.IsNullOrWhiteSpace(n.Photo),
                    Address = string.IsNullOrWhiteSpace(n.Address) ? "غير محدد" : n.Address,
                    ProfileUrl = Url.Action("ProviderProfile", "ProviderProfile", new { area = "Guest", id = n.Id, type = "nurse" }) ?? "#"
                })
                .ToList();

            var vm = new RequestVisitViewModel
            {
                Providers = doctors.Concat(nurses).ToList(),
                Specializations = doctors
                    .Select(d => d.Spec)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<IActionResult> SubmitVisit([FromForm] VisitRequestFormModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var type = (model.ProviderType ?? string.Empty).Trim().ToLowerInvariant();
            if (type is not ("doctor" or "nurse"))
            {
                return BadRequest(new { success = false, errors = new[] { "نوع مقدم الخدمة غير صالح." } });
            }

            if (type == "doctor")
            {
                var exists = await _context.Doctors.AsNoTracking().AnyAsync(d => d.Id == model.ProviderId && !d.IsDeleted, cancellationToken);
                if (!exists)
                {
                    return BadRequest(new { success = false, errors = new[] { "الطبيب المحدد غير موجود." } });
                }
            }
            else
            {
                var exists = await _context.Nurses.AsNoTracking().AnyAsync(n => n.Id == model.ProviderId && !n.IsDeleted, cancellationToken);
                if (!exists)
                {
                    return BadRequest(new { success = false, errors = new[] { "الممرض المحدد غير موجود." } });
                }
            }

            if (model.NationalIdFront is null || model.NationalIdFront.Length == 0)
            {
                return BadRequest(new { success = false, errors = new[] { "صورة وجه بطاقة الرقم القومي مطلوبة." } });
            }

            if (model.NationalIdBack is null || model.NationalIdBack.Length == 0)
            {
                return BadRequest(new { success = false, errors = new[] { "صورة ظهر بطاقة الرقم القومي مطلوبة." } });
            }

            if (model.RequestedVisitAt is null || model.RequestedVisitAt.Value < DateTime.Now.AddMinutes(-10))
            {
                return BadRequest(new { success = false, errors = new[] { "يرجى اختيار موعد زيارة في المستقبل." } });
            }

            var frontPath = await VisitRequestUploads.TrySaveImageAsync(_env, model.NationalIdFront, "nid_front", cancellationToken);
            var backPath = await VisitRequestUploads.TrySaveImageAsync(_env, model.NationalIdBack, "nid_back", cancellationToken);
            if (frontPath is null || backPath is null)
            {
                return BadRequest(new { success = false, errors = new[] { "تعذر حفظ صور البطاقة. استخدم صورة (jpg أو png أو webp) بحجم أقل من 5 ميجابايت." } });
            }

            var createdBy = await _context.Users.AsNoTracking().OrderBy(u => u.Id).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);
            if (createdBy == 0)
            {
                return StatusCode(500, new { success = false, errors = new[] { "تعذر تسجيل الطلب حالياً." } });
            }

            var row = new HomeCareService
            {
                RequestDate = DateTime.UtcNow,
                ServiceLocation = model.Address.Trim(),
                PatientId = null,
                ServiceRequestId = null,
                Status = HomeCareServiceStatuses.Pending,
                IsDeleted = false,
                CreatedBy = createdBy,
                ProviderType = type,
                ProviderId = model.ProviderId,
                RequesterFullName = model.FullName.Trim(),
                RequesterPhone = model.Phone.Trim(),
                RequesterEmail = model.Email.Trim(),
                RequestedVisitAt = model.RequestedVisitAt,
                NationalIdFrontImage = frontPath,
                NationalIdBackImage = backPath,
                MedicalCondition = model.Condition.Trim(),
                AdditionalNotes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _context.HomeCareServices.Add(row);
            await _context.SaveChangesAsync(cancellationToken);

            return Json(new { success = true, requestId = row.Id });
        }
    }
}
