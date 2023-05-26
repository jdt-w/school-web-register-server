using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Authentication;
using SchoolWebRegister.Services.Logging;
using SchoolWebRegister.Services.Users;
using SchoolWebRegister.Web.ViewModels.Account;
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
        [Route("/log")]
        public async Task<IActionResult> LogEventAction(IFormCollection form)
        {
            if (int.TryParse(form[nameof(ActionLog.EventId)], out int id))
            {
                await _logger.LogEventAction(new ActionLog
                {
                    DateTime = DateTime.Now,
                    EventId = id,
                    Component = form[nameof(ActionLog.Component)],
                    EventName = form[nameof(ActionLog.EventName)],
                    EventDescription = form[nameof(ActionLog.EventDescription)],
                    UserId = form[nameof(ActionLog.UserId)],
                    InvolvedUserId = form[nameof(ActionLog.InvolvedUserId)],
                    Context = form[nameof(ActionLog.Context)],
                    Source = form[nameof(ActionLog.Source)],
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });
                return Ok();
            }
            else
                return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/users/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (model == null) return Unauthorized(new BaseResponse(            
                code: Domain.StatusCode.Error,
                error: "MISSING_DATA"
            ));

            if (ModelState.IsValid)
            {
                ApplicationUser? user = await _userService.ValidateCredentials(model.Email, model.UserPassword);

                if (user != null)
                {
                    var accessToken = await _authenticationService.CreateAccessJwtToken(user);
                    var refreshToken = await _authenticationService.CreateRefreshJwtToken(user, model.RememberMe);

                    await _authenticationService.SignIn(user, accessToken, refreshToken);

                    var roles = await _userService.GetUserRoles(user);

                    return Ok(new BaseResponse(                    
                        code: Domain.StatusCode.Success,
                        data: new AuthenticationResponse(user, roles)
                    ));
                }
            }

            return Unauthorized(new BaseResponse(
                code: Domain.StatusCode.Error,
                error: new ErrorType
                {
                    Message = "Неверный логин или пароль.",
                    Type = new string[] { "INVALID_DATA" },
                }
            ));
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
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPost]
        [Route("/users/changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel changePasswordModel)
        {
            ApplicationUser? user = await _userService.GetUserById(changePasswordModel.GUID);

            if (user == null)
                return BadRequest(new BaseResponse(
                    code: Domain.StatusCode.Error,
                    error: new ErrorType
                    {
                        Message = "Неверный GUID пользователя.",
                        Type = new string[] { "INVALID_DATA" }
                    }
                ));

            bool isPasswordValid = _userService.ValidatePassword(user, changePasswordModel.OldPassword);
            if (!isPasswordValid)
                return BadRequest(new BaseResponse(
                   code: Domain.StatusCode.Error,
                   error: new ErrorType
                   {
                       Message = "Неверный старый пароль.",
                       Type = new string[] { "INVALID_DATA" }
                   }
               ));

            var result = await _userService.ChangePassword(user, changePasswordModel.NewPassword);
            if (result.Status == Domain.StatusCode.Success.ToString())
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
