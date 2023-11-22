using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels.User
{
    public class UserPasswordChangeViewModel
    {
        [Required(ErrorMessage = "This field can not be empty!")]
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "This field can not be empty!")]
        public required string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "Password does not match!")]
        [Required(ErrorMessage = "This field can not be empty!")]
        public required string ConfirmPassword { get; set; }
    }
}
