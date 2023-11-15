using MemberShip.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace MemberShip.Web.CustomValidations
{
    public class UserValidations : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            var isNumber = int.TryParse(user.UserName![0].ToString(), out _);
            if(isNumber)
            {
                errors.Add(new() { Code = "UserNameFirstCharNumber", Description = "The first letter of username can not be number!" });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
