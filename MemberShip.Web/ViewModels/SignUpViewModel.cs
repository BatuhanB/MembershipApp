using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels;
public class SignUpViewModel
{
    [Required(ErrorMessage ="This field can not be empty!")]
    public string UserName { get; set; }

    [EmailAddress(ErrorMessage ="Format is not correct!")]
    [Required(ErrorMessage = "This field can not be empty!")]
    public string Email { get; set; }

    [Required(ErrorMessage = "This field can not be empty!")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "This field can not be empty!")]
    public string Password { get; set; }

    [Compare("Password",ErrorMessage ="Password does not match!")]
    [Required(ErrorMessage = "This field can not be empty!")]
    public string PasswordConfirm { get; set; }
}