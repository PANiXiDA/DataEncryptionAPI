using Common.Configuration;
using Common.Configuration.Models;
using Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tools.JsonWebTokenEncryption.Models;

namespace Tools.JsonWebTokenEncryption
{
    public class JsonWebTokenEncryption
    {
        private readonly JsonWebTokenConfiguration _jsonWebTokenConfiguration;

        public JsonWebTokenEncryption(IOptions<SharedConfiguration> sharedConfiguration)
        {
            _jsonWebTokenConfiguration = sharedConfiguration.Value.JsonWebTokenConfiguration;
        }

        public JsonWebTokenData GenerateAccessToken(User user, string userAgent, string? ipAddress)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("userAgent", userAgent),
            new Claim("ipAddress", ipAddress ?? "Unknown ip")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jsonWebTokenConfiguration.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jsonWebTokenConfiguration.Issuer,
                audience: _jsonWebTokenConfiguration.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jsonWebTokenConfiguration.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            var tokenData = new JsonWebTokenData()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = GenerateRefreshToken(),
                CreatedAt = DateTime.UtcNow,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jsonWebTokenConfiguration.AccessTokenExpirationMinutes),
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jsonWebTokenConfiguration.RefreshTokenExpirationDays)
            };


            return tokenData;
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
