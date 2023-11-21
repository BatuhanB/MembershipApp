namespace MemberShip.Web.Services
{
    public interface IEmailService
    {
        Task SendResetEmail(string resetEmailLink,string toEmail);
    }
}
