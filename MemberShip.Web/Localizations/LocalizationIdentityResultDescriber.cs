using Microsoft.AspNetCore.Identity;

namespace MemberShip.Web.Localizations
{
    public class LocalizationIdentityResultDescriber:IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new() { Code = "DuplicateUserName", Description = $"{userName} baska bir kullanici tarafindan alinmistir!" };
        }
    }
}
