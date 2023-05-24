using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated(ClaimsPrincipal user);
        Task<TokenValidationResult> ValidateAndDecode(string? jwtToken);
        Task<JwtSecurityToken> CreateAccessJwtToken(ApplicationUser user);
        Task<JwtSecurityToken> CreateRefreshJwtToken(ApplicationUser user, bool rememberMe = true);
        Task<BaseResponse<IActionResult>> Authenticate(string? accessToken, string? refreshToken);
        Task<IActionResult> SignIn(ApplicationUser user, SecurityToken accessToken, SecurityToken refreshToken);
        Task SignOut();
    }
}
