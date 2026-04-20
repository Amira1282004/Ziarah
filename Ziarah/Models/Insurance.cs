using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Insurance
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public string PolicyNumberFormat { get; set; } = null!;

    public string? CoverageDetails { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Hospital> Hospitals { get; set; } = new List<Hospital>();

    public virtual ICollection<Lab> Labs { get; set; } = new List<Lab>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual ICollection<Pharmacy> Pharmacies { get; set; } = new List<Pharmacy>();

    public virtual ICollection<Radiology> Radiologies { get; set; } = new List<Radiology>();
}
