namespace Ziarah.Services;

/// <summary>يُستخدم عند عدم ضبط SMTP: يطبع الرابط في السجلات (مناسب للتطوير).</summary>
public sealed class LogEmailSender : IEmailSender
{
    private readonly ILogger<LogEmailSender> _logger;

    public LogEmailSender(ILogger<LogEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendRegistrationVerificationCodeAsync(string toEmail, string code, CancellationToken cancellationToken = default)
    {
        _logger.LogError("SMTP غير مضبوط — تعذر إرسال كود التأكيد للبريد {Email}. الكود: {Code}", toEmail, code);
        throw new InvalidOperationException("خدمة البريد غير مهيأة. يرجى ضبط إعدادات SMTP.");
    }

    public Task SendPasswordResetAsync(string toEmail, string firstName, string resetLink, CancellationToken cancellationToken = default)
    {
        _logger.LogError("SMTP غير مضبوط — تعذر إرسال رابط إعادة التعيين للبريد {Email}. الرابط: {Link}", toEmail, resetLink);
        throw new InvalidOperationException("خدمة البريد غير مهيأة. يرجى ضبط إعدادات SMTP.");
    }
}