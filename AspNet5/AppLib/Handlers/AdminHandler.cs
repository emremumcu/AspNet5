using AspNet5.AppLib.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Handlers
{
    public class AdminHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            ClaimsPrincipal principal = context.User;

            if (principal.HasClaim(c => c.Type == "Administrator"))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
