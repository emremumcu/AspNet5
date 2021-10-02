using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Filters
{
    public class RoleRequirementFilter : IAuthorizationFilter
    {
        private readonly string _role;

        public RoleRequirementFilter(string Role)
        {
            _role = Role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                // it isn't needed to set unauthorized result 
                // as the base class already requires the user to be authenticated
                // this also makes redirect to a login page work properly
                // context.Result = new UnauthorizedResult();
                return;
            }

            bool userHasRole = false;

            IEnumerable<Claim> roleClaims = context.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role);

            foreach(Claim c in roleClaims)
            {
                if (c.Value.Contains(_role)) userHasRole = true;
            }
            
            if (!userHasRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
