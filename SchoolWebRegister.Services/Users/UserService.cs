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
        public async Task<BaseResponse<ApplicationUser?>> GetUserById(string id)
        {
            try
            {
                ApplicationUser? user = await ValidateIfUserNotExist(id);

                return new BaseResponse<ApplicationUser?>
                {
                    Data = user,
                    StatusCode = StatusCode.Successful
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ApplicationUser?>
                {
                    Description = $"[GetUserById]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<BaseResponse<ApplicationUser?>> GetUserByLogin(string login)
        {
            try
            {
                var response = new BaseResponse<ApplicationUser?>();

                ApplicationUser? user = await _userManager.FindByNameAsync(login);
                if (user == null)
                {
                    response.Description = "Пользователь не найден.";
                    response.StatusCode = StatusCode.UserNotFound;
                }
                else
                    response.StatusCode = StatusCode.Successful;

                response.Data = user;

                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse<ApplicationUser?>
                {
                    Description = $"[GetUserById]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
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
        public async Task<BaseResponse<IEnumerable<ApplicationUser>>> GetUsers()
        {
            var response = new BaseResponse<IEnumerable<ApplicationUser>>();
            try
            {
                var users = await _userManager.Users.ToListAsync();
                if (users.Count() == 0)
                {
                    response.Description = "Список пользователей пуст.";
                    response.StatusCode = StatusCode.NoContent;
                }
                else
                    response.StatusCode = StatusCode.Successful;

                response.Data = users;

                return response;
            }
            catch (Exception ex) 
            {
                return new BaseResponse<IEnumerable<ApplicationUser>>
                {
                    Description = $"[GetUsers]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
