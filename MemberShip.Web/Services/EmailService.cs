using MemberShip.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MemberShip.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettingOptions _emailSettings = new();
        private readonly UserManager<AppUser> _userManager;

        public EmailService(IOptions<EmailSettingOptions> options,
            UserManager<AppUser> userManager)
        {
            _emailSettings = options.Value;
            _userManager = userManager;
        }

        public async Task SendResetEmail(string resetEmailLink, string toEmail)
        {
            var user = await _userManager.FindByEmailAsync(toEmail);

            var smtpClient = new SmtpClient();
            smtpClient.Host = _emailSettings.Host;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.Email);
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = "Localhost | Reset password link";
            mailMessage.Body = $@"<!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                    <meta charset=""UTF-8"">
                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                    <title>Password Reset</title>
                                </head>
                                <body style=""font-family: Arial, sans-serif;"">
                                
                                    <p>Dear {user.UserName},</p>
                                
                                    <p>We received a request to reset the password for your account. If you did not make this request, please ignore this email.    Otherwise,     click    the      link  below    to     reset your password:</p>
                                
                                    <p><a href='{resetEmailLink}'>Reset Your Password</a></p>
                                
                                    <p>This link will expire in 60 minutes, so be sure to use it promptly.</p>
                                
                                    <p>If you have any trouble with the link, copy and paste the following URL into your browser:</p>
                                    <p>{resetEmailLink}</p>
                                
                                    <p>Thank you,</p>
                                    <p>Your Company Name</p>
                                
                                </body>
                                </html>
                                ";
            mailMessage.IsBodyHtml = true;
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
