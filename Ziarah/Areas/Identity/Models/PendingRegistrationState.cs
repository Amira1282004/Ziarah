namespace Ziarah.Areas.Identity.Models;

/// <summary>بيانات مؤقتة في الجلسة بين خطوة التسجيل وتأكيد البريد.</summary>
public sealed class PendingRegistrationState
{
    public string PendingId { get; set; } = string.Empty;
    public string Role { get; set; } = "patient";
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string NormalizedUserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DateOfBirthIso { get; set; }
    public string? Gender { get; set; }
    public string NationalIdFrontUrl { get; set; } = string.Empty;
    public string NationalIdBackUrl { get; set; } = string.Empty;
    public string? LicenseUrl { get; set; }
    public int? SpecializationId { get; set; }
    public decimal? ConsultationPrice { get; set; }
    public int? ExperienceYears { get; set; }
    public bool PatientHasInsurance { get; set; }
    public string? PatientInsuranceImageUrl { get; set; }
    public string ExpectedCode { get; set; } = string.Empty;
    public DateTimeOffset CodeExpiresUtc { get; set; }
    public DateTimeOffset? LastResendUtc { get; set; }
}
