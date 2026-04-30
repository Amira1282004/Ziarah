using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class HomeCareService
{
    internal DateTime UpdatedOn;

    public int Id { get; set; }

    public int? ServiceRequestId { get; set; }

    public DateTime RequestDate { get; set; }

    public string ServiceLocation { get; set; } = null!;

    public int? PatientId { get; set; }

    public int Status { get; set; }

    /// <summary>نوع مقدم الخدمة المطلوب: doctor أو nurse (مع <see cref="ProviderId"/>).</summary>
    public string? ProviderType { get; set; }

    /// <summary>معرف الطبيب في Doctors أو الممرض في Nurses حسب <see cref="ProviderType"/>.</summary>
    public int? ProviderId { get; set; }

    public string? RequesterFullName { get; set; }

    public string? RequesterPhone { get; set; }

    public string? RequesterEmail { get; set; }

    public DateTime? RequestedVisitAt { get; set; }

    public string? NationalIdFrontImage { get; set; }

    public string? NationalIdBackImage { get; set; }

    public string? MedicalCondition { get; set; }

    public string? AdditionalNotes { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}
