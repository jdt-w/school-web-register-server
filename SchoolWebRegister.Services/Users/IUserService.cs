using System.Security.Claims;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Domain.Permissions;

namespace SchoolWebRegister.Services.Users
{
    public interface IUserService
    {
        Task<bool> IsUserInRole(ApplicationUser user, UserRole role);
        Task<BaseResponse<bool>> CreateUser(ApplicationUser user);
        Task<BaseResponse<bool>> DeleteUser(ApplicationUser user);
        Task<ApplicationUser?> GetUserById(string id);
        Task<ApplicationUser?> GetUserByLogin(string login);
        Task<IList<string>> GetUserRoles(ApplicationUser user);
        Task<BaseResponse<ApplicationUser>> UpdateUser(ApplicationUser user);
        Task GrantPermission(ApplicationUser user, params Permissions[] permissions);
        Task AddClaims(ApplicationUser user, IEnumerable<Claim> claims);
        Task AddToRoles(ApplicationUser user, IEnumerable<UserRole> roles);
        Task<ApplicationUser?> ValidateCredentials(string? login, string? password);
        Task<BaseResponse<bool>> ChangePassword(ApplicationUser user, string newPassword);
        IQueryable<ApplicationUser> GetUsers();
    }
}
