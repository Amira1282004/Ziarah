using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Ziarah.Data;


namespace Ziarah.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var term = (query ?? string.Empty).Trim();
            if (term.Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            const string doctorServiceArea = "بني سويف والفيوم";
            var expandedTerms = ExpandTerms(term);
            var doctorsRaw = await (from d in _context.Doctors.AsNoTracking()
                                    join u in _context.Users.AsNoTracking() on d.UserId equals u.Id
                                    join s in _context.Specializations.AsNoTracking() on d.SpecializationId equals s.Id
                                    where !d.IsDeleted
                                    select new
                                    {
                                        d.Id,
                                        FullName = u.FullName,
                                        SpecializationName = s.Name
                                    }).ToListAsync();

            var doctors = doctorsRaw
                .Select(d => new
                {
                    id = d.Id,
                    name = d.FullName,
                    specialization = d.SpecializationName,
                    cities = doctorServiceArea
                })
                .Select(d => new
                {
                    type = "doctor",
                    name = d.name,
                    meta = string.IsNullOrWhiteSpace(d.cities)
                        ? $"تخصص: {d.specialization}"
                        : $"تخصص: {d.specialization} | الموقع: {d.cities}",
                    url = Url.Action("ProviderProfile", "ProviderProfile", new { area = "Guest", id = d.id }),
                    score = ScoreMatch(expandedTerms, d.name, d.specialization, d.cities, "طبيب")
                })
                .Where(x => x.score > 0)
                .OrderByDescending(x => x.score)
                .Take(8)
                .ToList();

            var nurses = await _context.Nurses
                .AsNoTracking()
                .Include(n => n.User)
                .Where(n => !n.IsDeleted)
                .Select(n => new
                {
                    type = "nurse",
                    name = n.User.FullName,
                    meta = "تمريض منزلي - بني سويف والفيوم",
                    url = Url.Action("RequestVisit", "RequestVisit", new { area = "Guest" }),
                    score = ScoreMatch(expandedTerms, n.User.FirstName, n.User.LastName, "تمريض", "ممرض")
                })
                .Take(8)
                .ToListAsync();

            nurses = nurses
                .Where(n => n.score > 0)
                .OrderByDescending(n => n.score)
                .Take(8)
                .ToList();

            var services = await _context.Specializations
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .Select(s => new
                {
                    type = "service",
                    name = s.Name,
                    meta = "تخصص طبي",
                    url = Url.Action("RequestVisit", "RequestVisit", new { area = "Guest" }),
                    score = ScoreMatch(expandedTerms, s.Name, s.Description ?? string.Empty, "خدمة", "تخصص")
                })
                .Take(8)
                .ToListAsync();

            services = services
                .Where(s => s.score > 0)
                .OrderByDescending(s => s.score)
                .Take(8)
                .ToList();

            var results = doctors
                .Concat(nurses)
                .Concat(services)
                .OrderByDescending(x => x.score)
                .Take(15);

            return Json(results);
        }

        private static List<string> ExpandTerms(string term)
        {
            var tokens = term.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            tokens.Add(term);

            var aliases = new Dictionary<string, string[]>
            {
                ["قلب"] = new[] { "قلب وأوعية دموية", "اوعية", "باطنة" },
                ["جلدية"] = new[] { "جلدية", "الجلد" },
                ["عظام"] = new[] { "عظام", "مفاصل" },
                ["اطفال"] = new[] { "أطفال", "اطفال" },
                ["نساء"] = new[] { "نساء وتوليد", "توليد" },
                ["باطنة"] = new[] { "باطنة", "طب باطني" },
                ["مخ"] = new[] { "مخ وأعصاب", "اعصاب" },
                ["عين"] = new[] { "عيون", "رمد" },
                ["بنى سويف"] = new[] { "بني سويف", "بنى سويف" },
                ["الفيوم"] = new[] { "الفيوم", "فيوم" },
                ["تمريض"] = new[] { "تمريض", "ممرض", "ممرضة" }
            };

            var normalizedInput = NormalizeArabic(term);
            foreach (var alias in aliases)
            {
                if (normalizedInput.Contains(NormalizeArabic(alias.Key)))
                {
                    tokens.AddRange(alias.Value);
                }
            }

            return tokens
                .Select(NormalizeArabic)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();
        }

        private static int ScoreMatch(List<string> terms, params string[] fields)
        {
            var normalizedFields = fields
                .Select(NormalizeArabic)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var score = 0;
            foreach (var term in terms)
            {
                foreach (var field in normalizedFields)
                {
                    if (field.StartsWith(term))
                    {
                        score += 6;
                    }
                    else if (field.Contains(term))
                    {
                        score += 3;
                    }
                }
            }

            return score;
        }

        private static string NormalizeArabic(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return text
                .Trim()
                .ToLowerInvariant()
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ى", "ي")
                .Replace("ة", "ه")
                .Replace("ؤ", "و")
                .Replace("ئ", "ي")
                .Replace("ـ", "");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    internal class ErrorViewModel
    {
        public string RequestId { get; set; }
    }
}
