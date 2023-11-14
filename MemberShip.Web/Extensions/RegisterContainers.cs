using MemberShip.Web.CustomValidations;
using MemberShip.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberShip.Web.Extensions
{
    public static class RegisterContainers
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgresqlConnection");
            services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));

            services.AddIdentity<AppUser, AppRoles>(opt =>
            {
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequiredLength = 5;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
            }).AddPasswordValidator<PasswordValidations>().AddEntityFrameworkStores<AppDbContext>();

        }
    }
}
