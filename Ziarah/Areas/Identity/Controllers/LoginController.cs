using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ziarah.Data;
using Ziarah.Services;

namespace Ziarah.Areas.Identity.Controllers;

[Area("Identity")]
[AllowAnonymous]
public sealed class LoginController : Controller
{
    // ── مفاتيح TempData ──────────────────────────────────────────────────────
    private const string ResetTokenKey   = "Ziarah.ResetToken";
    private const string ResetEmailKey   = "Ziarah.ResetEmail";
    private const string ResetExpiryKey  = "Ziarah.ResetExpiry";

    private readonly ApplicationDbContext      _db;
    private readonly IPasswordHasher<object>   _hasher;
    private readonly IEmailSender              _email;
    private readonly ILogger<LoginController>  _logger;

    public LoginController(
        ApplicationDbContext     db,
        IPasswordHasher<object>  hasher,
        IEmailSender             email,
        ILogger<LoginController> logger)
    {
        _db     = db;
        _hasher = hasher;
        _email  = email;
        _logger = logger;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  LOGIN
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        [FromForm] string email,
        [FromForm] string password,
        [FromForm] bool   rememberMe,
        CancellationToken ct)
    {
        email    = email?.Trim()    ?? string.Empty;
        password = password?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "يرجى إدخال البريد الإلكتروني وكلمة المرور.");
            return View();
        }

        var normalized = email.ToUpperInvariant();
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized && !u.IsDeleted, ct);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
            return View();
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
        {
            ModelState.AddModelError(string.Empty, "الحساب موقوف مؤقتاً. حاول لاحقاً.");
            return View();
        }

        var verifyResult = _hasher.VerifyHashedPassword(null!, user.PasswordHash, password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            // زيادة AccessFailedCount
            await _db.Users
                .Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.AccessFailedCount, u => u.AccessFailedCount + 1), ct);

            ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
            return View();
        }

        // إعادة تعيين AccessFailedCount عند النجاح
        await _db.Users
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.AccessFailedCount, 0), ct);

        // تحديد الدور
        var roleClaim = await DetermineRoleAsync(user.Id, ct);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,           user.UserName),
            new(ClaimTypes.Email,          user.Email),
            new(ClaimTypes.Role,           roleClaim)
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = rememberMe });

        return roleClaim switch
        {
            "Doctor" or "Nurse" => RedirectToAction("Dashboard", "Dashboard", new { area = "Provider" }),
            _                   => RedirectToAction("Home",      "Home",      new { area = "Patient"  })
        };
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  FORGOT PASSWORD
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        [FromForm] string email,
        CancellationToken ct)
    {
        email = email?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            ModelState.AddModelError(string.Empty, "يرجى إدخال بريد إلكتروني صالح.");
            return View();
        }

        var normalized = email.ToUpperInvariant();
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized && !u.IsDeleted, ct);

        // لا نُفصح إن كان البريد موجوداً أم لا (أمان)
        if (user is null)
        {
            TempData["ForgotMessage"] = "إذا كان البريد مسجّلاً، ستصلك رسالة قريباً.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        // توليد توكن آمن
        var token  = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                             .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var expiry = DateTimeOffset.UtcNow.AddMinutes(30);

        // حفظ في Session
        HttpContext.Session.SetString(ResetTokenKey,  token);
        HttpContext.Session.SetString(ResetEmailKey,  normalized);
        HttpContext.Session.SetString(ResetExpiryKey, expiry.ToUnixTimeSeconds().ToString());

        // بناء الرابط
        var resetLink = Url.Action(
            nameof(ResetPassword), "Login",
            new { area = "Identity", token },
            Request.Scheme)!;

        try
        {
            await _email.SendPasswordResetAsync(user.Email, user.FirstName, resetLink, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل إرسال رسالة إعادة تعيين كلمة المرور");
            ModelState.AddModelError(string.Empty, "تعذّر إرسال الرسالة، حاول لاحقاً.");
            return View();
        }

        TempData["ForgotMessage"] = "تم إرسال رابط إعادة التعيين إلى بريدك. تحقق من صندوق الوارد أو Spam.";
        return RedirectToAction(nameof(ForgotPassword));
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  RESET PASSWORD
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet]
    public IActionResult ResetPassword([FromQuery] string token)
    {
        if (!ValidateResetToken(token))
        {
            TempData["ResetError"] = "الرابط غير صالح أو انتهت صلاحيته.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        ViewBag.Token = token;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        [FromForm] string token,
        [FromForm] string password,
        [FromForm] string confirmPassword,
        CancellationToken ct)
    {
        ViewBag.Token = token;

        if (!ValidateResetToken(token))
        {
            ModelState.AddModelError(string.Empty, "انتهت صلاحية الرابط. اطلب رابطاً جديداً.");
            return View();
        }

        // التحقق من كلمة المرور
        if (string.IsNullOrEmpty(password) || password.Length < 8
            || !password.Any(char.IsUpper) || !password.Any(char.IsLower)
            || !password.Any(char.IsDigit)
            || !password.Any(c => "!@#$%^&*".Contains(c, StringComparison.Ordinal)))
        {
            ModelState.AddModelError(string.Empty,
                "كلمة المرور يجب أن تكون 8 أحرف على الأقل وتشمل حرفاً كبيراً وصغيراً ورقماً ورمزاً من: !@#$%^&*");
            return View();
        }

        if (password != confirmPassword)
        {
            ModelState.AddModelError(string.Empty, "كلمتا المرور غير متطابقتين.");
            return View();
        }

        var normalizedEmail = HttpContext.Session.GetString(ResetEmailKey);
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            ModelState.AddModelError(string.Empty, "انتهت الجلسة. اطلب رابطاً جديداً.");
            return View();
        }

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted, ct);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "لم يُعثر على الحساب.");
            return View();
        }

        user.PasswordHash    = _hasher.HashPassword(null!, password);
        user.SecurityStamp   = Guid.NewGuid().ToString("N");
        user.AccessFailedCount = 0;
        user.LockoutEnd      = null;

        await _db.SaveChangesAsync(ct);

        // مسح توكن الجلسة
        HttpContext.Session.Remove(ResetTokenKey);
        HttpContext.Session.Remove(ResetEmailKey);
        HttpContext.Session.Remove(ResetExpiryKey);

        TempData["LoginMessage"] = "تم تغيير كلمة المرور بنجاح. يمكنك تسجيل الدخول الآن.";
        return RedirectToAction(nameof(Login));
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  LOGOUT
    // ══════════════════════════════════════════════════════════════════════════

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  HELPERS
    // ══════════════════════════════════════════════════════════════════════════

    private async Task<string> DetermineRoleAsync(int userId, CancellationToken ct)
    {
        if (await _db.Doctors.AnyAsync(d => d.UserId == userId && !d.IsDeleted, ct))
            return "Doctor";
        if (await _db.Nurses.AnyAsync(n => n.UserId == userId && !n.IsDeleted, ct))
            return "Nurse";
        return "Patient";
    }

    private bool ValidateResetToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;

        var sessionToken = HttpContext.Session.GetString(ResetTokenKey);
        if (sessionToken != token) return false;

        var expiryStr = HttpContext.Session.GetString(ResetExpiryKey);
        if (!long.TryParse(expiryStr, out var expiryUnix)) return false;

        return DateTimeOffset.UtcNow <= DateTimeOffset.FromUnixTimeSeconds(expiryUnix);
    }
}