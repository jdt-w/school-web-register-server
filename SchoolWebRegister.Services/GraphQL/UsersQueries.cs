using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Services.GraphQL
{
    public class UsersQueries
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
