using Dapper;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marqelle.Application.DTO;

namespace Marqelle.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public void Register(RegisterRequestDto dto)
        {
            string sql = @"INSERT INTO Users 
                           (FirstName, LastName, Email, Password)
                           VALUES 
                           (@FirstName, @LastName, @Email, @Password)";

            _db.Execute(sql, dto);
        }

        public List<Users> GetAll()
        {
            return _db.Query<Users>("SELECT * FROM Users").AsList();
        }

        public Users GetById(long id)
        {
            return _db.QueryFirstOrDefault<Users>(
                "SELECT * FROM Users WHERE Id = @Id",
                new { Id = id });
        }

        public void UpdateProfile(long userId, string firstName, string lastName, string email, string password)
        {
            string sql = @"UPDATE Users 
                           SET FirstName = @FirstName,
                               LastName = @LastName,
                               Email = @Email,
                               Password = @Password
                               WHERE Id = @UserId";

            _db.Execute(sql, new
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            });
        }

        public void Delete(long id)
        {
            _db.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = id });
        }

        public void UpdateRefreshToken(long userId, string refreshToken, DateTime expiryTime)
        {
            string sql = @"UPDATE Users
                   SET RefreshToken = @RefreshToken,
                       RefreshTokenExpiryTime = @ExpiryTime
                   WHERE Id = @UserId";

            _db.Execute(sql, new
            {
                UserId = userId,
                RefreshToken = refreshToken,
                ExpiryTime = expiryTime
            });
        }
    }
}
