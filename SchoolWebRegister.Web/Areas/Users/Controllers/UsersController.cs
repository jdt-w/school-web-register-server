using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Authentication;
using SchoolWebRegister.Services.Logging;
using SchoolWebRegister.Services.Users;
using SchoolWebRegister.Web.ViewModels.Account;
using System.Net;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SchoolWebRegister.Web.Areas.Users.Controllers
{
    [Authorize(Policy = "AllUsers")]
    [ApiController]
    [Route("[controller]")]
    public sealed class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILoggingService _logger;
        public UsersController(IUserService userService, IAuthenticationService authenticationService, ILoggingService logger)
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
                ApplicationUser? user = await _userService.ValidateCredentials(model.Email, model.UserPassword);

                if (user != null)
                {
                    var accessToken = await _authenticationService.CreateAccessJwtToken(user);
                    var refreshToken = await _authenticationService.CreateRefreshJwtToken(user, model.RememberMe);

                    await _authenticationService.SignIn(user, accessToken, refreshToken);

                    _logger.LogEventAction(new ActionLog
                    {
                        Component = "Login Page",
                        EventName = "Authentication",
                        EventDescription = $"User {user.Email} logged in.",
                        User = user,
                        InvolvedUser = user,
                        Context = "Login",
                        Source = "Login",
                        IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                    });
                    
                    return Ok(new AuthenticationResponse(user));
                }
                ModelState.AddModelError(nameof(LoginViewModel.Email), "Неверный логин или пароль");
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/users/login")]
        public async Task<IActionResult> Login(IFormCollection form)
        {
            if (form == null) return Unauthorized();

            bool.TryParse(form[nameof(LoginViewModel.RememberMe)], out bool rememberMe);

            return await Login(new LoginViewModel
            {
                Email = form[nameof(LoginViewModel.Email)],
                UserPassword = form[nameof(LoginViewModel.UserPassword)],
                RememberMe = rememberMe
            });
        }

        [HttpPost]
        [Route("/users/authenticate")]
        public async Task<IActionResult> Authenticate()
        {
            string? accessToken = Request.Cookies["accessToken"];
            string? refreshToken = Request.Cookies["refreshToken"];

            var result = await _authenticationService.Authenticate(accessToken, refreshToken);
            return result.Data;
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
