using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ziarah.Data;
using Ziarah.Services;

namespace Ziarah
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var runtimeBasePath = AppContext.BaseDirectory;
            builder.Configuration
                .AddJsonFile(Path.Combine(runtimeBasePath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(runtimeBasePath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: false);
            builder.Configuration.AddUserSecrets<Program>(optional: true);

            // Add services to the container.
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(45);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
            builder.Services.Configure<RegistrationOptions>(builder.Configuration.GetSection(RegistrationOptions.SectionName));

            builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

            builder.Services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Identity/Login/Login";
                    options.AccessDeniedPath = "/Guest/Home/Index";
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                DbSeeder.Seed(dbContext);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Guest}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
            
        }
    }
}
