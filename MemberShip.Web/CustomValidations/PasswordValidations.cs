using MemberShip.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace MemberShip.Web.CustomValidations
{
    public class PasswordValidations : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();
            if (password!.ToLower().Contains(user.UserName!.ToLower()))
            {
                errors.Add(new() { Code = "PasswordContainUserName", Description = "Password can not contain username" });
            }

            if (password!.ToLower().StartsWith("12345"))
            {
                errors.Add(new() { Code = "PasswordStartWithNumbers", Description = "Password can not start with 12345" });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
