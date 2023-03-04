using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SchoolWebRegister.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated(ClaimsPrincipal user);
        Task SignOut();
    }
}
