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
        private readonly HttpContext _context;

        public JWTAuthenticationService(IConfiguration config, IUserService userService, IHttpContextAccessor contextAccessor)
        {
            _validIssuer = config["JWT:ValidIssuer"];
            _validAudience = config["JWT:ValidAudience"];
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
            _accessValidityInDays = double.Parse(config["JWT:AccessTokenValidityInDays"]);
            _refreshDefaultValidityInDays = double.Parse(config["JWT:RefreshTokenDefaultValidityInDays"]);
            _refreshSmallValidityInDays = double.Parse(config["JWT:RefreshTokenSmallValidityInDays"]);
            _userService = userService;
            _context = contextAccessor.HttpContext;
        }

        private void AppendCookies(SecurityToken accessToken, SecurityToken refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            _context.Response.Cookies.Append("accessToken", tokenHandler.WriteToken(accessToken));
            _context.Response.Cookies.Append("refreshToken", tokenHandler.WriteToken(refreshToken));
        }
        private void AppendInvalidCookies()
        {
            CookieOptions options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                MaxAge = TimeSpan.Zero
            };
            _context.Response.Cookies.Append("accessToken", string.Empty, options);
            _context.Response.Cookies.Append("refreshToken", string.Empty, options);
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

        public bool IsAuthenticated(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }
        public async Task<IActionResult> SignIn(ApplicationUser user, SecurityToken accessToken, SecurityToken refreshToken)
        {
            AppendCookies(accessToken, refreshToken);
            return await Task.FromResult(new OkObjectResult(new AuthenticationResponse(user)));
        }
        public async Task SignOut() => await Task.Run(AppendInvalidCookies);
    }
}
