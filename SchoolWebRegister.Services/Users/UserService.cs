using System.Data;
using System.Data.Entity.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Users
{
    public sealed class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<ApplicationUser?> ValidateIfUserExist(string userId)
        {
            var existingEntity = await _userManager.FindByIdAsync(userId);
            if (existingEntity != null)
                throw new DuplicateNameException("Пользователь с таким Id уже существует.");
            return existingEntity;
        }
        private async Task<ApplicationUser?> ValidateIfUserNotExist(string userId)
        {
            var existingEntity = await _userManager.FindByIdAsync(userId);
            if (existingEntity == null)
                throw new ObjectNotFoundException("Пользователя с таким Id не существует.");
            return existingEntity;
        }

        public async Task<bool> IsUserInRole(ApplicationUser user, UserRole role)
        {
            return await _userManager.IsInRoleAsync(user, role.ToString());
        }
        public async Task<BaseResponse<bool>> CreateUser(ApplicationUser user)
        {
            try
            {
                await ValidateIfUserExist(user.Id);
                var result = await _userManager.CreateAsync(user);

                return new BaseResponse<bool>
                {
                    Data = result.Succeeded,
                    StatusCode = result.Succeeded ? StatusCode.Successful : StatusCode.BadRequest
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"[CreateUser]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<BaseResponse<bool>> DeleteUser(ApplicationUser user)
        {
            try
            {
                ApplicationUser? deleteUser = await ValidateIfUserNotExist(user.Id);
                var result = await _userManager.DeleteAsync(user);

                return new BaseResponse<bool>
                {
                    Data = result.Succeeded,
                    StatusCode = result.Succeeded ? StatusCode.Successful : StatusCode.BadRequest
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"[DeleteUser]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }
        public async Task<ApplicationUser?> GetUserById(string id)
        {
            try
            {
                ApplicationUser? user = await ValidateIfUserNotExist(id);
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ApplicationUser?> GetUserByLogin(string login)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login)) throw new ArgumentNullException(nameof(login));
                ApplicationUser? user = await _userManager.FindByNameAsync(login);                
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<BaseResponse<ApplicationUser>> UpdateUser(ApplicationUser modifiedUser)
        {
            try
            {
                ApplicationUser? editUser = await ValidateIfUserNotExist(modifiedUser.Id);

                var result = await _userManager.UpdateAsync(editUser);  
                return new BaseResponse<ApplicationUser>
                {
                    Data = editUser,
                    StatusCode = result.Succeeded ? StatusCode.Successful : StatusCode.BadRequest
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ApplicationUser>
                {
                    Description = $"[UpdateUser]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            try
            {
                var list = await _userManager.Users.ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<ApplicationUser>();
            }
        }
        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            try
            {
                if (user == null) throw new ArgumentNullException();
                var roles = await _userManager.GetRolesAsync(user);
                return roles;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}
