using System;

namespace Ziarah.Models;

public partial class InsuranceProvider
{
    public int Id { get; set; }

    public string ProviderName { get; set; } = null!;

    public string ProviderType { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Governorate { get; set; } = null!;

    public string Area { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }
}
