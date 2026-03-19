using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace Marqelle.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
            private readonly IConfiguration _configuration;

            public JwtService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public string GenerateToken(Users user)
            {
                var jwtSettings = _configuration.GetSection("jwt");
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["key"]));
                var tokenDescriptor = new SecurityTokenDescriptor

                {
                    Subject = new ClaimsIdentity(new[]
                        {
                   new Claim("UserId", user.Id.ToString(), ClaimValueTypes.Integer64),
                   new Claim(ClaimTypes.Email, user.Email),
                   new Claim("role", user.RoleId.ToString(), ClaimValueTypes.Integer)


                }),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationMinutes"])),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }

