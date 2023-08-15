using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.Service.Helpers
{
    public class AccessTokenManager
    {
        public static string GenerateJwtToken(string name, int role, int? staffId, IConfiguration configuration)
        {
            var tokenConfig = configuration.GetSection("Token");
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfig["SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.NameIdentifier, staffId.ToString()),
            };

            if (role != 0)
            {
                permClaims.Add(new Claim(ClaimTypes.Role, ((SystemRoleEnum)role).ToString()));

                var newtoken = new JwtSecurityToken(tokenConfig["Issuer"],
                tokenConfig["Issuer"],
                permClaims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(newtoken);
            }

            var token = new JwtSecurityToken(tokenConfig["Issuer"],
                tokenConfig["Issuer"],
                permClaims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
