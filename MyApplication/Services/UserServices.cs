using Marqelle.Application.DTO;
using Marqelle.Application.Helpers;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;

using System.Linq;

namespace Marqelle.Application.Services
{
    public class UserService : IUserServices
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public Users Register(RegisterRequestDto dto)
        {

            dto.Password = PasswordHasher.HashPassword(dto.Password);

            _repository.Register(dto);

            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                RoleId = 1
            };

            return user;
        }

        public Users Login(string email, string password)
        {
            var user = _repository.GetAll().FirstOrDefault(u => u.Email == email);
            if (user == null) return null;

            var hashedPassword = PasswordHasher.HashPassword(password);
            if (user.Password != hashedPassword) return null;

            var refreshToken = RefreshTokenHasher.GenerateRefreshToken();         
            var hashedToken = PasswordHasher.HashPassword(refreshToken);      
            var expiryTime = DateTime.UtcNow.AddDays(7);

            _repository.UpdateRefreshToken(user.Id, hashedToken, expiryTime);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;

            return user;
        }

        public void ChangePassword(long userId, string newPassword)
        {
            var hashedPassword = PasswordHasher.HashPassword(newPassword);

            _repository.UpdateProfile(userId, null, null, null, hashedPassword);
        }

        public void UpdateProfile(long userId, string firstName, string lastName, string email, string password)
        {
            var hashedPassword = PasswordHasher.HashPassword(password);

            _repository.UpdateProfile(userId, firstName, lastName, email, hashedPassword);
        }

        public void UpdateRefreshToken(long userId, string refreshToken, DateTime expiry)
        {
            _repository.UpdateRefreshToken(userId, refreshToken, expiry);
        }
    }
}
