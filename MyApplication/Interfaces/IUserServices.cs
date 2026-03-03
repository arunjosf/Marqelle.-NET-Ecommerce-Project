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
    public interface IUserServices
    {
        Users Register(RegisterRequestDto dto);
        Users Login(string email, string password);

        void ChangePassword(long userId, string newPassword);
        void UpdateProfile(long userId, string firstName, string lastName, string email, string password);
        void UpdateRefreshToken(long userId, string refreshToken, DateTime expiry);
        Users ValidateRefreshToken(string refreshToken);
        void LogOut(long userId);
    }
}
