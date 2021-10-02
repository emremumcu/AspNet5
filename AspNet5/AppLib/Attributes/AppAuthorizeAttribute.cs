using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Attributes
{
    public class AppAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string Privileges { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Do NOT engage if AllowAnonymousAttribute exists
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) return;

            if (string.IsNullOrEmpty(Privileges))
            {
                // context.Result = new UnauthorizedResult();
                // return;
                throw new Exception("Privileges can not be empty.");
            }

            ClaimsPrincipal user = context.HttpContext.User;

            var userpriv = context.HttpContext.User.Claims.Where(x => x.Type == "Privilege").Select(x => x.Value).ToList();

            var CommonList = userpriv.Intersect(Privileges.Split(",").ToList());

            if (CommonList.Count() > 0)
            {
                return;
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
