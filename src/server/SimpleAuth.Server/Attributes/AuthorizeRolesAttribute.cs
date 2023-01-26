using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace SimpleAuth.Server.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            if (roles != null && roles.Length > 0)
                Roles = string.Join(",", roles);
        }
    }
}
