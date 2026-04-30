using System.Text.RegularExpressions;

namespace Ziarah.Areas.Guest.Models;

public class InsuranceProviderViewModel
{
    public string Name { get; init; } = string.Empty;
    public string Specialization { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Governorate { get; init; } = string.Empty;
    public string Area { get; init; } = string.Empty;
    public string TypeKey { get; init; } = "hospital";
    public string ImageUrl { get; init; } = "/image/provider/doctor.jpg";

    public string PhoneDial => Regex.Replace(Phone, @"\D", string.Empty);
}
