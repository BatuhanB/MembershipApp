using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "This field can not be empty!")]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password does not match!")]
        [Required(ErrorMessage = "This field can not be empty!")]
        public required string PasswordConfirm { get; set; }
    }
}
