using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public sealed class UserController : Controller
    {
        private readonly IUserService _usersService;
        public UserController(IUserService userService)
        {
            _usersService = userService;
        }

        public async Task<IActionResult> DeleteUser(ApplicationUser user)
        {
            var response = await _usersService.DeleteUser(user);
            if (response.StatusCode == Domain.StatusCode.Successful)
            {
                return RedirectToAction("GetUsers");
            }
            return RedirectToAction("Error");
        }
        public async Task<IActionResult> CreateUser(ApplicationUser user)
        {
            var response = await _usersService.CreateUser(user);
            if (response.StatusCode == Domain.StatusCode.Successful)
            {
                return RedirectToAction("GetUsers");
            }
            return RedirectToAction("Error");
        }
    }
}
