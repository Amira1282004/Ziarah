namespace Ziarah.Areas.Guest.Models;

public class RequestVisitViewModel
{
    public List<ProviderCardViewModel> Providers { get; set; } = new();
    public List<string> Specializations { get; set; } = new();
}

public class ProviderCardViewModel
{
    public int Id { get; set; }
    public string Type { get; set; } = "doctor";
    public string Name { get; set; } = string.Empty;
    public string Spec { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Rating { get; set; }
    public string ImageUrl { get; set; } = "/assets/image/mohamed.png";
    public bool HasCustomPhoto { get; set; }
    public string Address { get; set; } = "بني سويف";
    public string ProfileUrl { get; set; } = "#";
}
