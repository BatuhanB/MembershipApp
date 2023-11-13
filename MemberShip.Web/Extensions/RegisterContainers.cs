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
        }
    }
}
