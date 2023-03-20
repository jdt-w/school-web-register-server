using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AuthorizeAttribute = HotChocolate.Authorization.AuthorizeAttribute;

namespace SchoolWebRegister.Domain.Permissions
{
    public sealed class RequiresPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly IEnumerable<Claim> _claims;
        public RequiresPermissionAttribute(Permissions permissions)
        {
            var bits = Enum.GetValues(typeof(Permissions))
                         .Cast<Enum>()
                         .Where(flag => permissions.HasFlag(flag));

            _claims = bits.Select(p => new Claim("permission", p.ToString()));
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            foreach (Claim claim in _claims)
            {
                bool hasClaim = context.HttpContext.User.Claims.Any(c => c.Equals(claim));
                if (!hasClaim)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            context.Result = new OkResult();
        }
    }
}