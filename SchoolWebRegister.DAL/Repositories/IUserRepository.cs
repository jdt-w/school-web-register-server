using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public interface IUserRepository : IDbRepository<ApplicationUser>
    {
        Task<bool> IsUserInRole(ApplicationUser user, UserRole role);
        Task<IEnumerable<IdentityUserClaim<string>>> GetClaims(ApplicationUser user);
        Task<ApplicationUser?> GetUserByLoginAsync(string login);
        Task<IList<string>> GetUserRoles(ApplicationUser user);
        Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims);
        Task AddRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task RemoveClaimAsync(ApplicationUser user, Claim claim);
    }
}
