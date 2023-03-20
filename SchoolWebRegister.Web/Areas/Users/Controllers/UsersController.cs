using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Authentication;
using SchoolWebRegister.Services.Authentication.JWT;
using SchoolWebRegister.Services.Users;
using SchoolWebRegister.Web.ViewModels.Account;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SchoolWebRegister.Web.Areas.Users.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public sealed class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly JWTAuthenticationService _authenticationService;
        private readonly ILogger<UsersController> _logger;
        public UsersController(IUserService userService, JWTAuthenticationService authenticationService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/users/login")]
        public IActionResult Login(string returnUrl)
        {
            _logger.LogWarning(returnUrl);
            ViewBag.ReturnUrl = returnUrl;
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/users/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (model == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                ApplicationUser? user = await _userService.ValidateCredentials(model.UserName, model.UserPassword);

                if (user != null)
                {
                    var accessToken = await _authenticationService.CreateAccessJwtToken(user);
                    var refreshToken = await _authenticationService.CreateRefreshJwtToken(user, model.RememberMe);

                    await _authenticationService.SignIn(user, accessToken, refreshToken);

                    _logger.LogInformation($"User {user.UserName} authenticated!", nameof(Login));
                    
                    return Ok(new AuthenticationResponse(user));
                }
                ModelState.AddModelError(nameof(LoginViewModel.UserName), "Неверный логин или пароль");
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/users/login")]
        public async Task<IActionResult> Login(IFormCollection form)
        {
            if (form == null) return Unauthorized();

            bool rememberMe = bool.TryParse(form["RememberMe"], out rememberMe);

            return await Login(new LoginViewModel
            {
                UserName = form["UserName"],
                UserPassword = form["UserPassword"],
                RememberMe = rememberMe
            });
        }

        [HttpPost]
        [Route("/users/authenticate")]
        public async Task<IActionResult> Authenticate()
        {
            string? accessToken = Request.Cookies["accessToken"];
            string? refreshToken = Request.Cookies["refreshToken"];

            return await _authenticationService.Authenticate(accessToken, refreshToken);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/users/refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPost]
        [Route("/users/revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            await Task.CompletedTask;
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/users/logout")]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.SignOut();
            return new OkObjectResult("Status Code 440: Authentication token expired.");
        }
    }
}
