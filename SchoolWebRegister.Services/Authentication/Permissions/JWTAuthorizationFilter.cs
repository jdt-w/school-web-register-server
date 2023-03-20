using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Services.Authentication.JWT;

namespace SchoolWebRegister.Domain.Permissions
{
    public class JWTAuthorizationFilter : IAuthorizationHandler
    {
        public readonly JWTAuthenticationService _authenticationService;
        public JWTAuthorizationFilter(JWTAuthenticationService authenticationService)
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

            return result is UnauthorizedResult ? AuthorizeResult.NotAuthenticated : AuthorizeResult.Allowed;
        }
    }
}