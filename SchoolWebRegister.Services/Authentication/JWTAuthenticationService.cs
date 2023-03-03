using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Services.Authentication
{
    public sealed class JWTAuthenticationService : IAuthenticationService
    {
        private readonly string _validIssuer;
        private readonly string _validAudience;
        private readonly string _key;
        private readonly double _validityInDays;
        private readonly IUserService _userService;
        public JWTAuthenticationService(IConfiguration config, IUserService userService)
        {
            _validIssuer = config["JWT:ValidIssuer"];
            _validAudience = config["JWT:ValidAudience"];
            _key = config["JWT:Key"];
            _validityInDays = double.Parse(config["JWT:AccessTokenValidityInDays"]);
            _userService = userService;
        }

        private async Task<JwtSecurityToken> ValidateAndDecode(string jwtToken, IEnumerable<SecurityKey> signingKeys)
        {
            var validationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(5),
                IssuerSigningKeys = signingKeys,
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,       
                ValidateAudience = true,
                ValidAudience = _validAudience,
                ValidateIssuer = true,
                ValidIssuer = _validIssuer
            };

            try
            {
                var rawValidatedToken = await new JwtSecurityTokenHandler().ValidateTokenAsync(jwtToken, validationParameters);
                return (JwtSecurityToken)rawValidatedToken.SecurityToken;
            }
            catch (SecurityTokenValidationException stvex)
            {
                throw new Exception($"Token failed validation: {stvex.Message}");
            }
            catch (ArgumentException argex)
            {
                throw new Exception($"Token was invalid: {argex.Message}");
            }
        }
        public async Task<string> CreateJwtToken(ApplicationUser user)
        {
            var roles = await _userService.GetUserRoles(user);
            var roleClaims = roles.Select(role => new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id)
            }
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_validityInDays),
                signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public bool IsAuthenticated(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }
        public Task<IActionResult> SignIn(HttpContext context, ApplicationUser user, string password, bool isPersistent, bool lockOutOnFailure)
        {
            throw new NotImplementedException();
        }
        public Task SignOut()
        {
            throw new NotImplementedException();
        }
    }
}
