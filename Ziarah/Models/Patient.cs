using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Patient
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? InsuranceId { get; set; }

    public string? BloodType { get; set; }

    public decimal? Height { get; set; }

    public decimal? Weight { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Insurance? Insurance { get; set; }

    public virtual User User { get; set; } = null!;
}
