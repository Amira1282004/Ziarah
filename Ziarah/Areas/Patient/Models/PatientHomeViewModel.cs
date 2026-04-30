namespace Ziarah.Areas.Patient.Models;

public sealed class PatientHomeViewModel
{
    public string  FirstName          { get; set; } = string.Empty;
    public string  FullName           { get; set; } = string.Empty;
    public string  Email              { get; set; } = string.Empty;
    public string  Phone              { get; set; } = string.Empty;
    public string? PhotoUrl           { get; set; }
    public bool    HasInsurance       { get; set; }
    public string? Address            { get; set; }
    public string? NationalIdFrontUrl { get; set; }
    public string? NationalIdBackUrl  { get; set; }

    public List<NotificationItem>  Notifications { get; set; } = new();
    public int UnreadCount => Notifications.Count(n => !n.IsRead);

    public List<HomeProviderCard> TopDoctors  { get; set; } = new();
    public List<HomeProviderCard> TopNurses   { get; set; } = new();
    public List<HomeVisitCard>    VisitHistory { get; set; } = new();
}

public sealed class NotificationItem
{
    public int      Id        { get; set; }
    public string   Title     { get; set; } = string.Empty;
    public string   Message   { get; set; } = string.Empty;
    public int     Type      { get; set; }
    public bool     IsRead    { get; set; }
    public DateTime CreatedOn { get; set; }
}

public sealed class HomeProviderCard
{
    public int     Id         { get; set; }
    public string  Type       { get; set; } = "doctor";
    public string  Name       { get; set; } = string.Empty;
    public string  Spec       { get; set; } = string.Empty;
    public decimal Price      { get; set; }
    public decimal Rating     { get; set; }
    public string? ImageUrl   { get; set; }
    public string  ProfileUrl { get; set; } = "#";
}

public sealed class HomeVisitCard
{
    public int      Id            { get; set; }
    public string   ProviderName  { get; set; } = string.Empty;
    public string   ProviderSpec  { get; set; } = string.Empty;
    public string?  ProviderPhoto { get; set; }
    public string   Status        { get; set; } = "pending";
    public string   StatusLabel   { get; set; } = "قيد الانتظار";
    public DateTime RequestedAt   { get; set; }
    public string   Address       { get; set; } = string.Empty;
    public decimal  Price         { get; set; }
}