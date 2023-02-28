using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Web.Areas.Admin.Controllers
{
    public sealed class UserController : Controller
    {
        private readonly IUserService _usersService;
        public UserController(IUserService userService)
        {
            _usersService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _usersService.GetUsers();
            if (response.StatusCode == Domain.StatusCode.Successful)
            {
                return View(response.Data);
            }
            return RedirectToAction("Error");
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        public async Task<IActionResult> DeleteUser(ApplicationUser user)
        {
            var response = await _usersService.DeleteUser(user);
            if (response.StatusCode == Domain.StatusCode.Successful)
            {
                return RedirectToAction("GetUsers");
            }
            return RedirectToAction("Error");
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        public async Task<IActionResult> CreateUser(ApplicationUser user)
        {
            var response = await _usersService.CreateUser(user);
            if (response.StatusCode == Domain.StatusCode.Successful)
            {
                return RedirectToAction("GetUsers");
            }
            return RedirectToAction("Error");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
