using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string otpCode);
        Task SendVerificationEmailAsync(string toEmail, string otpCode);
        Task SendLoginNotificationAsync(string toEmail, string firstName);
        Task SendLogoutNotificationAsync(string toEmail, string firstName);
        Task SendPasswordResetNotificationAsync(string toEmail, string firstName);
    }
}

