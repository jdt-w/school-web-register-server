using Microsoft.AspNetCore.Http;
using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using SchoolWebRegister.Services.Authentication;

namespace SchoolWebRegister.Domain.Permissions
{
    public sealed class JWTAuthorizationFilter : IAuthorizationHandler
    {
        public readonly IAuthenticationService _authenticationService;
        public JWTAuthorizationFilter(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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

            switch (result.StatusCode)
            {
                case StatusCode.Unauthorized: return AuthorizeResult.NotAuthenticated;
                case StatusCode.Forbidden: return AuthorizeResult.NotAllowed;
                case StatusCode.Successful: return AuthorizeResult.Allowed;
                default: return AuthorizeResult.NotAuthenticated;
            }
        }
    }
}