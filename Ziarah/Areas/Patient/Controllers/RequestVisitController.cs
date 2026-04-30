using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ziarah.Areas.Guest.Services;   // VisitRequestUploads (مشترك بين Guest و Patient)
using Ziarah.Areas.Patient.Models;
using Ziarah.Data;
using Ziarah.Models;

namespace Ziarah.Areas.Patient.Controllers;

[Area("Patient")]
[Authorize(Roles = "Patient")]
public sealed class RequestVisitController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment  _env;
    private readonly ILogger<RequestVisitController> _logger;

    public RequestVisitController(
        ApplicationDbContext db,
        IWebHostEnvironment  env,
        ILogger<RequestVisitController> logger)
    {
        _db     = db;
        _env    = env;
        _logger = logger;
    }

    // ── GET: /Patient/RequestVisit ────────────────────────────────────────────
    public async Task<IActionResult> RequestVisit(CancellationToken ct)
    {
        // هوية المريض
        if (!TryGetUserId(out var userId))
            return RedirectToAction("Login", "Login", new { area = "Identity" });

        // بيانات المستخدم
        var user = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .Select(u => new
            {
                u.FirstName, u.LastName, u.Email, u.PhoneNumber,
                u.NationalIdFrontImage, u.NationalIdBackImage
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return RedirectToAction("Login", "Login", new { area = "Identity" });

        // ── جلب الأطباء ──────────────────────────────────────────────────────
        const string serviceArea = "بني سويف والفيوم";

        var doctorsRaw = await (
            from d in _db.Doctors.AsNoTracking()
            join u in _db.Users.AsNoTracking()           on d.UserId          equals u.Id
            join s in _db.Specializations.AsNoTracking() on d.SpecializationId equals s.Id
            where !d.IsDeleted
            orderby d.Rating descending
            select new
            {
                d.Id,
                Name  = u.FullName,
                Spec  = s.Name,
                d.ConsultationPrice,
                d.Rating,
                u.Photo
            }
        ).ToListAsync(ct);

        var doctors = doctorsRaw.Select(d => new PatientProviderCardViewModel
        {
            Id             = d.Id,
            Type           = "doctor",
            Name           = d.Name,
            Spec           = d.Spec,
            Price          = d.ConsultationPrice,
            Rating         = d.Rating,
            ImageUrl       = string.IsNullOrWhiteSpace(d.Photo) ? "/assets/image/default-provider.png" : d.Photo,
            HasCustomPhoto = !string.IsNullOrWhiteSpace(d.Photo),
            Address        = serviceArea,
            ProfileUrl     = Url.Action("ProviderProfile", "ProviderProfile",
                                 new { area = "Patient", id = d.Id }) ?? "#"
        }).ToList();

        // ── جلب الممرضين ─────────────────────────────────────────────────────
        var nursesRaw = await (
            from n in _db.Nurses.AsNoTracking()
            join u in _db.Users.AsNoTracking() on n.UserId equals u.Id
            join hs in _db.HomeCareServices.AsNoTracking()
                on n.HomeCareServiceId equals hs.Id into hsJoin
            from hs in hsJoin.DefaultIfEmpty()
            where !n.IsDeleted
            orderby n.Id
            select new
            {
                n.Id,
                Name    = u.FullName,
                u.Photo,
                Address = hs != null ? hs.ServiceLocation : null
            }
        ).ToListAsync(ct);

        var nurses = nursesRaw.Select(n => new PatientProviderCardViewModel
        {
            Id             = n.Id,
            Type           = "nurse",
            Name           = n.Name,
            Spec           = "تمريض منزلي",
            Price          = 0,
            Rating         = 0,
            ImageUrl       = string.IsNullOrWhiteSpace(n.Photo) ? "/assets/image/default-provider.png" : n.Photo,
            HasCustomPhoto = !string.IsNullOrWhiteSpace(n.Photo),
            Address        = string.IsNullOrWhiteSpace(n.Address) ? "غير محدد" : n.Address,
            ProfileUrl     = Url.Action("ProviderProfile", "ProviderProfile",
                                 new { area = "Patient", id = n.Id, type = "nurse" }) ?? "#"
        }).ToList();

        // ── ViewModel ────────────────────────────────────────────────────────
        var vm = new PatientRequestVisitViewModel
        {
            FullName           = $"{user.FirstName} {user.LastName}".Trim(),
            Phone              = user.PhoneNumber ?? string.Empty,
            Email              = user.Email       ?? string.Empty,
            NationalIdFrontUrl = user.NationalIdFrontImage,
            NationalIdBackUrl  = user.NationalIdBackImage,
            Providers          = doctors.Concat(nurses).ToList(),
            Specializations    = doctors
                .Select(d => d.Spec)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => s)
                .ToList()
        };

        return View(vm);
    }

    // ── POST: /Patient/RequestVisit/SubmitVisit ───────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> SubmitVisit(
        [FromForm] PatientVisitRequestFormModel model,
        CancellationToken ct)
    {
        // هوية المريض
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { success = false, errors = new[] { "يجب تسجيل الدخول أولاً." } });

        // ── التحقق من ModelState ─────────────────────────────────────────────
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors  = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        // ── التحقق من نوع مقدم الخدمة ────────────────────────────────────────
        var type = (model.ProviderType ?? string.Empty).Trim().ToLowerInvariant();
        if (type is not ("doctor" or "nurse"))
            return BadRequest(new { success = false, errors = new[] { "نوع مقدم الخدمة غير صالح." } });

        // ── التحقق من وجود مقدم الخدمة ───────────────────────────────────────
        if (type == "doctor")
        {
            var exists = await _db.Doctors.AsNoTracking()
                .AnyAsync(d => d.Id == model.ProviderId && !d.IsDeleted, ct);
            if (!exists)
                return BadRequest(new { success = false, errors = new[] { "الطبيب المحدد غير موجود." } });
        }
        else
        {
            var exists = await _db.Nurses.AsNoTracking()
                .AnyAsync(n => n.Id == model.ProviderId && !n.IsDeleted, ct);
            if (!exists)
                return BadRequest(new { success = false, errors = new[] { "الممرض المحدد غير موجود." } });
        }

        // ── التحقق من الموعد ──────────────────────────────────────────────────
        if (model.RequestedVisitAt is null || model.RequestedVisitAt.Value < DateTime.Now.AddMinutes(-10))
            return BadRequest(new { success = false, errors = new[] { "يرجى اختيار موعد زيارة في المستقبل." } });

        // ── معالجة صور البطاقة ────────────────────────────────────────────────
        string? frontPath;
        string? backPath;

        // إذا كانت البطاقة محفوظة مسبقاً في الحساب نستخدمها مباشرة
        bool useSavedNid = model.UseAccount &&
                           !string.IsNullOrWhiteSpace(model.SavedNidFront) &&
                           !string.IsNullOrWhiteSpace(model.SavedNidBack);

        if (useSavedNid)
        {
            frontPath = model.SavedNidFront!;
            backPath  = model.SavedNidBack!;
        }
        else
        {
            // يجب رفع الصورتين
            if (model.NationalIdFront is null || model.NationalIdFront.Length == 0)
                return BadRequest(new { success = false, errors = new[] { "صورة وجه بطاقة الرقم القومي مطلوبة." } });

            if (model.NationalIdBack is null || model.NationalIdBack.Length == 0)
                return BadRequest(new { success = false, errors = new[] { "صورة ظهر بطاقة الرقم القومي مطلوبة." } });

            frontPath = await VisitRequestUploads.TrySaveImageAsync(_env, model.NationalIdFront, "nid_front", ct);
            backPath  = await VisitRequestUploads.TrySaveImageAsync(_env, model.NationalIdBack,  "nid_back",  ct);

            if (frontPath is null || backPath is null)
                return BadRequest(new { success = false, errors = new[] { "تعذر حفظ صور البطاقة. استخدم صورة (jpg أو png أو webp) بحجم أقل من 5 ميجابايت." } });
        }

        // ── إنشاء سجل الزيارة ────────────────────────────────────────────────
        var row = new HomeCareService
        {
            RequestDate          = DateTime.UtcNow,
            ServiceLocation      = model.Address.Trim(),
            PatientId            = userId,           // ← المريض المسجّل
            ServiceRequestId     = null,
            Status               = HomeCareServiceStatuses.Pending,
            IsDeleted            = false,
            CreatedBy            = userId,
            ProviderType         = type,
            ProviderId           = model.ProviderId,
            RequesterFullName    = model.FullName.Trim(),
            RequesterPhone       = model.Phone.Trim(),
            RequesterEmail       = model.Email.Trim(),
            RequestedVisitAt     = model.RequestedVisitAt,
            NationalIdFrontImage = frontPath,
            NationalIdBackImage  = backPath,
            MedicalCondition     = model.Condition.Trim(),
            AdditionalNotes      = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
        };

        _db.HomeCareServices.Add(row);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Patient {UserId} submitted visit request {RequestId} for {ProviderType} {ProviderId}",
            userId, row.Id, type, model.ProviderId);

        return Json(new { success = true, requestId = row.Id });
    }

    // ── Helper ───────────────────────────────────────────────────────────────
    private bool TryGetUserId(out int userId)
    {
        var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(str, out userId);
    }
}