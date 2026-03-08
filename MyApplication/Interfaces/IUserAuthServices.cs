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
    }
}
