using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class AmbulanceRequest
{
    public int Id { get; set; }

    public DateTime RequestTime { get; set; }

    public int Status { get; set; }

    public int? PatientId { get; set; }

    public string Location { get; set; } = null!;

    public int? HospitalId { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}
