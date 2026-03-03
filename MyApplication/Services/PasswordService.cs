using Microsoft.AspNetCore.Identity;
using Marqelle.Domain.Entities;
using Marqelle.Application.Interfaces;

namespace Marqelle.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<Users> _hasher;

        public PasswordService()
        {
            _hasher = new PasswordHasher<Users>();
        }

        public string Hash(string password, Users user)
        {
            return _hasher.HashPassword(user, password);
        }

        public bool Verify(string hashedValue, string plainValue, Users user)
        {
            var result = _hasher.VerifyHashedPassword(
                user,
                hashedValue,
                plainValue
            );

            return result == PasswordVerificationResult.Success;
        }
    }
}
