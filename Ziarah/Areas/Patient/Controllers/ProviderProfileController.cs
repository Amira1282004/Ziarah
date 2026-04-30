using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ziarah.Areas.Patient.Models;
using Ziarah.Data;

namespace Ziarah.Areas.Patient.Controllers;

[Area("Patient")]
[Authorize(Roles = "Patient")]
public sealed class ProviderProfileController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ProviderProfileController> _logger;

    public ProviderProfileController(
        ApplicationDbContext db,
        ILogger<ProviderProfileController> logger)
    {
        _db     = db;
        _logger = logger;
    }

    // ── GET: /Patient/ProviderProfile?id=5&type=doctor ────────────────────────
    public async Task<IActionResult> ProviderProfile(
        int    id,
        string type = "doctor",
        CancellationToken ct = default)
    {
        // هوية المريض
        if (!TryGetUserId(out var userId))
            return RedirectToAction("Login", "Login", new { area = "Identity" });

        // بيانات المريض المسجّل
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

        // ── بناء ViewModel حسب النوع ─────────────────────────────────────────
        PatientProviderProfileViewModel vm;

        if (string.Equals(type, "nurse", StringComparison.OrdinalIgnoreCase))
        {
            var nurse = await _db.Nurses
                .AsNoTracking()
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted, ct);

            if (nurse is null) return NotFound();

            vm = new PatientProviderProfileViewModel
            {
                Id              = nurse.Id,
                Type            = "nurse",
                FullName        = nurse.User.FullName,
                Specialty       = "تمريض منزلي",
                Bio             = "مقدم خدمة تمريض منزلي لمتابعة الحالات المزمنة والطارئة داخل المنزل.",
                Price           = 250,
                Rating          = 4.3m,
                ExperienceYears = 5,
                TotalRatings    = 30,
                ImageUrl        = string.IsNullOrWhiteSpace(nurse.User?.Photo)
                                      ? string.Empty
                                      : nurse.User.Photo!,
                Address         = "بني سويف والفيوم"
            };
        }
        else
        {
            var doctor = await _db.Doctors
                .AsNoTracking()
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, ct);

            if (doctor is null) return NotFound();

            vm = new PatientProviderProfileViewModel
            {
                Id              = doctor.Id,
                Type            = "doctor",
                FullName        = doctor.User.FullName,
                Specialty       = doctor.Specialization.Name,
                Bio             = string.IsNullOrWhiteSpace(doctor.Bio)
                                      ? $"طبيب متخصص في {doctor.Specialization.Name} يقدم خدمة الزيارات المنزلية."
                                      : doctor.Bio,
                Price           = doctor.ConsultationPrice,
                Rating          = doctor.Rating,
                ExperienceYears = doctor.ExperienceYears,
                TotalRatings    = doctor.TotalRatings,
                ImageUrl        = string.IsNullOrWhiteSpace(doctor.User.Photo)
                                      ? string.Empty
                                      : doctor.User.Photo!,
                Address         = "بني سويف والفيوم",
                Reviews         = await GetReviewsAsync(doctor.Id, "doctor", ct)
            };
        }

        // حقن بيانات المريض في ViewModel
        vm.PatientFullName           = $"{user.FirstName} {user.LastName}".Trim();
        vm.PatientPhone              = user.PhoneNumber ?? string.Empty;
        vm.PatientEmail              = user.Email       ?? string.Empty;
        vm.PatientNationalIdFrontUrl = user.NationalIdFrontImage;
        vm.PatientNationalIdBackUrl  = user.NationalIdBackImage;

        return View(vm);
    }

    // ── POST: /Patient/ProviderProfile/SubmitReview ───────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReview(
        [FromForm] SubmitReviewFormModel model,
        CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { success = false, message = "يجب تسجيل الدخول أولاً." });

        if (model.Stars < 1 || model.Stars > 5)
            return BadRequest(new { success = false, message = "التقييم يجب أن يكون بين 1 و 5 نجوم." });

        if (string.IsNullOrWhiteSpace(model.Comment))
            return BadRequest(new { success = false, message = "يرجى كتابة تعليقك قبل النشر." });

        var type = (model.ProviderType ?? string.Empty).Trim().ToLowerInvariant();
        if (type is not ("doctor" or "nurse"))
            return BadRequest(new { success = false, message = "نوع مقدم الخدمة غير صالح." });

        // ── التحقق من وجود مقدم الخدمة وتحديث تقييمه ──────────────────────
        if (type == "doctor")
        {
            var doctor = await _db.Doctors
                .FirstOrDefaultAsync(d => d.Id == model.ProviderId && !d.IsDeleted, ct);

            if (doctor is null)
                return NotFound(new { success = false, message = "الطبيب غير موجود." });

            // إعادة حساب المتوسط
            var newTotal  = doctor.TotalRatings + 1;
            var newRating = ((doctor.Rating * doctor.TotalRatings) + model.Stars) / newTotal;

            doctor.TotalRatings = newTotal;
            doctor.Rating       = Math.Round(newRating, 1);
        }
        else
        {
            var nurse = await _db.Nurses
                .FirstOrDefaultAsync(n => n.Id == model.ProviderId && !n.IsDeleted, ct);

            if (nurse is null)
                return NotFound(new { success = false, message = "الممرض غير موجود." });

            // (يمكن إضافة Rating للممرض لاحقاً إن وُجد العمود)
        }

        // ── حفظ التقييم (إن كان لديك جدول Reviews) ─────────────────────────
        // مثال:
        // _db.Reviews.Add(new Review
        // {
        //     ProviderId   = model.ProviderId,
        //     ProviderType = type,
        //     ReviewerId   = userId,
        //     Stars        = model.Stars,
        //     Comment      = model.Comment.Trim(),
        //     CreatedOn    = DateTime.UtcNow
        // });

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Patient {UserId} rated {ProviderType} {ProviderId} with {Stars} stars",
            userId, type, model.ProviderId, model.Stars);

        return Json(new { success = true, message = "تم نشر تقييمك بنجاح ✓" });
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    private bool TryGetUserId(out int userId)
    {
        var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(str, out userId);
    }

    /// <summary>
    /// جلب التقييمات من قاعدة البيانات.
    /// إن لم يكن لديك جدول Reviews بعد، أرجع قائمة فارغة أو بيانات ثابتة.
    /// </summary>
    private Task<List<ProviderReviewItem>> GetReviewsAsync(
        int providerId, string providerType, CancellationToken ct)
    {
        // ── فعّل هذا الكود بعد إنشاء جدول Reviews ────────────────────────────
        // return _db.Reviews
        //     .AsNoTracking()
        //     .Where(r => r.ProviderId == providerId && r.ProviderType == providerType && !r.IsDeleted)
        //     .OrderByDescending(r => r.CreatedOn)
        //     .Take(20)
        //     .Select(r => new ProviderReviewItem
        //     {
        //         Id           = r.Id,
        //         ReviewerName = r.Reviewer.FullName,
        //         ReviewerPhoto = r.Reviewer.Photo,
        //         Stars        = r.Stars,
        //         Comment      = r.Comment,
        //         CreatedOn    = r.CreatedOn
        //     })
        //     .ToListAsync(ct);

        // بيانات ثابتة مؤقتة
        var list = new List<ProviderReviewItem>
        {
            new() { Id = 1, ReviewerName = "علي أحمد إبراهيم",  Stars = 4, Comment = "دكتور ممتاز، متعاون جداً وشرح الحالة بالتفصيل",    CreatedOn = DateTime.UtcNow.AddDays(-3) },
            new() { Id = 2, ReviewerName = "محمد عبد الرحمن",   Stars = 5, Comment = "خدمة ممتازة وسريعة، وصل في الموعد المحدد تماماً",  CreatedOn = DateTime.UtcNow.AddDays(-7) },
            new() { Id = 3, ReviewerName = "سارة محمود",         Stars = 4, Comment = "أنصح به بشدة، تشخيص دقيق ومعاملة محترمة",          CreatedOn = DateTime.UtcNow.AddDays(-10) },
            new() { Id = 4, ReviewerName = "أحمد السيد",         Stars = 5, Comment = "تجربة رائعة، الطبيب محترف ومهتم بالمريض",           CreatedOn = DateTime.UtcNow.AddDays(-15) },
            new() { Id = 5, ReviewerName = "فاطمة علي",          Stars = 3, Comment = "جيد، لكن تأخر قليلاً عن الموعد المحدد",            CreatedOn = DateTime.UtcNow.AddDays(-20) },
        };

        return Task.FromResult(list);
    }
}