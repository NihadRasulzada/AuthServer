using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        readonly UserManager<UserApp> _userManager;
        readonly CustomTokenOptions _customTokenOptions;
        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> options)
        {
            _userManager = userManager;
            _customTokenOptions = options.Value;
        }

        private string CreateRefreshToken()
        {
            Byte[] bytes = new Byte[32];
            using RandomNumberGenerator generator = RandomNumberGenerator.Create();
            generator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private IEnumerable<Claim> GetClaims(UserApp userApp, List<string> audiences)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email),
                new Claim(ClaimTypes.Name, userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return claims;
        }

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            List<Claim> claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());

            return claims;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            DateTime accessTokenExpiratin = DateTime.UtcNow.AddHours(4).AddMinutes(_customTokenOptions.AccessTokenExpiration);
            DateTime refreshTokenExpiratin = DateTime.UtcNow.AddHours(4).AddMinutes(_customTokenOptions.RefreshTokenExpiration);
            SecurityKey securityKey = SignService.GetSymmetricSecurityKey(_customTokenOptions.SecurityKey);

            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken securityToken = new JwtSecurityToken(issuer: _customTokenOptions.Issuer, expires: accessTokenExpiratin, notBefore: DateTime.UtcNow.AddHours(4), claims: GetClaims(userApp, _customTokenOptions.Audience), signingCredentials: credentials);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string token = handler.WriteToken(securityToken);

            TokenDto dto = new TokenDto()
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiratin,
                RefreshTokenExpiration = refreshTokenExpiratin,
                RefreshToken = CreateRefreshToken()
            };

            return dto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            DateTime accessTokenExpiratin = DateTime.UtcNow.AddHours(4).AddMinutes(_customTokenOptions.AccessTokenExpiration);
            SecurityKey securityKey = SignService.GetSymmetricSecurityKey(_customTokenOptions.SecurityKey);

            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken securityToken = new JwtSecurityToken(issuer: _customTokenOptions.Issuer, expires: accessTokenExpiratin, notBefore: DateTime.UtcNow.AddHours(4), claims: GetClaimsByClient(client), signingCredentials: credentials);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string token = handler.WriteToken(securityToken);

            ClientTokenDto dto = new ClientTokenDto()
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiratin
            };

            return dto;
        }
    }
}
