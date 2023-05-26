using HotChocolate.Authorization;
using HotChocolate.Data;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Domain.Permissions;
using SchoolWebRegister.Services.Courses;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Services.GraphQL
{
    public class ApplicationUserType : ObjectType<ApplicationUser>
    {
        protected override void Configure(IObjectTypeDescriptor<ApplicationUser> descriptor)
        {
            descriptor.Authorize(new string[] { Permissions.Read.ToString() });
            descriptor.Field(f => f.Claims).Ignore();
            descriptor.Field(f => f.UserRoles).Ignore();
            descriptor.Field(f => f.PasswordHash).Ignore();
            descriptor.Field(f => f.SecurityStamp).Ignore();
            descriptor.Field(f => f.ConcurrencyStamp).Ignore();
        }
    }

    [Authorize]
    public sealed class UsersQueries
    {
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<ApplicationUser> GetUsers([Service] IUserService userService)
        {
            return userService.GetUsers();
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<ApplicationUser> GetCourse([Service] ICourseService courseService, int courseId)
        {
            return courseService.GetStudentsList(courseId);
        }
    }
}
