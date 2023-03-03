using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Authentication;
using SchoolWebRegister.Services.Users;
using SchoolWebRegister.Web.ViewModels.Account;

namespace SchoolWebRegister.Web.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly JWTAuthenticationService _authenticationService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IUserService userService, JWTAuthenticationService authenticationService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            _logger.LogWarning(returnUrl);
            ViewBag.ReturnUrl = returnUrl;
            return Ok();
        }        

        [AllowAnonymous]
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([FromForm] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _userService.GetUserByLogin(model.UserName);

                if (response.StatusCode == Domain.StatusCode.Successful)
                {
                    ApplicationUser? user = response.Data;

                    var result = await _authenticationService.CreateJwtToken(user);

                    _logger.LogInformation($"User {user.UserName} authenticated!", nameof(Login));
                    return Ok(new
                    {
                        token = result
                    });
                }
                ModelState.AddModelError(nameof(LoginViewModel.UserName), "Неверный логин или пароль");
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ResetPassword()
        {
            return Ok("Вы успешно сбросили пароль!");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await Task.CompletedTask;
            return Ok();
        }
    }
}
