using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupFAmof.Service.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.Service.Helpers
{
    public class AccessTokenManager
    {
        public static string GenerateJwtToken(string name, int role, int? accountId, IConfiguration configuration)
        {
            //2 minutes
            var ExpireDate = Ultils.GetCurrentDatetime().AddDays(0.00138);
            
            //30 days
            //var ExpireDate = Ultils.GetCurrentDatetime().AddDays(30);

            var tokenConfig = configuration.GetSection("Token");
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfig["SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.NameIdentifier, accountId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim(ClaimTypes.Expired, ExpireDate.ToString()),

            };

            var token = new JwtSecurityToken(tokenConfig["Issuer"],
                tokenConfig["Issuer"],
                permClaims,
                //expires: DateTime.Now.AddDays(30),
                expires: ExpireDate,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
