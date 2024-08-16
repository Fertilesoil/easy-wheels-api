using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EasyWheelsApi.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace EasyWheelsApi.Configuration
{
    public class TokenConfiguration(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(3),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, 
                ValidateIssuer = false, 
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!)
                ),
                ValidateLifetime = false, 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out securityToken
            );

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (
                jwtSecurityToken == null
                || !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
