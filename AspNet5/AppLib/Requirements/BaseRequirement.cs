using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Requirements
{
    /// <summary>    
    ///  An authorization requirement is a collection of data parameters that a policy can use to evaluate the current user principal. 
    ///  A requirement implements IAuthorizationRequirement, which is an empty marker interface.
    ///  
    ///  If an authorization policy contains multiple authorization requirements, all requirements must pass in order for the policy evaluation to succeed. 
    ///  In other words, multiple authorization requirements added to a single authorization policy are treated on an AND basis.
    ///  
    ///  Requirements are handled in AuthorizationHandlers.
    ///  Refer to BaseHandler in Handlers
    /// 
    ///  Reference:
    ///  https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-5.0
    ///  https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0
    /// </summary>

    public class BaseRequirement : IAuthorizationRequirement
    {
        public bool Login { get; private set; }

        public BaseRequirement(bool login)
        {
            Login = login;
        }
    }
}
