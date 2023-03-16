using System.Data;
using System.Data.Entity.Core;
using System.Security.Claims;
using GreenDonut;
using Microsoft.AspNetCore.Identity;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Domain.Permissions;

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
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException();

            var existingEntity = await _userManager.FindByIdAsync(userId);
            if (existingEntity != null)
                throw new DuplicateNameException("Пользователь с таким Id уже существует.");
            return existingEntity;
        }
        private async Task<ApplicationUser?> ValidateIfUserNotExist(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException();

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
        public IQueryable<ApplicationUser> GetUsers()
        {
            try
            {
                return _userManager.Users.AsQueryable();
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<ApplicationUser>().AsQueryable();
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
        public async Task GrantPermission(ApplicationUser user, params Permissions[] permissions)
        {
            foreach (Permissions permission in permissions)
            {
                string str = string.Concat(permission.GetType().Namespace, ".", permission.ToString());
                await _userManager.AddClaimAsync(user, new Claim("permission", str));
            }
        }
        public async Task AddClaims(ApplicationUser user, IEnumerable<Claim> claims)
        {
            await _userManager.AddClaimsAsync(user, claims);
        }
        public async Task AddToRoles(ApplicationUser user, IEnumerable<UserRole> roles)
        {
            await _userManager.AddToRolesAsync(user, roles.Select(x => x.ToString()));

            var claims = roles.Select(x => new Claim(ClaimTypes.Role, x.ToString()));
            await AddClaims(user, claims);
        }
        public async Task<BaseResponse<bool>> ChangePassword(ApplicationUser user, string newPassword)
        {
            try
            {
                bool isValid = await _userManager.CheckPasswordAsync(user, newPassword);

                if (!isValid)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.Forbidden,
                        Description = "New password doesn't match security requirements."
                    };
                }

                string newPasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
                if (!newPasswordHash.Equals(user.PasswordHash))
                {
                    user.PasswordHash = newPasswordHash;
                    var result = await UpdateUser(user);
                    return new BaseResponse<bool>
                    {
                        Data = result.StatusCode == StatusCode.Successful ? true : false,
                        Description = result.Description,
                        StatusCode = result.StatusCode
                    };
                }

                return new BaseResponse<bool>
                {
                    Data = false,
                    StatusCode = StatusCode.BadRequest,
                    Description = "New password is already equals old password"
                };
            }
            catch
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
