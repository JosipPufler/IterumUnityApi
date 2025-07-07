using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IterumApi.Services
{
    public class JwtTokenProvider
    {
        public static string CreateToken(string secureKey, int expiration, string subject, long userId, string? role = null)
        {
            var tokenKey = Encoding.UTF8.GetBytes(secureKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            List<Claim> claims = [
                new Claim(ClaimTypes.Name, subject), 
                new Claim("userId", userId.ToString()), 
                new Claim(JwtRegisteredClaimNames.Sub, subject)
            ];

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            tokenDescriptor.Subject = new ClaimsIdentity(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return serializedToken;
        }
    }
}
