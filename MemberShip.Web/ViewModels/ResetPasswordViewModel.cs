using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "Format is not correct!")]
        [Required(ErrorMessage = "This field can not be empty!")]
        public required string Email { get; set; }
    }
}
