using HotChocolate.Data;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Services.GraphQL
{
    public sealed class UsersQueries
    {     
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<ApplicationUser> Read([Service] IUserService userService)
        {
            return userService.GetUsers();
        }
    }
}
