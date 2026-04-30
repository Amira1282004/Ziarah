using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ziarah.Areas.Patient.Models;
using Ziarah.Data;

namespace Ziarah.Areas.Patient.Controllers;

[Area("Patient")]
[Authorize(Roles = "Patient")]
public sealed class HomeController : Controller
{
    private readonly ApplicationDbContext    _db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task<IActionResult> Home(CancellationToken ct)
    {
        // ── هوية المريض ─────────────────────────────────────────────────────
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return RedirectToAction("Login", "Login", new { area = "Identity" });

        // ── بيانات المستخدم ──────────────────────────────────────────────────
        var user = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .Select(u => new
            {
                u.FirstName,
                u.LastName,
                u.Email,
                u.PhoneNumber,
                u.Photo,
                u.NationalIdFrontImage,
                u.NationalIdBackImage,
                // u.Address                   // إن وجد في جدول Users
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return RedirectToAction("Login", "Login", new { area = "Identity" });

        // ── بيانات المريض (تأمين) ────────────────────────────────────────────
        var patient = await _db.Patients
            .AsNoTracking()
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .Select(p => new { p.HasInsurance })
            .FirstOrDefaultAsync(ct);

        // ── إشعارات المريض (آخر 20 غير محذوفة) ─────────────────────────────
        var notifications = await _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedOn)
            .Take(20)
            .Select(n => new NotificationItem
            {
                Id        = n.Id,
                Title     = n.Title,
                Message   = n.Message,
                Type      = n.Type,
                IsRead    = n.IsRead,
                CreatedOn = n.CreatedOn
            })
            .ToListAsync(ct);

        // ── أعلى 8 أطباء تقييماً ─────────────────────────────────────────────
        var topDoctors = await (
            from d in _db.Doctors.AsNoTracking()
            join u in _db.Users.AsNoTracking()           on d.UserId          equals u.Id
            join s in _db.Specializations.AsNoTracking() on d.SpecializationId equals s.Id
            where !d.IsDeleted
            orderby d.Rating descending
            select new HomeProviderCard
            {
                Id       = d.Id,
                Type     = "doctor",
                Name     = u.FullName,
                Spec     = s.Name,
                Price    = d.ConsultationPrice,
                Rating   = d.Rating,
                ImageUrl = u.Photo
            })
            .Take(8)
            .ToListAsync(ct);

        foreach (var d in topDoctors)
            d.ProfileUrl = Url.Action("ProviderProfile", "ProviderProfile",
                               new { area = "Patient", id = d.Id }) ?? "#";

        // ── أعلى 8 ممرضين ────────────────────────────────────────────────────
        var topNurses = await (
            from n in _db.Nurses.AsNoTracking()
            join u in _db.Users.AsNoTracking() on n.UserId equals u.Id
            where !n.IsDeleted
            orderby n.Id
            select new HomeProviderCard
            {
                Id       = n.Id,
                Type     = "nurse",
                Name     = u.FullName,
                Spec     = "تمريض منزلي",
                Price    = 0,
                Rating   = 0,
                ImageUrl = u.Photo
            })
            .Take(8)
            .ToListAsync(ct);

        foreach (var n in topNurses)
            n.ProfileUrl = Url.Action("ProviderProfile", "ProviderProfile",
                               new { area = "Patient", id = n.Id, type = "nurse" }) ?? "#";

        // ── سجل الزيارات (آخر 20) ────────────────────────────────────────────
        var visits = await _db.HomeCareServices
            .AsNoTracking()
            .Where(h => !h.IsDeleted &&
                        (h.PatientId == userId || h.RequesterEmail == user.Email))
            .OrderByDescending(h => h.RequestDate)
            .Take(20)
            .ToListAsync(ct);

        var visitCards = new List<HomeVisitCard>();
        foreach (var v in visits)
        {
            string   providerName  = "—";
            string   providerSpec  = "—";
            string?  providerPhoto = null;
            decimal  price         = 0;

            if (v.ProviderType == "doctor" && v.ProviderId.HasValue)
            {
                var doc = await _db.Doctors.AsNoTracking()
                    .Include(d => d.User)
                    .Include(d => d.Specialization)
                    .FirstOrDefaultAsync(d => d.Id == v.ProviderId.Value, ct);
                if (doc is not null)
                {
                    providerName  = doc.User?.FullName ?? "طبيب";
                    providerSpec  = doc.Specialization?.Name ?? "—";
                    providerPhoto = doc.User?.Photo;
                    price         = doc.ConsultationPrice;
                }
            }
            else if (v.ProviderType == "nurse" && v.ProviderId.HasValue)
            {
                var nurse = await _db.Nurses.AsNoTracking()
                    .Include(n => n.User)
                    .FirstOrDefaultAsync(n => n.Id == v.ProviderId.Value, ct);
                if (nurse is not null)
                {
                    providerName  = nurse.User?.FullName ?? "ممرض";
                    providerSpec  = "تمريض منزلي";
                    providerPhoto = nurse.User?.Photo;
                }
            }

            var (statusEn, statusAr) = MapStatus(v.Status);
            visitCards.Add(new HomeVisitCard
            {
                Id            = v.Id,
                ProviderName  = providerName,
                ProviderSpec  = providerSpec,
                ProviderPhoto = providerPhoto,
                Status        = statusEn,
                StatusLabel   = statusAr,
                RequestedAt   = v.RequestedVisitAt ?? v.RequestDate,
                Address       = v.ServiceLocation ?? "—",
                Price         = price
            });
        }

        // ── ViewModel ────────────────────────────────────────────────────────
        var vm = new PatientHomeViewModel
        {
            FirstName          = user.FirstName,
            FullName           = $"{user.FirstName} {user.LastName}".Trim(),
            Email              = user.Email,
            Phone              = user.PhoneNumber ?? "—",
            PhotoUrl           = user.Photo,
            HasInsurance       = patient?.HasInsurance ?? false,
            // Address            = user.Address,
            NationalIdFrontUrl = user.NationalIdFrontImage,
            NationalIdBackUrl  = user.NationalIdBackImage,
            Notifications      = notifications,
            TopDoctors         = topDoctors,
            TopNurses          = topNurses,
            VisitHistory       = visitCards
        };

        // بيانات المريض للـ JS
        ViewBag.CurrentUser = System.Text.Json.JsonSerializer.Serialize(new
        {
            FullName           = vm.FullName,
            Phone              = vm.Phone,
            Email              = vm.Email,
            Address            = vm.Address ?? string.Empty,
            NationalIdFrontUrl = vm.NationalIdFrontUrl ?? string.Empty,
            NationalIdBackUrl  = vm.NationalIdBackUrl  ?? string.Empty
        });

        return View(vm);
    }

    // ── Mark notification as read (AJAX) ────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkNotificationRead(int id, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var notif = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, ct);

        if (notif is null) return NotFound();

        notif.IsRead  = true;
        notif.ReadAt  = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Ok();
    }

    // ── Mark all notifications as read (AJAX) ────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllNotificationsRead(CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt,  DateTime.UtcNow), ct);

        return Ok();
    }

    // ── Helper ───────────────────────────────────────────────────────────────
    private static (string en, string ar) MapStatus(int status) => status switch
    {
        1 => ("confirmed", "مؤكدة"),
        2 => ("completed", "منتهية"),
        3 => ("cancelled", "ملغاة"),
        _  => ("pending",  "قيد الانتظار")
    };
}