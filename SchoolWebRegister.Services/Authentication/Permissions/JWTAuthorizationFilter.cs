//using System.Text.Json;
//using Microsoft.AspNetCore.Http;
//using SchoolWebRegister.Services.Users;
//using SchoolWebRegister.Domain.Entity;
//using IAuthenticationService = SchoolWebRegister.Services.Authentication.IAuthenticationService;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Azure;
//using Microsoft.AspNetCore.Http.Features;
//using Microsoft.AspNetCore.Mvc.Abstractions;
//using Microsoft.AspNetCore.Mvc.Formatters;
//using Microsoft.AspNetCore.Mvc.Infrastructure;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Authentication;
//using System.Text;

//namespace SchoolWebRegister.Domain.Permissions
//{
//    public class AuthorizationRequirement : IAuthorizationRequirement
//    {
//        public AuthorizationRequirement()
//        {

//        }
//    }

//    public sealed class JWTAuthorizationFilter : AuthorizationHandler<AuthorizationRequirement>
//    {
//        private readonly IUserService _userService;
//        private readonly IAuthenticationService _authenticationService;
//        public JWTAuthorizationFilter(IUserService userService, IAuthenticationService authenticationService)
//        {
//            _userService = userService;
//            _authenticationService = authenticationService;
//        }

//        private async Task<string?> TryGetGUID(string? refreshToken)
//        {
//            if (string.IsNullOrEmpty(refreshToken)) return null;

//            try
//            {
//                var decode = await _authenticationService.ValidateAndDecode(refreshToken);
//                if (decode.Exception == null)
//                {
//                    if (decode.Claims.Any(claim => claim.Key.Equals("guid")))
//                    {
//                        return decode.Claims.FirstOrDefault(x => x.Key.Equals("guid")).Value.ToString();
//                    }
//                }
//                return null;
//            }
//            catch
//            {
//                return null;
//            }
//        }
//        private async Task<bool> CheckPermissions(string guid, IReadOnlyList<string> roles)
//        {
//            if (roles == null || roles.Count == 0)
//                return true;

//            if (roles != null && roles.Count > 0)
//            {
//                ApplicationUser? user = await _userService.GetUserById(guid);
//                if (user == null) return false;

//                foreach (string role in roles)
//                {
//                    if (Enum.TryParse(typeof(Permissions), role, out object? permission))
//                    {
//                        bool hasClaim = await _userService.UserHasClaim(user, "permission", role);
//                        if (!hasClaim) return false;
//                    }
//                }
//                return true;
//            }
//            return false;
//        }

//        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
//        {
//            HttpContext? httpContext = context.Resource as HttpContext;
//            string? accessToken = httpContext?.Request.Cookies["accessToken"];
//            string? refreshToken = httpContext?.Request.Cookies["refreshToken"];

//            var result = await _authenticationService.Authenticate(accessToken, refreshToken);

//            if (result is OkObjectResult)
//            {
//                var response = new BaseResponse(code: StatusCode.Success);
//                string jsonString = JsonSerializer.Serialize(response);
//                httpContext.Response.OnStarting(() =>
//                {
//                    var executor = httpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
//                    executor.HttpContext.Response.ContentType = "application/json";
//                    executor.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
//                    return Task.CompletedTask;
//                });
//                context.Succeed(requirement);
//            }
//            else
//            {
//                var response = new BaseResponse(
//                    code: StatusCode.Error,
//                    error: new ErrorType
//                    {
//                        Message = "Invalid Tokens",
//                        Type = new string[] { "AUTH_NOT_ALLOWED", "INVALID_TOKENS" }
//                    }
//                );
//                string jsonString = JsonSerializer.Serialize(response);
//                byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
//                _authenticationService.AppendInvalidCookies();
//                httpContext.Response.OnStarting(() =>
//                {
//                    var executor = httpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
//                    executor.HttpContext.Response.ContentType = "application/json";
//                    executor.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
//                    return Task.CompletedTask;
//                });
//            }
//        }

//        private Task WriteProblemDetailsAsync(HttpContext context, string detail)
//        {
//            var problemDetails = new ProblemDetails { Detail = detail, Status = context.Response.StatusCode };
//            var result = new ObjectResult(problemDetails)
//            {
//                ContentTypes = new MediaTypeCollection(),
//                StatusCode = problemDetails.Status,
//                DeclaredType = problemDetails.GetType(),
//            };
//            var executor = context.RequestServices.GetRequiredService<IActionResultExecutor<ObjectResult>>();
//            var routeData = context.GetRouteData() ?? new RouteData();
//            var actionContext = new ActionContext(context, routeData, new ActionDescriptor());
//            return executor.ExecuteAsync(actionContext, result);
//        }    

//    //public async ValueTask<AuthorizeResult> AuthorizeAsync(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives, CancellationToken cancellationToken = default)
//    //{
//    //    HttpContext? httpContext = context.ContextData.Values.FirstOrDefault(x => x is HttpContext) as HttpContext;
//    //    string? accessToken = httpContext?.Request.Cookies["accessToken"];
//    //    string? refreshToken = httpContext?.Request.Cookies["refreshToken"];

//    //    var result = await _authenticationService.Authenticate(accessToken, refreshToken);

//    //    if (result.StatusCode != StatusCode.Successful) return AuthorizeResult.NotAuthenticated;

//    //    string? guid = await TryGetGUID(refreshToken);
//    //    var directive = directives.FirstOrDefault(x => x.Roles != null && x.Roles.Any(role => Enum.TryParse(typeof(Permissions), role, out var result)));
//    //    bool permissionsPassed = await CheckPermissions(guid, directive?.Roles);

//    //    if (!permissionsPassed) return AuthorizeResult.NotAllowed;

//    //    return AuthorizeResult.Allowed;
//    //}
//}
//}