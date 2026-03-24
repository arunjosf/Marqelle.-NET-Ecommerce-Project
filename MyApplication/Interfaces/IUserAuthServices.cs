using Marqelle.Application.DTO;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IUserAuthServices
    {
        Task<RegisterResponseDto> Register(RegisterRequestDto dto);
        Task<Users?> Login(string email, string password);
        Task UpdateRefreshToken(long userId, string refreshToken, DateTime expiry);
        Task<Users?> ValidateRefreshToken(string refreshToken);
        Task LogOut(long userId);
        Task<object> GetProfileAsync(long userId);
        Task ChangePasswordAsync(long userId, string currentPassword, string newPassword);
        Task UpdateProfileAsync(long userId, string firstName, string lastName, string email);
        Task ForgotPasswordAsync(string email, string newPassword);
        Task ResendVerificationOtpAsync(string email);
        Task SendOtpAsync(string email);
        Task VerifyOtpAndResetAsync(string otpCode, string newPassword);
        Task<Users> VerifyEmailAsync(string otpCode);
        Task SendChangeEmailOtpAsync(long userId, string newEmail);
        Task VerifyAndChangeEmailAsync(string otpCode);


    }
}
