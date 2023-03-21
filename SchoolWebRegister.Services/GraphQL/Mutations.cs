using HotChocolate.Authorization;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Domain.Permissions;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Services.GraphQL
{
    public class ResponseType<T> : ObjectType<BaseResponse>
    where T : class, IOutputType
    {
        protected override void Configure(
            IObjectTypeDescriptor<BaseResponse> descriptor)
        {
            descriptor.Field(f => f.StatusCode);
            descriptor.Field(f => f.Description);
            descriptor.Field(f => f.Data);
        }
    }
    public class GenericResponseType<T> : ObjectType<BaseResponse<T>>
    where T : class, IOutputType
    {
        protected override void Configure(
            IObjectTypeDescriptor<BaseResponse<T>> descriptor)
        {
            descriptor.Field(f => f.StatusCode);
            descriptor.Field(f => f.Description);
            descriptor
                .Field(f => f.Data)
                .Type<T>();
        }
    }

    [Authorize(Policy = "AllUsers")]
    public class Mutations
    {
        //[RequiresPermission(Permissions.Edit)]
        public async Task<BaseResponse> UpdateUserName([Service] IUserService userService, [ID] string guid, string newUserName)
        {
            ApplicationUser? user = await userService.GetUserById(guid);
            ApplicationUser? uniqueLogin = await userService.GetUserByLogin(newUserName);

            if (user == null) return new BaseResponse
            {
                StatusCode = StatusCode.UserNotFound,
                Description = "User GUID is not valid."
            };

            if (uniqueLogin != null) return new BaseResponse
            {
                StatusCode = StatusCode.BadRequest,
                Description = "New user name is not unique."
            };

            user.UserName = newUserName;

            var result = await userService.UpdateUser(user);
            return new BaseResponse
            {
                Description = result.Description,
                StatusCode = result.StatusCode,
                Data = result.Data
            };
        }

        //[RequiresPermission(Permissions.Edit)]
        public async Task<BaseResponse> UpdateUserPassword([Service] IUserService userService, [ID] string guid, string newUserPassword)
        {
            ApplicationUser? user = await userService.GetUserById(guid);

            if (user == null) return new BaseResponse
            {
                StatusCode = StatusCode.UserNotFound,
                Description = "User GUID is not valid."
            };

            var result = await userService.ChangePassword(user, newUserPassword);
            return new BaseResponse
            {
                StatusCode = result.StatusCode,
                Description = result.Description
            };
        }
    }
}