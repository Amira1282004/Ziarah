using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class Nurse
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Photo { get; set; }

    public int? HomeCareServiceId { get; set; }

    public int? UserId { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? User { get; set; }
}
