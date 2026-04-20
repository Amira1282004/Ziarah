using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Pharmacy
{
    public int Id { get; set; }

    public string PharmacyName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Location { get; set; } = null!;

    public bool Open24Hours { get; set; }

    public bool DeliveryAvailable { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public int? InsuranceId { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Insurance? Insurance { get; set; }
}
