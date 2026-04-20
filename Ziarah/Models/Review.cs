using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Review
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int AppointmentId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsApproved { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}
