using Microsoft.AspNetCore.Identity;

namespace MemberShip.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string? City { get; set; }
    }
}
