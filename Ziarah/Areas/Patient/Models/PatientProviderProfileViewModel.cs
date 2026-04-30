namespace Ziarah.Areas.Patient.Models;

/// <summary>
/// ViewModel لصفحة بروفايل مقدم الخدمة - Patient Area
/// يُضاف إليه بيانات المريض المسجّل لملء نموذج الحجز تلقائياً
/// </summary>
public class PatientProviderProfileViewModel
{
    // ── بيانات مقدم الخدمة ───────────────────────────────────────────────────
    public int     Id              { get; set; }
    public string  Type            { get; set; } = "doctor";
    public string  FullName        { get; set; } = string.Empty;
    public string  Specialty       { get; set; } = string.Empty;
    public string  Bio             { get; set; } = string.Empty;
    public decimal Price           { get; set; }
    public decimal Rating          { get; set; }
    public int     ExperienceYears { get; set; }
    public int     TotalRatings    { get; set; }
    public string  ImageUrl        { get; set; } = string.Empty;
    public string  Address         { get; set; } = "بني سويف";

    // ── بيانات المريض المسجّل (تُملأ تلقائياً في نموذج الحجز) ─────────────
    public string  PatientFullName           { get; set; } = string.Empty;
    public string  PatientPhone              { get; set; } = string.Empty;
    public string  PatientEmail              { get; set; } = string.Empty;
    public string? PatientAddress            { get; set; }
    public string? PatientNationalIdFrontUrl { get; set; }
    public string? PatientNationalIdBackUrl  { get; set; }

    /// <summary>هل البطاقتان محفوظتان مسبقاً؟</summary>
    public bool HasSavedNid =>
        !string.IsNullOrWhiteSpace(PatientNationalIdFrontUrl) &&
        !string.IsNullOrWhiteSpace(PatientNationalIdBackUrl);

    // ── التقييمات ────────────────────────────────────────────────────────────
    public List<ProviderReviewItem> Reviews { get; set; } = new();
}

/// <summary>
/// بند تقييم واحد يُعرض في صفحة البروفايل
/// </summary>
public class ProviderReviewItem
{
    public int      Id          { get; set; }
    public string   ReviewerName { get; set; } = string.Empty;
    public string?  ReviewerPhoto { get; set; }
    public int      Stars        { get; set; }
    public string   Comment      { get; set; } = string.Empty;
    public DateTime CreatedOn    { get; set; }
}

/// <summary>
/// نموذج إضافة تقييم جديد (يُرسَل عبر AJAX)
/// </summary>
public class SubmitReviewFormModel
{
    public int    ProviderId   { get; set; }
    public string ProviderType { get; set; } = "doctor";
    public int    Stars        { get; set; }
    public string Comment      { get; set; } = string.Empty;
}