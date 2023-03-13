using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public sealed class HomeController : Controller
    {
        private readonly IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View(_userService.GetUsers());
        }
    }
}
