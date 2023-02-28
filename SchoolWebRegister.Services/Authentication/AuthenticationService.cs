using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthenticationService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> IsAuthenticated(ApplicationUser user)
        {
            ClaimsPrincipal claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            return _signInManager.IsSignedIn(claimsPrincipal);
        }
        public async Task<SignInResult> SignIn(HttpContext context, ApplicationUser user, string password, bool isPersistent, bool lockOutOnFailure)
        {
            ClaimsPrincipal claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                IsPersistent = isPersistent,
            };

            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockOutOnFailure);
            if (signInResult.Succeeded)
            {
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            }
            return signInResult;
        }
        public async Task SignOut() => await _signInManager.SignOutAsync();
    }
}
