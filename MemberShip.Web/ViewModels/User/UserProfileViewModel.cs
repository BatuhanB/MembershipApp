namespace MemberShip.Web.ViewModels.User
{
    public class UserProfileViewModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
