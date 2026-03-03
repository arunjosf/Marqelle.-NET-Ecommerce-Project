using Dapper;
using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Application.Services;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Marqelle.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;
        private readonly IPasswordService _passwordService;

        public UserRepository(IDbConnection db, IPasswordService passwordService)
        {
            _db = db;
            _passwordService = passwordService;
        }

        public void Register(Users user)
        {
            string sql = @"INSERT INTO Users 
                           (FirstName, LastName, Email, Password, RoleId, Blocked, Status, RefreshToken, RefreshTokenExpiryTime)
                           VALUES 
                           (@FirstName, @LastName, @Email, @Password, @RoleId, @Blocked, @Status, @RefreshToken, @RefreshTokenExpiryTime)";

            if (user.RoleId == 0)
                user.RoleId = 1;

            if (user.Blocked == false) user.Blocked = false;

            if (string.IsNullOrEmpty(user.Status)) user.Status = "Active";

            if (string.IsNullOrEmpty(user.RefreshToken))
                user.RefreshToken = ""; 
            user.RefreshTokenExpiryTime = null; 
            _db.Execute(sql, user);
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

        public Users? GetByRefreshToken(string refreshToken)
        {
 
            var usersWithTokens = _db.Query<Users>("SELECT * FROM Users WHERE RefreshToken IS NOT NULL");

            foreach (var user in usersWithTokens)
            {
                if (_passwordService.Verify(user.RefreshToken, refreshToken, user))
                {
                    if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                        return null;

                    return user;
                }
            }

            return null;
        }
        public void LogOut(long userId)
        {
            string sql = @"UPDATE Users SET RefreshToken = '',
                          RefreshTokenExpiryTime = NULL
                          WHERE Id = @userId";
            _db.Execute(sql, new { UserId = userId });
        }
    }
}
