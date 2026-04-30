using Microsoft.AspNetCore.Hosting;

namespace Ziarah.Services;

public static class RegistrationUploads
{
    private const string ProjectFileName = "Ziarah.csproj";

    public static string GetRegistrationsDirectory(IWebHostEnvironment env)
    {
        var webRoot = ResolveWebRootPath(env);
        return Path.Combine(webRoot, "image", "docs");
    }

    private static string ResolveWebRootPath(IWebHostEnvironment env)
    {
        // 1) Prefer the project wwwroot (where the developer expects files to appear).
        foreach (var start in new[] { env.ContentRootPath, AppContext.BaseDirectory }.Where(p => !string.IsNullOrWhiteSpace(p)))
        {
            var current = new DirectoryInfo(start!);
            for (var i = 0; i < 8 && current is not null; i++, current = current.Parent)
            {
                var csproj = Path.Combine(current.FullName, ProjectFileName);
                if (File.Exists(csproj))
                {
                    var projectWebRoot = Path.Combine(current.FullName, "wwwroot");
                    Directory.CreateDirectory(projectWebRoot);
                    return projectWebRoot;
                }
            }
        }

        // 2) Fallback to hosting paths.
        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(env.ContentRootPath))
        {
            candidates.Add(Path.Combine(env.ContentRootPath, "wwwroot"));
            candidates.Add(Path.Combine(env.ContentRootPath, "Ziarah", "wwwroot"));
        }

        if (!string.IsNullOrWhiteSpace(env.WebRootPath))
        {
            candidates.Add(env.WebRootPath);
        }

        foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(candidate) && Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        // Final fallback: create under content root.
        var fallback = Path.Combine(env.ContentRootPath, "wwwroot");
        Directory.CreateDirectory(fallback);
        return fallback;
    }

    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif"
    };

    private const long MaxBytes = 5 * 1024 * 1024;

    /// <summary>يحفظ الملف داخل مجلد فرعي لجلسة التسجيل ويعيد المسار النسبي تحت wwwroot (يبدأ بـ /).</summary>
    public static async Task<string?> TrySaveImageAsync(IWebHostEnvironment env, IFormFile file, string pendingId, string prefix, CancellationToken ct)
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

        var folder = Path.Combine(GetRegistrationsDirectory(env), pendingId);
        Directory.CreateDirectory(folder);

        var name = $"{prefix}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, name);

        await using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, useAsync: true))
        {
            await file.CopyToAsync(stream, ct);
        }

        return "/image/docs/" + pendingId + "/" + name;
    }
}
