using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication
{
    public sealed class DefaultAuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        public DefaultAuthenticationService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public bool IsAuthenticated(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }
        public async Task<IActionResult> SignIn(HttpContext context, ApplicationUser user, string password, bool isPersistent, bool lockOutOnFailure)
        {
            ClaimsPrincipal claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                IsPersistent = isPersistent,
            };

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockOutOnFailure);
            if (signInResult.Succeeded)
            {
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
                return new OkObjectResult("Authenticated");
            }
            return new UnauthorizedResult();
        }
        public async Task SignOut() => await _signInManager.SignOutAsync();
    }
}
