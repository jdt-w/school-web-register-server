using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> IsAuthenticated(ApplicationUser user);
        Task<SignInResult> SignIn(HttpContext context, ApplicationUser user, string password, bool isPersistent, bool lockOutOnFailure);
        Task SignOut();
    }
}
