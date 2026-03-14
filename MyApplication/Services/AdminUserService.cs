using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IGenericRepository<Users> _userRepository;

        public AdminUserService(IGenericRepository<Users> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<AdminUserManagementDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users
                .Where(u => u.RoleId == 1) 
                .OrderBy(u => u.FirstName)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<List<AdminUserManagementDto>> SearchUsersAsync(long? id, string? name)
        {
            var users = await _userRepository.GetAllAsync();

            var query = users
                .Where(u => u.RoleId == 1)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(u => u.Id == id.Value);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(name.ToLower()) ||
                    u.LastName.ToLower().Contains(name.ToLower()));

            return query.Select(MapToDto).ToList();
        }

        public async Task BlockUserAsync(long userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            if (user.RoleId == 2)
                throw new Exception("Admin accounts cannot be blocked.");

            if (user.Blocked)
                throw new Exception("User is already blocked.");

            user.Blocked = true;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
        }

        public async Task UnblockUserAsync(long userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            if (!user.Blocked)
                throw new Exception("User is not blocked.");

            user.Blocked = false;

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
        }

        private static AdminUserManagementDto MapToDto(Users u) => new AdminUserManagementDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Blocked = u.Blocked
        };
    }
}

