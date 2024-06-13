using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TestTask.API.Model;

namespace TestTask.API.Helper
{
    public class JwtHelper : IJwtHelper
    {
        private readonly DataContext _context;
        private readonly string secretKey;
        private readonly string issuer;
        private readonly string audience;
        private readonly SymmetricSecurityKey signingKey;

        public JwtHelper(DataContext context, IConfiguration configuration)
        {
            _context = context;
            var jwtSettings = configuration.GetSection("JwtSettings");
            secretKey = jwtSettings.GetValue<string>("SecretKey");
            issuer = jwtSettings.GetValue<string>("Issuer");
            audience = jwtSettings.GetValue<string>("Audience");
            signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string GenerateJwtToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
      
                return userId;
            }
            catch
            {
                return null;
            }
        }

        public RefreshToken GenerateRefreshToken(string ipAddress, int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = getUniqueToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                CreatedDate = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                Id = userId
            };

            return refreshToken;

            string getUniqueToken()
            {
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var tokenIsUnique = !_context.IsTokenUnique(token);

                if (!tokenIsUnique)
                    return getUniqueToken();

                return token;
            }
        }
    }
}
