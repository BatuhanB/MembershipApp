using MemberShip.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace MemberShip.Web.TagHelpers
{
    public class UserRolesTagHelper:TagHelper
    {
        public required string UserId { get; set; }

        private readonly UserManager<AppUser> _userManager;

        public UserRolesTagHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            var stringBuilder = new StringBuilder();
            foreach ( var role in userRoles)
            {
                stringBuilder.Append(@$"<span class='badge bg-success mx-1'>{role.ToLower()}</span>");
            }

            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}
