namespace Ziarah.Areas.Guest.Models;

public class ProviderProfileViewModel
{
    public int Id { get; set; }
    public string Type { get; set; } = "doctor";
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Rating { get; set; }
    public int ExperienceYears { get; set; }
    public int TotalRatings { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Address { get; set; } = "بني سويف";
}
