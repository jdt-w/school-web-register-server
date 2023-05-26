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

    public sealed class MutationType : ObjectType<Mutation>
    {
        protected override void Configure(IObjectTypeDescriptor<Mutation> descriptor)
        {
            descriptor.Authorize(new[] { Permissions.Edit.ToString() });
            descriptor.BindFieldsImplicitly();
        }
    }

    [Authorize]
    public class Mutation
    {
        public async Task<BaseResponse> UpdateUserPassword([Service] IUserService userService, [ID] string guid, string oldPassword, string newPassword)
        {
            ApplicationUser? user = await userService.GetUserById(guid);

            if (user == null) return new BaseResponse
            {
                StatusCode = StatusCode.UserNotFound,
                Description = "User GUID is not valid."
            };

            bool isPasswordValid = userService.ValidatePassword(user, oldPassword);
            if (!isPasswordValid) return new BaseResponse
            {
                StatusCode = StatusCode.BadRequest,
                Description = "Old password is not valid."
            };

            var result = await userService.ChangePassword(user, newPassword);
            return new BaseResponse
            {
                StatusCode = result.StatusCode,
                Description = result.Description
            };
        }
    }
}