namespace Ziarah.Areas.Patient.Models;

/// <summary>
/// ViewModel لصفحة طلب زيارة - Patient Area
/// يُستخدم لعرض قائمة مقدمي الخدمة مع بيانات المريض المسجّل
/// </summary>
public class PatientRequestVisitViewModel
{
    // ── بيانات المريض (تُملأ تلقائياً من الحساب) ─────────────────────────────
    public string  FullName           { get; set; } = string.Empty;
    public string  Phone              { get; set; } = string.Empty;
    public string  Email              { get; set; } = string.Empty;
    public string? Address            { get; set; }
    public string? NationalIdFrontUrl { get; set; }
    public string? NationalIdBackUrl  { get; set; }

    /// <summary>هل البطاقتان محفوظتان مسبقاً؟</summary>
    public bool HasSavedNid =>
        !string.IsNullOrWhiteSpace(NationalIdFrontUrl) &&
        !string.IsNullOrWhiteSpace(NationalIdBackUrl);

    // ── قائمة مقدمي الخدمة ────────────────────────────────────────────────────
    public List<PatientProviderCardViewModel> Providers        { get; set; } = new();
    public List<string>                       Specializations  { get; set; } = new();
}

/// <summary>
/// بطاقة مقدم خدمة (طبيب أو ممرض) في صفحة طلب الزيارة
/// </summary>
public class PatientProviderCardViewModel
{
    public int     Id             { get; set; }
    public string  Type           { get; set; } = "doctor";   // "doctor" | "nurse"
    public string  Name           { get; set; } = string.Empty;
    public string  Spec           { get; set; } = string.Empty;
    public decimal Price          { get; set; }
    public decimal Rating         { get; set; }
    public string  ImageUrl       { get; set; } = "/assets/image/default-provider.png";
    public bool    HasCustomPhoto { get; set; }
    public string  Address        { get; set; } = "بني سويف";
    public string  ProfileUrl     { get; set; } = "#";
}