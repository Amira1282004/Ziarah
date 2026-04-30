using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ziarah.Areas.Identity.Models;
using Ziarah.Data;
using Ziarah.Models;
using Ziarah.Services;
using PatientAccount = Ziarah.Models.Patient;

namespace Ziarah.Areas.Identity.Controllers;

[Area("Identity")]
[AllowAnonymous]
public sealed class RegisterController : Controller
{
    private const string SessionKey = "Ziarah.PendingRegistration";

    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IEmailSender _email;
    private readonly IPasswordHasher<object> _passwordHasher;
    private readonly IOptions<RegistrationOptions> _regOptions;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(
        ApplicationDbContext db,
        IWebHostEnvironment env,
        IEmailSender email,
        IPasswordHasher<object> passwordHasher,
        IOptions<RegistrationOptions> regOptions,
        ILogger<RegisterController> logger)
    {
        _db = db;
        _env = env;
        _email = email;
        _passwordHasher = passwordHasher;
        _regOptions = regOptions;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        ViewBag.Specializations = await _db.Specializations
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Name)
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        [FromForm] string role,
        [FromForm] string firstName,
        [FromForm] string lastName,
        [FromForm] string phone,
        [FromForm] string email,
        [FromForm] string password,
        [FromForm] string? gender,
        [FromForm] string? dateOfBirth,
        [FromForm] int? specializationId,
        [FromForm] decimal? consultationPrice,
        [FromForm] int? experienceYears,
        [FromForm] bool hasInsurance,
        IFormFile nationalIdFront,
        IFormFile nationalIdBack,
        IFormFile? professionalLicense,
        IFormFile? insuranceDoc,
        CancellationToken ct)
    {
        role = (role ?? "patient").Trim().ToLowerInvariant();
        firstName = firstName?.Trim() ?? string.Empty;
        lastName = lastName?.Trim() ?? string.Empty;
        phone = phone?.Trim() ?? string.Empty;
        email = email?.Trim() ?? string.Empty;
        gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim();
        dateOfBirth = string.IsNullOrWhiteSpace(dateOfBirth) ? null : dateOfBirth.Trim();

        var err = ValidateRegistration(role, firstName, lastName, phone, email, password, specializationId, consultationPrice, experienceYears, hasInsurance, nationalIdFront, nationalIdBack, professionalLicense, insuranceDoc);
        if (err is not null)
        {
            ModelState.AddModelError(string.Empty, err);
            ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
            return View();
        }

        var normalizedEmail = email.ToUpperInvariant();
        var normalizedUserName = normalizedEmail;

        if (await _db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted, ct))
        {
            ModelState.AddModelError(string.Empty, "البريد الإلكتروني مسجّل مسبقاً.");
            ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
            return View();
        }

        if (await _db.Users.AnyAsync(u => u.NormalizedUserName == normalizedUserName && !u.IsDeleted, ct))
        {
            ModelState.AddModelError(string.Empty, "اسم المستخدم (البريد) مستخدم مسبقاً.");
            ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
            return View();
        }

        if (role == "doctor" && specializationId is int sid &&
            !await _db.Specializations.AnyAsync(s => s.Id == sid && !s.IsDeleted, ct))
        {
            ModelState.AddModelError(string.Empty, "التخصص غير صالح.");
            ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
            return View();
        }

        var pendingId = Guid.NewGuid().ToString("N");
        var frontUrl = await RegistrationUploads.TrySaveImageAsync(_env, nationalIdFront, pendingId, "nid_front", ct);
        var backUrl = await RegistrationUploads.TrySaveImageAsync(_env, nationalIdBack, pendingId, "nid_back", ct);
        if (frontUrl is null || backUrl is null)
        {
            ModelState.AddModelError(string.Empty, "تعذّر حفظ صور بطاقة الرقم القومي. تأكد من الصيغة والحجم.");
            ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
            return View();
        }

        string? licenseUrl = null;
        if (role is "doctor" or "nurse")
        {
            if (professionalLicense is null || professionalLicense.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "صورة شهادة المزاولة مطلوبة.");
                ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
                return View();
            }

            licenseUrl = await RegistrationUploads.TrySaveImageAsync(_env, professionalLicense, pendingId, "license", ct);
            if (licenseUrl is null)
            {
                ModelState.AddModelError(string.Empty, "تعذّر حفظ صورة الشهادة.");
                ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
                return View();
            }
        }

        string? insuranceUrl = null;
        if (role == "patient" && hasInsurance)
        {
            if (insuranceDoc is null || insuranceDoc.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "صورة كارنيه التأمين مطلوبة عند اختيار وجود تأمين.");
                ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
                return View();
            }

            insuranceUrl = await RegistrationUploads.TrySaveImageAsync(_env, insuranceDoc, pendingId, "insurance", ct);
            if (insuranceUrl is null)
            {
                ModelState.AddModelError(string.Empty, "تعذّر حفظ صورة التأمين.");
                ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
                return View();
            }
        }

        var code = Random.Shared.Next(0, 10000).ToString("D4");
        var expiry = TimeSpan.FromMinutes(Math.Clamp(_regOptions.Value.CodeExpiryMinutes, 1, 60));

        var state = new PendingRegistrationState
        {
            PendingId = pendingId,
            Role = role,
            Email = email,
            NormalizedEmail = normalizedEmail,
            UserName = email,
            NormalizedUserName = normalizedUserName,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            PasswordHash = _passwordHasher.HashPassword(null!, password),
            DateOfBirthIso = dateOfBirth,
            Gender = gender,
            NationalIdFrontUrl = frontUrl,
            NationalIdBackUrl = backUrl,
            LicenseUrl = licenseUrl,
            SpecializationId = role == "doctor" ? specializationId : null,
            ConsultationPrice = role == "doctor" ? consultationPrice : null,
            ExperienceYears = role == "doctor" ? experienceYears : null,
            PatientHasInsurance = role == "patient" && hasInsurance,
            PatientInsuranceImageUrl = insuranceUrl,
            ExpectedCode = code,
            CodeExpiresUtc = DateTimeOffset.UtcNow.Add(expiry)
        };

        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(state));

        try
        {
            await _email.SendRegistrationVerificationCodeAsync(email, code, ct);
            TempData["VerifyMessage"] = "تم إرسال كود التأكيد إلى بريدك.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل إرسال بريد التأكيد");
            ModelState.AddModelError(string.Empty, $"تعذّر إرسال رسالة التأكيد. ({ex.Message})");
            ViewBag.Specializations = await _db.Specializations.AsNoTracking().Where(s => !s.IsDeleted).OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync();
            return View();
        }

        return RedirectToAction(nameof(VerifyCode));
    }

    [HttpGet]
    public IActionResult VerifyCode()
    {
        var state = ReadPending();
        if (state is null)
        {
            return RedirectToAction(nameof(Register));
        }

        ViewBag.MaskedEmail = MaskEmail(state.Email);
        ViewBag.CodeExpiresMs = state.CodeExpiresUtc.ToUnixTimeMilliseconds();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyCode([FromForm] string code, CancellationToken ct)
    {
        var state = ReadPending();
        if (state is null)
        {
            return RedirectToAction(nameof(Register));
        }

        ViewBag.MaskedEmail = MaskEmail(state.Email);
        ViewBag.CodeExpiresMs = state.CodeExpiresUtc.ToUnixTimeMilliseconds();

        if (DateTimeOffset.UtcNow > state.CodeExpiresUtc)
        {
            ModelState.AddModelError(string.Empty, "انتهت صلاحية الكود. اطلب إرسال كود جديد.");
            return View();
        }

        var entered = (code ?? string.Empty).Trim().Replace(" ", string.Empty);
        if (!string.Equals(entered, state.ExpectedCode, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "الكود غير صحيح. تحقق من الرسالة وحاول مرة أخرى.");
            return View();
        }

        if (await _db.Users.AnyAsync(u => u.NormalizedEmail == state.NormalizedEmail && !u.IsDeleted, ct))
        {
            HttpContext.Session.Remove(SessionKey);
            ModelState.AddModelError(string.Empty, "هذا البريد أصبح مسجّلاً. يمكنك تسجيل الدخول.");
            return View();
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var signedInUserId = 0;
        var signedInUserName = string.Empty;
        var signedInEmail = string.Empty;
        var roleClaimLocal = string.Empty;
        try
        {
            var createdBy = await EnsureCreatorUserAsync(ct);
            var nextUserId = (await _db.Users.MaxAsync(u => (int?)u.Id, ct) ?? 0) + 1;

            DateOnly? dob = null;
            if (!string.IsNullOrWhiteSpace(state.DateOfBirthIso) && DateOnly.TryParse(state.DateOfBirthIso, out var parsed))
            {
                dob = parsed;
            }

            var user = new User
            {
                Id = nextUserId,
                FirstName = state.FirstName,
                LastName = state.LastName,
                UserName = state.UserName,
                NormalizedUserName = state.NormalizedUserName,
                Email = state.Email,
                NormalizedEmail = state.NormalizedEmail,
                EmailConfirmed = true,
                PasswordHash = state.PasswordHash,
                SecurityStamp = Guid.NewGuid().ToString("N"),
                ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                PhoneNumber = state.Phone,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                DateOfBirth = dob,
                Gender = state.Gender,
                Photo = null,
                NationalIdFrontImage = state.NationalIdFrontUrl,
                NationalIdBackImage = state.NationalIdBackUrl,
                Status = 1,
                IsDeleted = false,
                CreatedBy = createdBy
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            if (state.Role == "patient")
            {
                int? insuranceId = null;
                if (state.PatientHasInsurance)
                {
                    insuranceId = await _db.Insurances.AsNoTracking().Where(i => !i.IsDeleted && i.IsActive).OrderBy(i => i.Id).Select(i => (int?)i.Id).FirstOrDefaultAsync(ct);
                }

                _db.Patients.Add(new PatientAccount
                {
                    UserId = user.Id,
                    InsuranceId = insuranceId,
                    HasInsurance = state.PatientHasInsurance,
                    InsuranceImage = state.PatientInsuranceImageUrl,
                    IsDeleted = false,
                    CreatedBy = createdBy
                });
                roleClaimLocal = "Patient";
            }
            else if (state.Role == "doctor")
            {
                _db.Doctors.Add(new Doctor
                {
                    UserId = user.Id,
                    SpecializationId = state.SpecializationId!.Value,
                    Bio = null,
                    ProfessionalLicenseImage = state.LicenseUrl!,
                    ConsultationPrice = state.ConsultationPrice!.Value,
                    ExperienceYears = state.ExperienceYears ?? 0,
                    Rating = 0,
                    TotalRatings = 0,
                    IsVerified = false,
                    IsDeleted = false,
                    CreatedBy = createdBy
                });
                roleClaimLocal = "Doctor";
            }
            else
            {
                _db.Nurses.Add(new Nurse
                {
                    UserId = user.Id,
                    ProfessionalLicenseImage = state.LicenseUrl!,
                    HomeCareServiceId = null,
                    IsDeleted = false,
                    CreatedBy = createdBy
                });
                roleClaimLocal = "Nurse";
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            signedInUserId = user.Id;
            signedInUserName = user.UserName;
            signedInEmail = user.Email;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "فشل حفظ المستخدم في قاعدة البيانات");
            var details = ex.InnerException?.Message ?? ex.Message;
            ModelState.AddModelError(string.Empty, $"تعذّر إكمال التسجيل. ({details})");
            return View();
        }

        HttpContext.Session.Remove(SessionKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, signedInUserId.ToString()),
            new(ClaimTypes.Name, signedInUserName),
            new(ClaimTypes.Email, signedInEmail),
            new(ClaimTypes.Role, roleClaimLocal)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        if (roleClaimLocal == "Patient")
        {
            TempData["AccountCreatedSuccess"] = "تم إنشاء حسابك بنجاح.";
            return RedirectToAction("Home", "Home", new { area = "Patient" });
        }

        TempData["AccountCreatedSuccess"] = "تم إنشاء حسابك بنجاح.";
        return RedirectToAction("Dashboard", "Dashboard", new { area = "Provider" });
    }

    private async Task<int> EnsureCreatorUserAsync(CancellationToken ct)
    {
        var adminId = await _db.Users
            .Where(u => !u.IsDeleted && u.UserName == "admin")
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(ct);
        if (adminId.HasValue)
        {
            return adminId.Value;
        }

        var anyActiveUserId = await _db.Users
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.Id)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(ct);
        if (anyActiveUserId.HasValue)
        {
            return anyActiveUserId.Value;
        }

        var bootstrapId = (await _db.Users.MaxAsync(u => (int?)u.Id, ct) ?? 0) + 1;
        var bootstrap = new User
        {
            Id = bootstrapId,
            FirstName = "System",
            LastName = "Creator",
            UserName = "system.creator",
            NormalizedUserName = "SYSTEM.CREATOR",
            Email = "system.creator@ziarah.local",
            NormalizedEmail = "SYSTEM.CREATOR@ZIARAH.LOCAL",
            EmailConfirmed = true,
            PasswordHash = "SYSTEM_BOOTSTRAP_HASH",
            SecurityStamp = Guid.NewGuid().ToString("N"),
            ConcurrencyStamp = Guid.NewGuid().ToString("N"),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            Photo = null,
            NationalIdFrontImage = "/image/docs/system/default-front.jpg",
            NationalIdBackImage = "/image/docs/system/default-back.jpg",
            Status = 1,
            IsDeleted = false,
            CreatedBy = bootstrapId
        };

        _db.Users.Add(bootstrap);
        await _db.SaveChangesAsync(ct);
        return bootstrapId;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendCode(CancellationToken ct)
    {
        var state = ReadPending();
        if (state is null)
        {
            return RedirectToAction(nameof(Register));
        }

        var cooldown = TimeSpan.FromSeconds(Math.Clamp(_regOptions.Value.ResendCooldownSeconds, 10, 600));
        if (state.LastResendUtc is { } last && DateTimeOffset.UtcNow - last < cooldown)
        {
            var waitSec = (int)Math.Ceiling((cooldown - (DateTimeOffset.UtcNow - last)).TotalSeconds);
            TempData["VerifyMessage"] = $"انتظر {waitSec} ثانية قبل طلب كود جديد.";
            return RedirectToAction(nameof(VerifyCode));
        }

        var code = Random.Shared.Next(0, 10000).ToString("D4");
        var expiry = TimeSpan.FromMinutes(Math.Clamp(_regOptions.Value.CodeExpiryMinutes, 1, 60));
        state.ExpectedCode = code;
        state.CodeExpiresUtc = DateTimeOffset.UtcNow.Add(expiry);
        state.LastResendUtc = DateTimeOffset.UtcNow;
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(state));

        try
        {
            await _email.SendRegistrationVerificationCodeAsync(state.Email, code, ct);
            TempData["VerifyMessage"] = "تم إرسال كود جديد إلى بريدك.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل إعادة إرسال الكود");
            TempData["VerifyMessage"] = "تعذّر إرسال الكود. حاول لاحقاً.";
        }

        return RedirectToAction(nameof(VerifyCode));
    }

    private PendingRegistrationState? ReadPending()
    {
        var json = HttpContext.Session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<PendingRegistrationState>(json);
        }
        catch
        {
            return null;
        }
    }

    private static string MaskEmail(string email)
    {
        var at = email.IndexOf('@');
        if (at <= 1)
        {
            return "***@***";
        }

        var visible = email[..Math.Min(2, at)];
        return visible + "***" + email[at..];
    }

    private static string? ValidateRegistration(
        string role,
        string firstName,
        string lastName,
        string phone,
        string email,
        string password,
        int? specializationId,
        decimal? consultationPrice,
        int? experienceYears,
        bool hasInsurance,
        IFormFile nationalIdFront,
        IFormFile nationalIdBack,
        IFormFile? professionalLicense,
        IFormFile? insuranceDoc)
    {
        if (role is not ("patient" or "doctor" or "nurse"))
        {
            return "نوع الحساب غير معروف.";
        }

        if (firstName.Length < 2 || firstName.Length > 100 || lastName.Length < 2 || lastName.Length > 100)
        {
            return "الاسم الأول والأخير مطلوبان (حرفان على الأقل).";
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^01[0125][0-9]{8}$"))
        {
            return "رقم الهاتف يجب أن يكون مصرياً بصيغة 01xxxxxxxxx (11 رقماً).";
        }

        if (string.IsNullOrWhiteSpace(email) || email.Length > 256 || !email.Contains('@', StringComparison.Ordinal))
        {
            return "البريد الإلكتروني غير صالح.";
        }

        if (string.IsNullOrEmpty(password) || password.Length < 8
            || !password.Any(char.IsUpper) || !password.Any(char.IsLower)
            || !password.Any(char.IsDigit) || !password.Any(c => "!@#$%^&*".Contains(c, StringComparison.Ordinal)))
        {
            return "كلمة المرور يجب أن تكون 8 أحرف على الأقل وتشمل حرفاً كبيراً وصغيراً ورقماً ورمزاً من: !@#$%^&*";
        }

        if (nationalIdFront is null || nationalIdFront.Length == 0 || nationalIdBack is null || nationalIdBack.Length == 0)
        {
            return "صورة وجه وظهر بطاقة الرقم القومي مطلوبتان.";
        }

        if (role == "doctor")
        {
            if (!specializationId.HasValue || specializationId.Value < 1)
            {
                return "اختر التخصص.";
            }

            if (!consultationPrice.HasValue || consultationPrice.Value < 1 || consultationPrice.Value > 100_000)
            {
                return "سعر الكشف مطلوب وبقيمة معقولة.";
            }

            if (!experienceYears.HasValue || experienceYears.Value < 0 || experienceYears.Value > 60)
            {
                return "سنوات الخبرة مطلوبة (0–60).";
            }

            if (professionalLicense is null || professionalLicense.Length == 0)
            {
                return "صورة شهادة الطب مطلوبة.";
            }
        }

        if (role == "nurse")
        {
            if (professionalLicense is null || professionalLicense.Length == 0)
            {
                return "صورة شهادة التمريض مطلوبة.";
            }
        }

        if (role == "patient" && hasInsurance)
        {
            if (insuranceDoc is null || insuranceDoc.Length == 0)
            {
                return "ارفع صورة كارنيه التأمين عند تفعيل خيار التأمين.";
            }
        }

        return null;
    }
}
