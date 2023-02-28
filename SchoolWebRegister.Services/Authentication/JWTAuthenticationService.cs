using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolWebRegister.Domain.Entity;
using Microsoft.AspNetCore.Http;

namespace SchoolWebRegister.Services.Authentication
{
    public sealed class JWTAuthenticationService : IAuthenticationService
    {
        private readonly string _validIssuer;
        private readonly string _validAudience;
        private readonly string _key;
        private readonly double _validityInDays;
        private readonly UserManager<ApplicationUser> _userManager;
        public JWTAuthenticationService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _validIssuer = config["JWT:ValidIssuer"];
            _validAudience = config["JWT:ValidAudience"];
            _key = config["JWT:Key"];
            _validityInDays = double.Parse(config["JWT:AccessTokenValidityInDays"]);
            _userManager = userManager;
        }

        private async Task<JwtSecurityToken> ValidateAndDecode(string jwt, IEnumerable<SecurityKey> signingKeys)
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
                var rawValidatedToken = await new JwtSecurityTokenHandler().ValidateTokenAsync(jwt, validationParameters);
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
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }
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

        public Task<bool> IsAuthenticated(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> SignIn(HttpContext context, ApplicationUser user, string password, bool isPersistent, bool lockOutOnFailure)
        {
            throw new NotImplementedException();
        }

        public Task SignOut()
        {
            throw new NotImplementedException();
        }
    }
}
