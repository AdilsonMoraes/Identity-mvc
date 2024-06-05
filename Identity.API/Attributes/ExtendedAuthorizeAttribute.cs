using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Identity.Services.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ExtendedAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string Role { get; }

        public ExtendedAuthorizeAttribute(string role)
        {
            Role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authenticationService = context.HttpContext.RequestServices.GetService<IAuthenticationService>();
            if (authenticationService != null)
            {
                if (!authenticationService.IsAdmin())
                {
                    var userComplete = authenticationService.GetUserWithRole();
                    if (userComplete.Roles.IsNullOrEmpty() || !userComplete.Roles.Contains(Role))
                    {
                        context.Result = new UnauthorizedObjectResult(string.Empty);
                    }
                }
            }
            else
            {
                context.Result = new UnauthorizedObjectResult(string.Empty);
            }
        }
    }
}
