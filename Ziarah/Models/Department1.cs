using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Department1
{
    public int Id { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string? Description { get; set; }

    public int? HospitalId { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}
