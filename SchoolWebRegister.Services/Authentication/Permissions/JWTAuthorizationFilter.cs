using Microsoft.AspNetCore.Http;
using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using SchoolWebRegister.Services.Users;
using SchoolWebRegister.Domain.Entity;
using IAuthenticationService = SchoolWebRegister.Services.Authentication.IAuthenticationService;
using Microsoft.Extensions.Logging.Abstractions;

namespace SchoolWebRegister.Domain.Permissions
{
    public sealed class JWTAuthorizationFilter : IAuthorizationHandler
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        public JWTAuthorizationFilter(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }

        private async Task<string?> TryGetGUID(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return null;

            try
            {
                var decode = await _authenticationService.ValidateAndDecode(refreshToken);
                if (decode.Exception == null)
                {
                    if (decode.Claims.Any(claim => claim.Key.Equals("guid")))
                    {
                        return decode.Claims.FirstOrDefault(x => x.Key.Equals("guid")).Value.ToString();
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        private async Task<bool> CheckPermissions(string guid, IReadOnlyList<string> roles)
        {
            if (roles == null || roles.Count == 0)
                return true;

            if (roles != null && roles.Count > 0)
            {
                ApplicationUser? user = await _userService.GetUserById(guid);
                if (user == null) return false;

                foreach (string role in roles)
                {
                    if (Enum.TryParse(typeof(Permissions), role, out object? permission))
                    {
                        bool hasClaim = await _userService.UserHasClaim(user, "permission", role);
                        if (!hasClaim) return false;
                    }
                }
                return true;
            }
            return false;
        }

        public ValueTask<AuthorizeResult> AuthorizeAsync(IMiddlewareContext context, AuthorizeDirective directive, CancellationToken cancellationToken = default)
        {
            return new ValueTask<AuthorizeResult>(AuthorizeResult.NotAuthenticated);
        }
        public async ValueTask<AuthorizeResult> AuthorizeAsync(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives, CancellationToken cancellationToken = default)
        {
            HttpContext? httpContext = context.ContextData.Values.FirstOrDefault(x => x is HttpContext) as HttpContext;
            string? accessToken = httpContext?.Request.Cookies["accessToken"];
            string? refreshToken = httpContext?.Request.Cookies["refreshToken"];

            var result = await _authenticationService.Authenticate(accessToken, refreshToken);

            if (result.StatusCode != StatusCode.Successful) return AuthorizeResult.NotAuthenticated;

            string? guid = await TryGetGUID(refreshToken);
            var directive = directives.FirstOrDefault(x => x.Roles != null && x.Roles.Any(role => Enum.TryParse(typeof(Permissions), role, out var result)));
            bool permissionsPassed = await CheckPermissions(guid, directive?.Roles);
            
            if (!permissionsPassed) return AuthorizeResult.NotAllowed;
            
            return AuthorizeResult.Allowed;
        }
    }
}