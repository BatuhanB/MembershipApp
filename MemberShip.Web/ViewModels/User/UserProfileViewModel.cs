using MemberShip.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels.User
{
    public class UserProfileViewModel
    {
        [Required(ErrorMessage = $"{nameof(Name)} can not be empty!")]
        public required string Name { get; set; }

        [Required(ErrorMessage = $"{nameof(Email)} can not be empty!")]
        [EmailAddress(ErrorMessage = "Email is not in correct form!")]
        public required string Email { get; set; }

        [Required(ErrorMessage = $"{nameof(PhoneNumber)} can not be empty!")]
        public required string PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }
        public string? City { get; set; }
        public Gender? Gender { get; set; }

        public IFormFile? Picture { get; set; }
    }
}
