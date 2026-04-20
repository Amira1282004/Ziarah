using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Department
{
    public int Id { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime HospitalId { get; set; }

    public DateTime IsDeleted { get; set; }

    public DateTime CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime LastModifiedBy { get; set; }

    public DateTime LastModifiedOn { get; set; }
}
