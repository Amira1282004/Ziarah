namespace Ziarah.Services;

public sealed class RegistrationOptions
{
    public const string SectionName = "Registration";

    public int CodeExpiryMinutes { get; set; } = 5;
    public int ResendCooldownSeconds { get; set; } = 60;
}
