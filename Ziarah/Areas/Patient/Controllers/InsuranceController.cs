using Microsoft.AspNetCore.Mvc;
using Ziarah.Areas.Patient.Models;
using Ziarah.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Ziarah.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class InsuranceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsuranceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Insurance()
        {
            var hospitals = _context.Hospitals
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Select(p => new InsuranceProviderViewModel
                {
                    Name = p.HospitalName,
                    Specialization = "مستشفى",
                    Address = p.Location,
                    Phone = p.Hotline,
                    TypeKey = "hospital",
                    ImageUrl = p.ImageUrl ?? "/image/provider/doctor.jpg"
                })
                .ToList()
                .AsEnumerable();

            var labs = _context.Labs
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Select(p => new InsuranceProviderViewModel
                {
                    Name = p.LabName,
                    Specialization = "معمل تحاليل",
                    Address = p.Location,
                    Phone = p.Phone,
                    TypeKey = "lab",
                    ImageUrl = p.ImageUrl ?? "/image/provider/doctor.jpg"
                })
                .ToList()
                .AsEnumerable();

            var pharmacies = _context.Pharmacies
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Select(p => new InsuranceProviderViewModel
                {
                    Name = p.PharmacyName,
                    Specialization = "صيدلية",
                    Address = p.Location,
                    Phone = p.Phone,
                    TypeKey = "pharmacy",
                    ImageUrl = p.ImageUrl ?? "/image/provider/doctor.jpg"
                })
                .ToList();

            var radiologies = _context.Radiologies
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Select(p => new InsuranceProviderViewModel
                {
                    Name = p.NameRadiology,
                    Specialization = "مركز أشعة",
                    Address = p.Location,
                    Phone = p.Phone,
                    TypeKey = "radiology",
                    ImageUrl = p.ImageUrl ?? "/image/provider/doctor.jpg"
                })
                .ToList()
                .AsEnumerable();

            var providers = hospitals
                .Concat(labs)
                .Concat(pharmacies)
                .Concat(radiologies)
                .Select(p =>
                {
                    var (address, area, governorate) = SplitLocation(p.Address);
                    return new InsuranceProviderViewModel
                    {
                        Name = p.Name,
                        Specialization = p.Specialization,
                        Address = address,
                        Phone = p.Phone,
                        Governorate = governorate,
                        Area = area,
                        TypeKey = p.TypeKey,
                        ImageUrl = p.ImageUrl
                    };
                })
                .Where(p => IsTargetGovernorate(p.Governorate, p.Address))
                .OrderBy(p => p.TypeKey)
                .ThenBy(p => p.Name)
                .ToList();

            return View(providers);
        }

        private static (string Address, string Area, string Governorate) SplitLocation(string location)
        {
            var parts = (location ?? string.Empty)
                .Split(" - ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (parts.Count == 0)
            {
                return ("غير محدد", string.Empty, string.Empty);
            }

            var governorate = parts[^1];
            var area = parts.Count >= 2 ? parts[^2] : string.Empty;
            var address = parts.Count >= 3 ? string.Join(" - ", parts.Take(parts.Count - 2)) : parts[0];

            return (address, area, governorate);
        }

        private static bool IsTargetGovernorate(string governorate, string address)
        {
            var gov = NormalizeArabic(governorate);
            var addr = NormalizeArabic(address);
            return gov.Contains("الفيوم") || gov.Contains("بنسويف") ||
                   addr.Contains("الفيوم") || addr.Contains("بنسويف");
        }

        private static string NormalizeArabic(string value)
        {
            var normalized = (value ?? string.Empty).ToLowerInvariant()
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ى", "ي")
                .Replace("ة", "ه");

            var cleaned = new StringBuilder(normalized.Length);
            foreach (var ch in normalized)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    cleaned.Append(ch);
                }
            }

            return cleaned.ToString();
        }

    }
}
