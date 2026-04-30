using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ziarah.Areas.Patient.Models;

/// <summary>
/// نموذج إرسال طلب زيارة من المريض المسجّل.
/// يختلف عن نموذج Guest بأن بيانات المريض (الاسم/الهاتف/الإيميل/البطاقة)
/// تُجلب من الحساب تلقائياً ما لم يختر "بيانات جديدة".
/// </summary>
public class PatientVisitRequestFormModel
{
    // ── بيانات مقدم الخدمة ──────────────────────────────────────────────────
    [Required(ErrorMessage = "نوع مقدم الخدمة مطلوب.")]
    [RegularExpression("^(doctor|nurse)$", ErrorMessage = "نوع مقدم الخدمة غير صالح.")]
    public string ProviderType { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "معرف مقدم الخدمة غير صالح.")]
    public int ProviderId { get; set; }

    // ── هل يستخدم بيانات الحساب؟ ────────────────────────────────────────────
    /// <summary>true = يستخدم بيانات الحساب ، false = بيانات جديدة</summary>
    public bool UseAccount { get; set; } = true;

    // ── بيانات مقدم الطلب (مطلوبة دائماً، تُملأ من الحساب أو يدوياً) ──────
    [Required(ErrorMessage = "الاسم بالكامل مطلوب.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "الاسم يجب أن يكون بين 3 و 200 حرفاً.")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "رقم الهاتف مطلوب.")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "أدخل رقم موبايل مصري صحيح (11 رقماً).")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صالحة.")]
    [StringLength(256)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "العنوان مطلوب.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "العنوان يجب أن يكون بين 5 و 500 حرفاً.")]
    public string Address { get; set; } = null!;

    // ── الموعد والحالة ───────────────────────────────────────────────────────
    [Required(ErrorMessage = "موعد الزيارة مطلوب.")]
    public DateTime? RequestedVisitAt { get; set; }

    [Required(ErrorMessage = "الحالة المرضية مطلوبة.")]
    [StringLength(2000, MinimumLength = 2, ErrorMessage = "صف الحالة المرضية (حتى 2000 حرف).")]
    public string Condition { get; set; } = null!;

    [StringLength(4000)]
    public string? Notes { get; set; }

    // ── صور البطاقة ─────────────────────────────────────────────────────────
    /// <summary>صورة وجه البطاقة (مطلوبة إذا لم تكن محفوظة مسبقاً)</summary>
    public IFormFile? NationalIdFront { get; set; }

    /// <summary>صورة ظهر البطاقة (مطلوبة إذا لم تكن محفوظة مسبقاً)</summary>
    public IFormFile? NationalIdBack { get; set; }

    /// <summary>مسار صورة وجه البطاقة المحفوظة مسبقاً في الحساب</summary>
    public string? SavedNidFront { get; set; }

    /// <summary>مسار صورة ظهر البطاقة المحفوظة مسبقاً في الحساب</summary>
    public string? SavedNidBack { get; set; }
}