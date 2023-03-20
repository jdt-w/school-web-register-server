using System.Security.Claims;

namespace SchoolWebRegister.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated(ClaimsPrincipal user);
        Task SignOut();
    }
}
