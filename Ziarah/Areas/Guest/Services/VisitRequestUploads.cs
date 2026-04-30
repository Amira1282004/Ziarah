using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Ziarah.Areas.Guest.Services;

public static class VisitRequestUploads
{
    /// <summary>
    /// مجلد رفع صور طلبات الزيارة. لا يعتمد على WebRootPath فقط (قد يكون null في بعض الاستضافات).
    /// </summary>
    public static string GetVisitRequestsDirectory(IWebHostEnvironment env)
    {
        var webRoot = !string.IsNullOrWhiteSpace(env.WebRootPath)
            ? env.WebRootPath
            : Path.Combine(env.ContentRootPath, "wwwroot");
        return Path.Combine(webRoot, "uploads", "visit-requests");
    }

    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif"
    };

    private const long MaxBytes = 5 * 1024 * 1024;

    public static async Task<string?> TrySaveImageAsync(IWebHostEnvironment env, IFormFile file, string prefix, CancellationToken ct)
    {
        if (file.Length == 0 || file.Length > MaxBytes)
        {
            return null;
        }

        var contentType = (file.ContentType ?? string.Empty).ToLowerInvariant();
        if (!AllowedContentTypes.Contains(contentType))
        {
            return null;
        }

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext) || ext.Length > 6)
        {
            ext = contentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => ".jpg"
            };
        }

        ext = ext.ToLowerInvariant();
        if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp" or ".gif"))
        {
            return null;
        }

        var folder = GetVisitRequestsDirectory(env);
        Directory.CreateDirectory(folder);

        var name = $"{prefix}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, name);

        await using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, useAsync: true))
        {
            await file.CopyToAsync(stream, ct);
        }

        return "/uploads/visit-requests/" + name;
    }
}
