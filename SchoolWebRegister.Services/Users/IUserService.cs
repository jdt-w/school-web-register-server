using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Users
{
    public interface IUserService
    {
        Task<bool> IsUserInRole(ApplicationUser user, UserRole role);
        Task<BaseResponse<bool>> CreateUser(ApplicationUser user);
        Task<BaseResponse<bool>> DeleteUser(ApplicationUser user);
        Task<BaseResponse<ApplicationUser?>> GetUserById(string id);
        Task<BaseResponse<ApplicationUser?>> GetUserByLogin(string login);
        Task<IList<string>> GetUserRoles(ApplicationUser user);
        Task<BaseResponse<ApplicationUser>> UpdateUser(ApplicationUser user);
        Task<BaseResponse<IEnumerable<ApplicationUser>>> GetUsers();
    }
}
