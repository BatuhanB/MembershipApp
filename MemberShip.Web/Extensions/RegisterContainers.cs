using MemberShip.Web.CustomValidations;
using MemberShip.Web.Localizations;
using MemberShip.Web.Models;
using MemberShip.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MemberShip.Web.Extensions
{
    public static class RegisterContainers
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgresqlConnection");
            services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));


            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(1);
            });

            services.Configure<EmailSettingOptions>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddIdentity<AppUser, AppRoles>(opt =>
            {
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequiredLength = 5;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            }).AddPasswordValidator<PasswordValidations>()
            .AddUserValidator<UserValidations>()
            .AddErrorDescriber<LocalizationIdentityResultDescriber>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AppDbContext>();

        }
    }
}
