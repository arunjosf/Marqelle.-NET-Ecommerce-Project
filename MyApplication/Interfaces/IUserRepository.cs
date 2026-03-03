using Marqelle.Application.DTO;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IUserRepository
    {
        List<Users> GetAll();
        Users GetById(long id);
        void Register(Users user);
        void UpdateProfile(long userId, string firstName, string lastName, string email, string password);
        void Delete(long id);
        void UpdateRefreshToken(long userId, string refreshToken, DateTime expiryTime);
        void LogOut(long userId);
        Users GetByRefreshToken(string refreshToken);
    };
}
