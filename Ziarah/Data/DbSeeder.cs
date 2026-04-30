using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using Ziarah.Models;
using System.Text;

namespace Ziarah.Data;

public static class DbSeeder
{
    private static readonly string[] ArabicFirstNames = { "محمد", "أحمد", "محمود", "مصطفى", "خالد", "عبدالرحمن", "إبراهيم", "طارق", "وائل", "ياسر", "حسن", "حسين", "علاء", "سامي", "عمرو", "رامي", "وليد", "شريف", "زين", "مروان" };
    private static readonly string[] ArabicLastNames = { "عبدالله", "السيد", "حسن", "عبدالفتاح", "الشرقاوي", "العطار", "سليمان", "بدوي", "حماد", "رشاد", "فرج", "عبدالمنعم", "مرسي", "عبدالعزيز", "فهمي", "صالح", "لطفي", "طه", "نبيل", "حجازي" };
    private static readonly string[] ProviderProfileImages = { "/image/provider/doctor.jpg", "/image/provider/nurse.jpg", "/image/provider/ahmed.jpg", "/image/provider/ali.jpg", "/image/provider/ibrahim.jpeg", "/image/provider/khaled.jpg", "/image/provider/mahmoud.jpg", "/image/provider/mohamed.png", "/image/provider/nour.png", "/image/provider/omar.png", "/image/provider/rasha.jpg", "/image/provider/saad.JPG", "/image/provider/sara.png", "/image/provider/taha.jpg", "/image/provider/youssef.png" };

    private const string ProviderNationalIdFront = "/image/provider/بطاقة الرقم القومي1.jpg";
    private const string ProviderNationalIdBack = "/image/provider/بطاقة الرقم القومي2.jpg";
    private const string ProviderLicense = "/image/provider/شهادة الطب.jfif";
    private const string UserProfileImage = "/image/user/user-image_1796659.jpg";
    private const string UserNationalIdFront = "/image/user/بطاقة الرقم القومي1.jpg";
    private const string UserNationalIdBack = "/image/user/بطاقة الرقم القومي2.jpg";
    private const string UserInsuranceImage = "/image/user/كارنيه الـتأمين.jpg";

    public static void Seed(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();
        var adminUser = EnsureAdminUser(context);
        var insurance = EnsureInsurance(context, adminUser.Id);
        var specializationIds = EnsureMedicalSpecializations(context, adminUser.Id);
        EnsureDoctors(context, adminUser.Id, specializationIds, 50);
        EnsureNurses(context, adminUser.Id, 30);
        EnsurePatients(context, adminUser.Id, insurance.Id, 200);
        EnsureInsuranceNetworkFromExcel(context, adminUser.Id, insurance.Id);
        EnsureInsuranceImagePaths(context, insurance.Id);
        LocalizeSeedData(context, specializationIds);
    }

    private static User EnsureAdminUser(ApplicationDbContext context)
    {
        var admin = context.Users.FirstOrDefault(u => u.UserName == "admin");
        if (admin is not null) return admin;
        var nextUserId = context.Users.Any() ? context.Users.Max(u => u.Id) + 1 : 1;
        admin = new User { Id = nextUserId, FirstName = "مشرف", LastName = "النظام", UserName = "admin", NormalizedUserName = "ADMIN", Email = "admin@ziarah.local", NormalizedEmail = "ADMIN@ZIARAH.LOCAL", EmailConfirmed = true, PasswordHash = "SEED_PLACEHOLDER_HASH", PhoneNumberConfirmed = false, TwoFactorEnabled = false, Photo = null, NationalIdFrontImage = UserNationalIdFront, NationalIdBackImage = UserNationalIdBack, LockoutEnabled = true, AccessFailedCount = 0, Status = 1, IsDeleted = false, CreatedBy = nextUserId };
        context.Users.Add(admin);
        context.SaveChanges();
        return admin;
    }

    private static Insurance EnsureInsurance(ApplicationDbContext context, int createdBy)
    {
        var insurance = context.Insurances.FirstOrDefault();
        if (insurance is not null) return insurance;
        insurance = new Insurance { CompanyName = "تأمين اعضاء هيئة التدريس", PolicyNumberFormat = "FAC-{000000}", CoverageDetails = "شبكة علاجية لاعضاء هيئة التدريس تشمل مستشفيات ومعامل وصيدليات ومراكز اشعة.", IsActive = true, IsDeleted = false, CreatedBy = createdBy };
        context.Insurances.Add(insurance);
        context.SaveChanges();
        return insurance;
    }

    private static List<int> EnsureMedicalSpecializations(ApplicationDbContext context, int createdBy)
    {
        var seedSpecializations = new List<(string Name, string Description)> { ("قلب وأوعية دموية", "تشخيص وعلاج أمراض القلب والشرايين"), ("جلدية", "أمراض الجلد والشعر والأظافر"), ("مخ وأعصاب", "مشكلات المخ والأعصاب والعمود الفقري العصبي"), ("أطفال", "رعاية صحية للأطفال من الولادة حتى المراهقة"), ("عظام", "إصابات ومشكلات العظام والمفاصل"), ("عيون", "فحص وعلاج أمراض العيون والإبصار"), ("أنف وأذن وحنجرة", "مشكلات السمع والأنف والجيوب والحنجرة"), ("باطنة", "تشخيص وعلاج الأمراض الباطنية المزمنة والحادة"), ("جراحة عامة", "التدخلات الجراحية العامة والمتابعة بعد العملية"), ("نساء وتوليد", "صحة المرأة والحمل والولادة") };
        var existingNames = context.Specializations.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var toAdd = seedSpecializations.Where(s => !existingNames.Contains(s.Name)).Select(s => new Specialization { Name = s.Name, Description = s.Description, IsDeleted = false, CreatedBy = createdBy }).ToList();
        if (toAdd.Count > 0) { context.Specializations.AddRange(toAdd); context.SaveChanges(); }
        return context.Specializations.OrderBy(s => s.Id).Take(10).Select(s => s.Id).ToList();
    }

    private static void EnsureDoctors(ApplicationDbContext context, int createdBy, List<int> specializationIds, int targetCount)
    {
        var existing = context.Doctors.Count();
        var missing = targetCount - existing;
        if (missing <= 0) return;
        var nextUserId = context.Users.Any() ? context.Users.Max(u => u.Id) + 1 : 1;
        var users = new List<User>();
        var doctors = new List<Doctor>();
        for (var i = 0; i < missing; i++)
        {
            var userId = nextUserId++;
            var no = existing + i + 1;
            var userName = $"doctor.seed.{no:D3}";
            var email = $"doctor{no:D3}@ziarah.local";
            users.Add(new User { Id = userId, FirstName = ArabicFirstNames[(no - 1) % ArabicFirstNames.Length], LastName = ArabicLastNames[(no - 1) % ArabicLastNames.Length], UserName = userName, NormalizedUserName = userName.ToUpperInvariant(), Email = email, NormalizedEmail = email.ToUpperInvariant(), EmailConfirmed = true, PasswordHash = "SEED_PLACEHOLDER_HASH", PhoneNumberConfirmed = false, TwoFactorEnabled = false, Photo = null, NationalIdFrontImage = ProviderNationalIdFront, NationalIdBackImage = ProviderNationalIdBack, LockoutEnabled = true, AccessFailedCount = 0, Status = 1, IsDeleted = false, CreatedBy = createdBy });
            doctors.Add(new Doctor { UserId = userId, SpecializationId = specializationIds[i % specializationIds.Count], Bio = "طبيب متخصص يقدم زيارات منزلية داخل محافظتي بني سويف والفيوم.", ProfessionalLicenseImage = ProviderLicense, ConsultationPrice = 250.00m + i, ExperienceYears = 3 + (i % 15), Rating = 4.2m, TotalRatings = 10 + i, IsVerified = true, IsDeleted = false, CreatedBy = createdBy });
        }
        context.Users.AddRange(users);
        context.Doctors.AddRange(doctors);
        context.SaveChanges();
    }

    private static void EnsureNurses(ApplicationDbContext context, int createdBy, int targetCount)
    {
        var existing = context.Nurses.Count();
        var missing = targetCount - existing;
        if (missing <= 0) return;
        var nextUserId = context.Users.Any() ? context.Users.Max(u => u.Id) + 1 : 1;
        var users = new List<User>();
        var nurses = new List<Nurse>();
        for (var i = 0; i < missing; i++)
        {
            var userId = nextUserId++;
            var no = existing + i + 1;
            var userName = $"nurse.seed.{no:D3}";
            var email = $"nurse{no:D3}@ziarah.local";
            var first = ArabicFirstNames[(no + 3) % ArabicFirstNames.Length];
            var last = ArabicLastNames[(no + 5) % ArabicLastNames.Length];
            users.Add(new User { Id = userId, FirstName = first, LastName = last, UserName = userName, NormalizedUserName = userName.ToUpperInvariant(), Email = email, NormalizedEmail = email.ToUpperInvariant(), EmailConfirmed = true, PasswordHash = "SEED_PLACEHOLDER_HASH", PhoneNumberConfirmed = false, TwoFactorEnabled = false, Photo = null, NationalIdFrontImage = ProviderNationalIdFront, NationalIdBackImage = ProviderNationalIdBack, LockoutEnabled = true, AccessFailedCount = 0, Status = 1, IsDeleted = false, CreatedBy = createdBy });
            nurses.Add(new Nurse { ProfessionalLicenseImage = ProviderLicense, UserId = userId, IsDeleted = false, CreatedBy = createdBy });
        }
        context.Users.AddRange(users);
        context.Nurses.AddRange(nurses);
        context.SaveChanges();
    }

    private static void EnsurePatients(ApplicationDbContext context, int createdBy, int insuranceId, int targetCount)
    {
        var existing = context.Patients.Count();
        var missing = targetCount - existing;
        if (missing <= 0) return;
        var bloodTypes = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
        var nextUserId = context.Users.Any() ? context.Users.Max(u => u.Id) + 1 : 1;
        var users = new List<User>();
        var patients = new List<Patient>();
        for (var i = 0; i < missing; i++)
        {
            var userId = nextUserId++;
            var no = existing + i + 1;
            var userName = $"patient.seed.{no:D3}";
            var email = $"patient{no:D3}@ziarah.local";
            var hasInsurance = no % 3 != 0;
            users.Add(new User { Id = userId, FirstName = ArabicFirstNames[(no + 7) % ArabicFirstNames.Length], LastName = ArabicLastNames[(no + 9) % ArabicLastNames.Length], UserName = userName, NormalizedUserName = userName.ToUpperInvariant(), Email = email, NormalizedEmail = email.ToUpperInvariant(), EmailConfirmed = true, PasswordHash = "SEED_PLACEHOLDER_HASH", PhoneNumber = $"010{(70000000 + no):D8}", PhoneNumberConfirmed = false, TwoFactorEnabled = false, DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-20 - (no % 35))), Gender = no % 2 == 0 ? "Male" : "Female", Photo = null, NationalIdFrontImage = UserNationalIdFront, NationalIdBackImage = UserNationalIdBack, LockoutEnabled = true, AccessFailedCount = 0, Status = 1, IsDeleted = false, CreatedBy = createdBy });
            patients.Add(new Patient { UserId = userId, InsuranceId = hasInsurance ? insuranceId : null, HasInsurance = hasInsurance, InsuranceImage = hasInsurance ? UserInsuranceImage : null, BloodType = bloodTypes[no % bloodTypes.Length], Height = 155 + (no % 25), Weight = 55 + (no % 35), IsDeleted = false, CreatedBy = createdBy });
        }
        context.Users.AddRange(users);
        context.Patients.AddRange(patients);
        context.SaveChanges();
    }

    private static void LocalizeSeedData(ApplicationDbContext context, List<int> specializationIds)
    {
        var doctorUsers = context.Users.Where(u => u.UserName.StartsWith("doctor.seed.")).OrderBy(u => u.UserName).ToList();
        for (var i = 0; i < doctorUsers.Count; i++) { doctorUsers[i].FirstName = ArabicFirstNames[i % ArabicFirstNames.Length]; doctorUsers[i].LastName = ArabicLastNames[i % ArabicLastNames.Length]; doctorUsers[i].Photo = null; }
        var nurseUsers = context.Users.Where(u => u.UserName.StartsWith("nurse.seed.")).OrderBy(u => u.UserName).ToList();
        for (var i = 0; i < nurseUsers.Count; i++) { nurseUsers[i].FirstName = ArabicFirstNames[(i + 2) % ArabicFirstNames.Length]; nurseUsers[i].LastName = ArabicLastNames[(i + 4) % ArabicLastNames.Length]; nurseUsers[i].Photo = null; }
        var doctors = context.Doctors.OrderBy(d => d.Id).ToList();
        for (var i = 0; i < doctors.Count; i++) { doctors[i].SpecializationId = specializationIds[i % specializationIds.Count]; doctors[i].Bio = "طبيب متخصص يقدم زيارات منزلية داخل محافظتي بني سويف والفيوم."; }
        context.SaveChanges();
    }

    private static void EnsureInsuranceNetworkFromExcel(ApplicationDbContext context, int createdBy, int insuranceId)
    {
        var excelPath = Path.Combine(AppContext.BaseDirectory, "مشروع العلاج-1.xlsx");
        if (!File.Exists(excelPath))
        {
            excelPath = Path.Combine(Directory.GetCurrentDirectory(), "مشروع العلاج-1.xlsx");
        }

        if (!File.Exists(excelPath))
        {
            return;
        }

        using var workbook = new XLWorkbook(excelPath);
        var sheet = workbook.Worksheet(1);
        var range = sheet.RangeUsed();
        if (range is null)
        {
            return;
        }

        var hospitals = new List<Hospital>();
        var labs = new List<Lab>();
        var pharmacies = new List<Pharmacy>();
        var radiologies = new List<Radiology>();
        var imageMaps = BuildInsuranceImageMaps();

        foreach (var row in range.RowsUsed().Skip(1))
        {
            var providerName = row.Cell(1).GetString().Trim();
            if (string.IsNullOrWhiteSpace(providerName))
            {
                continue;
            }

            var providerType = row.Cell(2).GetString().Trim();
            var address = row.Cell(3).GetString().Trim();
            var phone = row.Cell(4).GetString().Trim();
            var governorate = row.Cell(5).GetString().Trim();
            var area = row.Cell(6).GetString().Trim();
            if (!IsTargetGovernorate(governorate))
            {
                continue;
            }

            var location = string.Join(" - ", new[] { address, area, governorate }.Where(v => !string.IsNullOrWhiteSpace(v)));

            if (providerType.Contains("صيد", StringComparison.OrdinalIgnoreCase))
            {
                pharmacies.Add(new Pharmacy
                {
                    PharmacyName = providerName,
                    Phone = NormalizePhone(phone),
                    Location = NormalizeLocation(location),
                    ImageUrl = ResolveInsuranceImagePath("pharmacy", providerName, imageMaps),
                    Open24Hours = false,
                    DeliveryAvailable = false,
                    InsuranceId = insuranceId,
                    IsDeleted = false,
                    CreatedBy = createdBy
                });
                continue;
            }

            if (providerType.Contains("أشعة", StringComparison.OrdinalIgnoreCase) ||
                providerType.Contains("اشعة", StringComparison.OrdinalIgnoreCase))
            {
                radiologies.Add(new Radiology
                {
                    NameRadiology = providerName,
                    Phone = NormalizePhone(phone),
                    Location = NormalizeLocation(location),
                    ImageUrl = ResolveInsuranceImagePath("radiology", providerName, imageMaps),
                    Types = providerType,
                    InsuranceId = insuranceId,
                    IsDeleted = false,
                    CreatedBy = createdBy
                });
                continue;
            }

            if (providerType.Contains("معمل", StringComparison.OrdinalIgnoreCase) ||
                providerType.Contains("تحاليل", StringComparison.OrdinalIgnoreCase))
            {
                labs.Add(new Lab
                {
                    LabName = providerName,
                    Phone = NormalizePhone(phone),
                    Location = NormalizeLocation(location),
                    ImageUrl = ResolveInsuranceImagePath("lab", providerName, imageMaps),
                    TakingHomeSample = false,
                    InsuranceId = insuranceId,
                    IsDeleted = false,
                    CreatedBy = createdBy
                });
                continue;
            }

            hospitals.Add(new Hospital
            {
                HospitalName = providerName,
                Hotline = NormalizePhone(phone),
                Location = NormalizeLocation(location),
                ImageUrl = ResolveInsuranceImagePath("hospital", providerName, imageMaps),
                InsuranceId = insuranceId,
                IsDeleted = false,
                CreatedBy = createdBy
            });
        }

        if (hospitals.Count + labs.Count + pharmacies.Count + radiologies.Count == 0)
        {
            return;
        }

        context.Hospitals.RemoveRange(context.Hospitals.Where(x => x.InsuranceId == insuranceId));
        context.Labs.RemoveRange(context.Labs.Where(x => x.InsuranceId == insuranceId));
        context.Pharmacies.RemoveRange(context.Pharmacies.Where(x => x.InsuranceId == insuranceId));
        context.Radiologies.RemoveRange(context.Radiologies.Where(x => x.InsuranceId == insuranceId));
        context.SaveChanges();
        context.Hospitals.AddRange(hospitals);
        context.Labs.AddRange(labs);
        context.Pharmacies.AddRange(pharmacies);
        context.Radiologies.AddRange(radiologies);
        context.SaveChanges();
    }

    private static void EnsureInsuranceImagePaths(ApplicationDbContext context, int insuranceId)
    {
        var imageMaps = BuildInsuranceImageMaps();
        var hasUpdates = false;

        var hospitals = context.Hospitals.Where(x => !x.IsDeleted && x.InsuranceId == insuranceId).ToList();
        foreach (var hospital in hospitals)
        {
            var imagePath = ResolveInsuranceImagePath("hospital", hospital.HospitalName, imageMaps);
            if (!string.Equals(hospital.ImageUrl, imagePath, StringComparison.Ordinal))
            {
                hospital.ImageUrl = imagePath;
                hasUpdates = true;
            }
        }

        var labs = context.Labs.Where(x => !x.IsDeleted && x.InsuranceId == insuranceId).ToList();
        foreach (var lab in labs)
        {
            var imagePath = ResolveInsuranceImagePath("lab", lab.LabName, imageMaps);
            if (!string.Equals(lab.ImageUrl, imagePath, StringComparison.Ordinal))
            {
                lab.ImageUrl = imagePath;
                hasUpdates = true;
            }
        }

        var pharmacies = context.Pharmacies.Where(x => !x.IsDeleted && x.InsuranceId == insuranceId).ToList();
        foreach (var pharmacy in pharmacies)
        {
            var imagePath = ResolveInsuranceImagePath("pharmacy", pharmacy.PharmacyName, imageMaps);
            if (!string.Equals(pharmacy.ImageUrl, imagePath, StringComparison.Ordinal))
            {
                pharmacy.ImageUrl = imagePath;
                hasUpdates = true;
            }
        }

        var radiologies = context.Radiologies.Where(x => !x.IsDeleted && x.InsuranceId == insuranceId).ToList();
        foreach (var radiology in radiologies)
        {
            var imagePath = ResolveInsuranceImagePath("radiology", radiology.NameRadiology, imageMaps);
            if (!string.Equals(radiology.ImageUrl, imagePath, StringComparison.Ordinal))
            {
                radiology.ImageUrl = imagePath;
                hasUpdates = true;
            }
        }

        if (hasUpdates)
        {
            context.SaveChanges();
        }
    }

    private static Dictionary<string, Dictionary<string, string>> BuildInsuranceImageMaps()
    {
        return new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["hospital"] = BuildImageMapForType("hospital"),
            ["lab"] = BuildImageMapForType("lab"),
            ["pharmacy"] = BuildImageMapForType("pharmacy"),
            ["radiology"] = BuildImageMapForType("radiology")
        };
    }

    private static Dictionary<string, string> BuildImageMapForType(string typeKey)
    {
        var root = Directory.GetCurrentDirectory();
        var folderPath = Path.Combine(root, "wwwroot", "image", "insurance", typeKey);
        if (!Directory.Exists(folderPath))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var filePath in Directory.GetFiles(folderPath))
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var normalized = NormalizeForMatch(fileName);
            if (!string.IsNullOrWhiteSpace(normalized) && !map.ContainsKey(normalized))
            {
                map[normalized] = $"/image/insurance/{typeKey}/{Path.GetFileName(filePath)}";
            }
        }

        return map;
    }

    private static string ResolveInsuranceImagePath(string typeKey, string providerName, Dictionary<string, Dictionary<string, string>> imageMaps)
    {
        const string fallback = "/image/provider/doctor.jpg";
        if (!imageMaps.TryGetValue(typeKey, out var typeMap))
        {
            return fallback;
        }

        var normalizedName = NormalizeForMatch(providerName);
        if (typeMap.TryGetValue(normalizedName, out var exact))
        {
            return exact;
        }

        var partial = typeMap.FirstOrDefault(x => normalizedName.Contains(x.Key) || x.Key.Contains(normalizedName));
        return string.IsNullOrWhiteSpace(partial.Value) ? fallback : partial.Value;
    }

    private static string NormalizeForMatch(string value)
    {
        var normalized = (value ?? string.Empty).ToLowerInvariant();
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

    private static bool IsTargetGovernorate(string governorate)
    {
        if (string.IsNullOrWhiteSpace(governorate))
        {
            return false;
        }

        var normalized = NormalizeForMatch(governorate);
        return normalized.Contains("بني سويف") || normalized.Contains("الفيوم");
    }

    private static string NormalizePhone(string? rawPhone)
    {
        if (string.IsNullOrWhiteSpace(rawPhone))
        {
            return "غير متاح";
        }

        var value = rawPhone.Trim();
        return value.Length <= 50 ? value : value[..50];
    }

    private static string NormalizeLocation(string? rawLocation)
    {
        if (string.IsNullOrWhiteSpace(rawLocation))
        {
            return "غير محدد";
        }

        var value = rawLocation.Trim();
        return value.Length <= 500 ? value : value[..500];
    }
}
