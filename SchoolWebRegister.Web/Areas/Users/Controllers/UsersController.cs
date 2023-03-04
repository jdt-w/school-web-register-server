using System.IdentityModel.Tokens.Jwt;
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
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser? user = await _userService.GetUserByLogin(model.UserName);

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
        [Route("/users/authenticate")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authenticate()
        {
            string? accessToken = Request.Cookies["accessToken"];
            string? refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken)) 
                return Unauthorized();

            var refreshResult = await _authenticationService.ValidateAndDecode(refreshToken);
            if (!refreshResult.IsValid)
            {
                await _authenticationService.SignOut();
                return Unauthorized();
            }
            
            var accessResult = await _authenticationService.ValidateAndDecode(accessToken);
            if (!accessResult.IsValid)
            {
                var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
                string? userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type.Equals("uid"))?.Value;
                ApplicationUser? user = await _userService.GetUserById(userId);

                if (user == null) return Unauthorized();

                var newAccessToken = await _authenticationService.CreateAccessJwtToken(user);
                await _authenticationService.SignIn(user, newAccessToken, refreshResult.SecurityToken);
            }
            return Ok();
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

        [HttpGet]
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await _userService.GetUsers();
        }

        [HttpGet("{id}")]
        public async Task<ApplicationUser?> GetUserById(string id)
        {
            return await _userService.GetUserById(id);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/users/logout")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.SignOut();
            return Content("Status Code 440: Authentication token expired.");
        }
    }
}
