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
            descriptor.BindFieldsExplicitly();
            descriptor.Field(f => f.Id);
            descriptor.Field(f => f.UserName);
            descriptor.Field(f => f.Email);
        }
    }

    [Authorize(Policy = "AllUsers")]
    public sealed class UsersQueries
    {
        //[UsePaging(typeof(ApplicationUser))]
        //[UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        [RequiresPermission(Permissions.Read)]
        public IQueryable<ApplicationUser> GetUsers([Service] IUserService userService)
        {
            return userService.GetUsers();
        }

        //[UsePaging(typeof(ApplicationUser))]
        //[UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        [RequiresPermission(Permissions.Read)]
        public IQueryable<ApplicationUser> GetCourse([Service] ICourseService courseService, int courseId)
        {
            return courseService.GetStudentsList(courseId);
        }
    }
}
