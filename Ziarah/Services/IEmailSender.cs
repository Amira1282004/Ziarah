namespace Ziarah.Services;

public interface IEmailSender
{
    Task SendRegistrationVerificationCodeAsync(string toEmail, string code, CancellationToken cancellationToken = default);

    Task SendPasswordResetAsync(string toEmail, string firstName, string resetLink, CancellationToken cancellationToken = default);
}