using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated(ClaimsPrincipal user);
        Task<IActionResult> SignIn(HttpContext context, ApplicationUser user, string password, bool isPersistent, bool lockOutOnFailure);
        Task SignOut();
    }
}
