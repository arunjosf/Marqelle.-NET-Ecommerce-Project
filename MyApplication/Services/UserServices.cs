using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using System.Linq;

namespace Marqelle.Application.Services
{
    public class UserService : IUserServices
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordService _passwordService;

        public UserService(IUserRepository repository,
                           IPasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        public Users Register(RegisterRequestDto dto)
        {
            var existingUser = _repository.GetAll()
                .FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null)
                throw new Exception("Email already exists");

            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                RoleId = 1
            };

            user.Password = _passwordService.Hash(dto.Password, user);

            _repository.Register(user);

            return user;
        }

        public Users Login(string email, string password)
        {
            var user = _repository.GetAll()
                .FirstOrDefault(u => u.Email == email);

            if (user == null)
                return null;

        
            var isValid = _passwordService.Verify(
                user.Password,
                password,
                user);

            if (!isValid)
                return null;

            return user;
        }

        public void ChangePassword(long userId, string newPassword)
        {
            var user = _repository.GetById(userId);

            if (user == null)
                throw new Exception("User not found");

            var hashedPassword = _passwordService.Hash(newPassword, user);

            _repository.UpdateProfile(userId, null, null, null, hashedPassword);
        }

        public void UpdateProfile(long userId,
                                  string firstName,
                                  string lastName,
                                  string email,
                                  string password)
        {
            var user = _repository.GetById(userId);

            if (user == null)
                throw new Exception("User not found");

            string hashedPassword = null;

            if (!string.IsNullOrEmpty(password))
            {
                hashedPassword = _passwordService.Hash(password, user);
            }

            _repository.UpdateProfile(userId,
                                      firstName,
                                      lastName,
                                      email,
                                      hashedPassword);
        }

        public void UpdateRefreshToken(long userId,
                                       string refreshToken,
                                       DateTime expiry)
        {
            _repository.UpdateRefreshToken(userId, refreshToken, expiry);
        }

        public Users? ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var user = _repository.GetByRefreshToken(refreshToken);

            if (user == null)
                return null;

            if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return null;

            return user;
        }

        public void LogOut(long userId)
        {
            _repository.LogOut(userId);
        }
    }
}