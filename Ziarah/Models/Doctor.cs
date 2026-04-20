using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Doctor
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SpecializationId { get; set; }

    public string? Bio { get; set; }

    public decimal ConsultationPrice { get; set; }

    public int ExperienceYears { get; set; }

    public decimal Rating { get; set; }

    public int TotalRatings { get; set; }

    public bool IsVerified { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Specialization Specialization { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
