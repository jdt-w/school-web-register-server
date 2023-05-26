using System.Security.Claims;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Domain.Permissions;

namespace SchoolWebRegister.Services.Users
{
    public interface IUserService
    {
        bool ValidatePassword(ApplicationUser user, string password);
        Task<bool> IsUserInRole(ApplicationUser user, UserRole role);
        Task<bool> UserHasClaim(ApplicationUser user, string key, string value);
        Task<BaseResponse> CreateUser(ApplicationUser user);
        Task<BaseResponse> DeleteUser(string guid);
        Task<ApplicationUser?> GetUserById(string guid);
        Task<ApplicationUser?> GetUserByLogin(string login);
        Task<IList<string>> GetUserRoles(ApplicationUser user);
        Task<BaseResponse> UpdateUser(ApplicationUser user);
        Task GrantPermission(ApplicationUser user, Permissions permissions);
        Task RemovePermission(ApplicationUser user, params Permissions[] permissions);
        Task AddClaims(ApplicationUser user, IEnumerable<Claim> claims);
        Task AddToRoles(ApplicationUser user, IEnumerable<UserRole> roles);
        Task<ApplicationUser?> ValidateCredentials(string? login, string? password);
        Task<BaseResponse> ChangePassword(ApplicationUser user, string newPassword);
        IQueryable<ApplicationUser> GetUsers();
    }
}
