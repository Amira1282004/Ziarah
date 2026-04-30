using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Ziarah.Services;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendRegistrationVerificationCodeAsync(string toEmail, string code, CancellationToken cancellationToken = default)
    {
        var missingKeys = new List<string>();
        if (string.IsNullOrWhiteSpace(_options.Host))     missingKeys.Add("Smtp:Host");
        if (string.IsNullOrWhiteSpace(_options.User))     missingKeys.Add("Smtp:User");
        if (string.IsNullOrWhiteSpace(_options.From))     missingKeys.Add("Smtp:From");
        if (string.IsNullOrWhiteSpace(_options.Password)) missingKeys.Add("Smtp:Password");
        if (missingKeys.Count > 0)
        {
            var msg = "إعدادات SMTP ناقصة: " + string.Join(", ", missingKeys);
            _logger.LogError(msg);
            throw new InvalidOperationException(msg);
        }

        var body = $"""
            <div style="font-family:'Cairo',Tahoma,Arial,sans-serif;direction:rtl;text-align:right;max-width:520px;margin:auto;border:1px solid #e6edf5;border-radius:12px;overflow:hidden">
              <div style="background:linear-gradient(135deg,#128CCF,#12b6cf);padding:16px 20px;color:#fff">
                <h2 style="margin:0;font-size:20px">تأكيد إنشاء حسابك في زيارة</h2>
              </div>
              <div style="padding:20px;color:#1f2937;line-height:1.9">
                <p style="margin:0 0 12px">مرحباً،</p>
                <p style="margin:0 0 14px">كود التأكيد الخاص بك هو:</p>
                <div style="display:inline-block;background:#f3f8ff;border:1px dashed #128CCF;border-radius:10px;padding:8px 16px;font-size:28px;font-weight:800;letter-spacing:6px;color:#128CCF">
                  {code}
                </div>
                <p style="margin:14px 0 0;font-size:13px;color:#6b7280">صلاحية الكود محدودة. إذا لم تطلب إنشاء هذا الحساب يمكنك تجاهل الرسالة.</p>
              </div>
            </div>
            """;

        await SendAsync(toEmail, "زيارة — كود تأكيد التسجيل", body, cancellationToken);
    }

    public async Task SendPasswordResetAsync(string toEmail, string firstName, string resetLink, CancellationToken cancellationToken = default)
    {
        var body = $"""
            <div style="font-family:'Cairo',Tahoma,Arial,sans-serif;direction:rtl;text-align:right;max-width:520px;margin:auto;border:1px solid #e6edf5;border-radius:12px;overflow:hidden">
              <div style="background:linear-gradient(135deg,#128CCF,#12b6cf);padding:16px 20px;color:#fff">
                <h2 style="margin:0;font-size:20px">إعادة تعيين كلمة المرور — زيارة</h2>
              </div>
              <div style="padding:24px;color:#1f2937;line-height:1.9">
                <p style="margin:0 0 10px">مرحباً <strong>{firstName}</strong>،</p>
                <p style="margin:0 0 16px">تلقّينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك. اضغط على الزر أدناه لاختيار كلمة مرور جديدة:</p>
                <div style="text-align:center;margin:24px 0">
                  <a href="{resetLink}"
                     style="background:linear-gradient(135deg,#128CCF,#0f76b3);color:#fff;padding:12px 32px;border-radius:28px;text-decoration:none;font-size:15px;font-weight:700;display:inline-block">
                    إعادة تعيين كلمة المرور
                  </a>
                </div>
                <p style="margin:0 0 6px;font-size:13px;color:#6b7280">
                  ينتهي هذا الرابط خلال <strong>30 دقيقة</strong>.
                </p>
                <p style="margin:0;font-size:13px;color:#6b7280">
                  إذا لم تطلب إعادة التعيين، تجاهل هذه الرسالة وستبقى كلمة مرورك كما هي.
                </p>
              </div>
            </div>
            """;

        await SendAsync(toEmail, "زيارة — إعادة تعيين كلمة المرور", body, cancellationToken);
    }

    // ── مساعد مشترك ────────────────────────────────────────────────────────
    private async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct)
    {
        var missingKeys = new List<string>();
        if (string.IsNullOrWhiteSpace(_options.Host))     missingKeys.Add("Smtp:Host");
        if (string.IsNullOrWhiteSpace(_options.User))     missingKeys.Add("Smtp:User");
        if (string.IsNullOrWhiteSpace(_options.From))     missingKeys.Add("Smtp:From");
        if (string.IsNullOrWhiteSpace(_options.Password)) missingKeys.Add("Smtp:Password");
        if (missingKeys.Count > 0)
        {
            var msg = "إعدادات SMTP ناقصة: " + string.Join(", ", missingKeys);
            _logger.LogError(msg);
            throw new InvalidOperationException(msg);
        }

        var normalizedPassword = string.IsNullOrWhiteSpace(_options.Password)
            ? string.Empty
            : new string(_options.Password.Where(c => !char.IsWhiteSpace(c)).ToArray());

        using var message = new MailMessage
        {
            From       = new MailAddress(_options.From),
            Subject    = subject,
            Body       = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl   = _options.EnableSsl,
            Credentials = string.IsNullOrEmpty(_options.User)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(_options.User, normalizedPassword)
        };

        try
        {
            await client.SendMailAsync(message, ct);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "فشل SMTP أثناء إرسال بريد إلى {Email}", toEmail);
            throw;
        }
    }
}