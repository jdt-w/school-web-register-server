using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Services.Authentication.JWT
{
    public sealed class JWTAuthenticationService : IAuthenticationService
    {
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly SymmetricSecurityKey _key;
        private readonly double _accessValidityInDays;
        private readonly double _refreshDefaultValidityInDays;
        private readonly double _refreshSmallValidityInDays;
        private readonly IUserService _userService;
        public JWTAuthenticationService(IConfiguration config, IUserService userService)
        {
            _validIssuer = config["JWT:ValidIssuer"];
            _validAudience = config["JWT:ValidAudience"];
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
            _accessValidityInDays = double.Parse(config["JWT:AccessTokenValidityInDays"]);
            _refreshDefaultValidityInDays = double.Parse(config["JWT:RefreshTokenDefaultValidityInDays"]);
            _refreshSmallValidityInDays = double.Parse(config["JWT:RefreshTokenSmallValidityInDays"]);
            _userService = userService;
        }

        public async Task<TokenValidationResult> ValidateAndDecode(string jwtToken)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKeys = new[] { _key },
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = _validAudience,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _validIssuer
                };
                return await new JwtSecurityTokenHandler().ValidateTokenAsync(jwtToken, validationParameters);
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
        public async Task<JwtSecurityToken> CreateAccessJwtToken(ApplicationUser user)
        {
            var roles = await _userService.GetUserRoles(user);
            var roleClaims = roles.Select(role => new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id),
                new Claim("username", user.UserName)
            }
            .Union(roleClaims);

            DateTime now = DateTime.UtcNow;

            var signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                notBefore: now,
                claims: claims,
                expires: now.AddDays(_accessValidityInDays),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        public async Task<JwtSecurityToken> CreateRefreshJwtToken(ApplicationUser user, bool rememberMe = true)
        {
            var roles = await _userService.GetUserRoles(user);
            var roleClaims = roles.Select(role => new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id),
                new Claim("username", user.UserName)
            }
            .Union(roleClaims);

            DateTime now = DateTime.UtcNow;

            var signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                notBefore: now,
                claims: claims,
                expires: now.AddDays(rememberMe ? _refreshDefaultValidityInDays : _refreshSmallValidityInDays),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        public void AppendCookies(HttpContext context, SecurityToken accessToken, SecurityToken refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            context.Response.Cookies.Append("accessToken", tokenHandler.WriteToken(accessToken));
            context.Response.Cookies.Append("refreshToken", tokenHandler.WriteToken(refreshToken));
        }
        public void AppendInvalidCookies(HttpContext context, CookieOptions options)
        {
            context.Response.Cookies.Append("accessToken", string.Empty, options);
            context.Response.Cookies.Append("refreshToken", string.Empty, options);
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
