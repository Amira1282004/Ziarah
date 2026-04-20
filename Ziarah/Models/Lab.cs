using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Lab
{
    public int Id { get; set; }

    public string LabName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Location { get; set; } = null!;

    public bool TakingHomeSample { get; set; }

    public int? InsuranceId { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Insurance? Insurance { get; set; }
}
