using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels
{
    public class SignInViewModel
    {
        [EmailAddress(ErrorMessage = "Format is not correct!")]
        [Required(ErrorMessage = "This field can not be empty!")]
        public required string Email { get; set; }
        
        [Required(ErrorMessage = "This field can not be empty!")]
        public required string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}
