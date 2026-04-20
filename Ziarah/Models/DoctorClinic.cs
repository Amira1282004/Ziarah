using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class DoctorClinic
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public int ClinicId { get; set; }

    public decimal? ConsultationPrice { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}
