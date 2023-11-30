using MemberShip.Web.Models.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MemberShip.Web.TagHelpers
{
    public class UserPictureTagHelper : TagHelper
    {
        public string? ImageUrl { get; set; }
        public required Gender Gender { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            if (String.IsNullOrEmpty(ImageUrl))
            {
                switch (Gender)
                {
                    case Gender.Male:
                        output.Attributes.SetAttribute("src", $"/img/avatars/male-avatar.png");
                        break;
                    case Gender.Female:
                        output.Attributes.SetAttribute("src", $"/img/avatars/female-avatar-removebg-preview.png");
                        break;
                    default:
                        output.Attributes.SetAttribute("src", $"/img/avatars/male-avatar.png");
                        break;
                }
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/img/user-profile-pictures/{ImageUrl}");
            }
        }
    }
}
