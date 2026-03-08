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

        public UserAuthService(IGenericRepository<Users> repository,
                           IPasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        public async Task<RegisterResponseDto> Register(RegisterRequestDto dto)
        {
            var users = await _repository.GetAllAsync();
            var existingUser = users.FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null)
                throw new Exception("Email already exists");

            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                RoleId = 1,
                Status = "Active",
                Blocked = false,
                RefreshToken = null,
                RefreshTokenExpiryTime = null
            };

            user.Password = _passwordService.Hash(dto.Password, user);

            await _repository.AddAsync(user);
            await _repository.SaveAsync();

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

        
            var isValid = _passwordService.Verify(
                          user.Password,
                          password,
                          user);

            if (!isValid)
                return null;

            return user;
        }


        public async Task UpdateRefreshToken(long userId, string refreshToken, DateTime expiry)
        {
            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
            {
                return;
            }
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiry;

            _repository.UpdateAsync(user);
            await _repository.SaveAsync();
        }

        public async Task<Users?> ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var users = await _repository.GetAllAsync();

            foreach (var u in users)
            {
                if (string.IsNullOrEmpty(u.RefreshToken))
                    continue;

                var isValid = _passwordService.Verify(u.RefreshToken, refreshToken, u);

                if (isValid)
                {
                    if (u.RefreshTokenExpiryTime == null ||
                        u.RefreshTokenExpiryTime < DateTime.UtcNow)
                        return null;

                    return u;
                }
            }

            return null;
        }

        public async Task LogOut(long userId)
        {
            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
                return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            _repository.UpdateAsync(user);
            await _repository.SaveAsync();
        }
    }
}