using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using System.Linq;

namespace Marqelle.Application.Services
{
    public class UserAuthService : IUserAuthServices
    {
        private readonly IGenericRepository<Users> _repository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;


        public UserAuthService(
            IGenericRepository<Users> repository,
            IPasswordService passwordService,
            IEmailService emailService)
        {
            _repository = repository;
            _passwordService = passwordService;
            _emailService = emailService;
        }


        public async Task<RegisterResponseDto> Register(RegisterRequestDto dto)
        {

            var allUsers = await _repository.GetAllAsync();
            var expiredPending = allUsers
                .Where(u => u.Status == "Pending" && u.OtpExpiry < DateTime.UtcNow)
                .ToList();
            foreach (var expired in expiredPending)
            {
                _repository.Delete(expired);
            }
            if (expiredPending.Any())
                await _repository.SaveAsync();

            allUsers = await _repository.GetAllAsync();
            var existingUser = allUsers.FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null && existingUser.Status == "Active")
                throw new Exception("User already Registered.");

            if (existingUser != null && existingUser.Status == "Pending")
            {
                var newOtp = new Random().Next(100000, 999999).ToString();
                existingUser.OtpCode = newOtp;
                existingUser.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
                _repository.Update(existingUser);
                await _repository.SaveAsync();
                await _emailService.SendVerificationEmailAsync(dto.Email, newOtp);

                return new RegisterResponseDto
                {
                    Id = existingUser.Id,
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    Email = existingUser.Email
                };
            }

            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Password and Confirm Password must be same.");

            var otp = new Random().Next(100000, 999999).ToString();

            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                RoleId = 1,
                Status = "Pending",
                Blocked = false,
                OtpCode = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(10),
                RefreshToken = null,
                RefreshTokenExpiryTime = null
            };

            user.Password = _passwordService.Hash(dto.Password, user);

            await _repository.AddAsync(user);
            await _repository.SaveAsync();

            await _emailService.SendVerificationEmailAsync(dto.Email, otp);

            return new RegisterResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public async Task<Users?> Login(string email, string password)
        {
            var user = await _repository.FindAsync(u => u.Email == email);

            if (user == null)
                return null;

            if (user.Blocked)
                throw new Exception("Your account has been blocked.");

            if (user.Status == "Pending")
                throw new Exception("Please verify your email before logging in.");

            var isValid = _passwordService.Verify(user.Password, password, user);

            if (!isValid)
                return null;

            _ = _emailService.SendLoginNotificationAsync(user.Email, user.FirstName);

            return user;
        }

        public async Task<Users> VerifyEmailAsync(string otpCode)
        {
            var allUsers = await _repository.GetAllAsync();
            var user = allUsers.FirstOrDefault(u => u.OtpCode == otpCode && u.Status == "Pending");

            if (user == null)
                throw new Exception("Invalid OTP.");

            if (user.OtpExpiry < DateTime.UtcNow)
                throw new Exception("OTP has expired. Please request a new one.");

            user.Status = "Active";
            user.OtpCode = null;
            user.OtpExpiry = null;

            _repository.Update(user);
            await _repository.SaveAsync();

            return user;
        }

        public async Task ResendVerificationOtpAsync(string email)
        {
            var user = await _repository.FindAsync(u => u.Email == email);

            if (user == null)
                throw new Exception("No account found with that email.");

            if (user.Status == "Active")
                throw new Exception("Email is already verified.");

            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

            _repository.Update(user);
            await _repository.SaveAsync();

            await _emailService.SendVerificationEmailAsync(email, otp);
        }

        public async Task UpdateRefreshToken(long userId, string refreshToken, DateTime expiry)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return;

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiry;

            _repository.Update(user);
            await _repository.SaveAsync();
        }

        public async Task<Users?> ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var users = await _repository.GetAllAsync();

            foreach (var u in users)
            {
                if (string.IsNullOrEmpty(u.RefreshToken)) continue;

                var isValid = _passwordService.Verify(u.RefreshToken, refreshToken, u);

                if (isValid)
                {
                    if (u.RefreshTokenExpiryTime == null || u.RefreshTokenExpiryTime < DateTime.UtcNow)
                        return null;

                    return u;
                }
            }

            return null;
        }

        public async Task LogOut(long userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            _repository.Update(user);
            await _repository.SaveAsync();

            _ = _emailService.SendLogoutNotificationAsync(user.Email, user.FirstName);
        }

        public async Task ChangePasswordAsync(long userId, string currentPassword, string newPassword)
        {
            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            var isValid = _passwordService.Verify(user.Password, currentPassword, user);

            if (!isValid)
                throw new Exception("Current password is incorrect.");

            user.Password = _passwordService.Hash(newPassword, user);
            _repository.Update(user);
            await _repository.SaveAsync();
        }

        public async Task UpdateProfileAsync(long userId, string firstName, string lastName, string email)
        {
            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;

            _repository.Update(user);
            await _repository.SaveAsync();
        }

        public async Task ForgotPasswordAsync(string email, string newPassword)
        {
            var user = await _repository.FindAsync(u => u.Email == email);
            if (user == null)
                throw new Exception("No account found with that email address.");

            user.Password = _passwordService.Hash(newPassword, user);
            _repository.Update(user);
            await _repository.SaveAsync();
        }

        public async Task SendOtpAsync(string email)
        {
            var user = await _repository.FindAsync(u => u.Email == email);

            if (user == null)
                throw new Exception("No account found with that email address.");

            var otp = new Random().Next(100000, 999999).ToString();

            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);

            _repository.Update(user);
            await _repository.SaveAsync();

            await _emailService.SendOtpEmailAsync(email, otp);
        }

        public async Task VerifyOtpAndResetAsync(string otpCode, string newPassword)
        {
            var allUsers = await _repository.GetAllAsync();
            var user = allUsers.FirstOrDefault(u => u.OtpCode == otpCode);

            if (user == null)
                throw new Exception("Invalid OTP.");

            if (user.OtpExpiry < DateTime.UtcNow)
                throw new Exception("OTP has expired. Please request a new one.");

            user.Password = _passwordService.Hash(newPassword, user);
            user.OtpCode = null;
            user.OtpExpiry = null;

            _repository.Update(user);
            await _repository.SaveAsync();

            _ = _emailService.SendPasswordResetNotificationAsync(user.Email, user.FirstName);
        }

        public async Task SendChangeEmailOtpAsync(long userId, string newEmail)
        {
            var all = await _repository.GetAllAsync();
            if (all.Any(u => u.Email == newEmail && u.Id != userId))
                throw new Exception("This email is already in use.");

            var user = await _repository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");

            var otp = new Random().Next(100000, 999999).ToString();

            user.UpdatingEmail = newEmail;
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            _repository.Update(user);
            await _repository.SaveAsync();

            await _emailService.SendOtpEmailAsync(newEmail, otp);
        }
        public async Task VerifyAndChangeEmailAsync(string otpCode)
        {
            var all = await _repository.GetAllAsync();
            var user = all.FirstOrDefault(u => u.OtpCode == otpCode && u.UpdatingEmail != null);

            if (user == null)
                throw new Exception("Invalid OTP.");

            if (user.OtpExpiry < DateTime.UtcNow)
            {
                user.UpdatingEmail = null;
                user.OtpCode = null;
                user.OtpExpiry = null;
                _repository.Update(user);
                await _repository.SaveAsync();
                throw new Exception("OTP has expired. Please request a new one.");
            }

            user.Email = user.UpdatingEmail!;
            user.UpdatingEmail = null;
            user.OtpCode = null;
            user.OtpExpiry = null;

            _repository.Update(user);
            await _repository.SaveAsync();
        }
    }
}
