namespace AspNet5.AppLib.Handlers
{
    using AspNet5.AppLib.Requirements;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// An authorization handler is responsible for the evaluation of a requirement's properties. 
    /// The authorization handler evaluates the requirements against a provided AuthorizationHandlerContext to determine if access is allowed.
    /// 
    /// A requirement can have multiple handlers. 
    /// A handler may inherit AuthorizationHandler<TRequirement>, where TRequirement is the requirement to be handled. 
    /// Alternatively, a handler may implement IAuthorizationHandler to handle more than one type of requirement.
    /// 
    /// Handlers are registered in the services collection during configuration.
    /// services.AddSingleton<IAuthorizationHandler, MyHandler>();
    /// Handlers can be registered using any of the built-in service lifetimes.
    /// 
    /// Note that the Handle method in the handler returns no value. How is a status of either success or failure indicated?
    /// 
    ///  * A handler indicates success by calling context.Succeed(IAuthorizationRequirement requirement), passing the requirement that has been successfully validated.
    ///  * A handler doesn't need to handle failures generally, as other handlers for the same requirement may succeed.
    ///  * To guarantee failure, even if other requirement handlers succeed, call context.Fail.
    ///  
    ///  If a handler calls context.Succeed or context.Fail, all other handlers are still called. 
    ///  This allows requirements to produce side effects, such as logging, which takes place even if another handler has successfully validated or failed a requirement.
    ///  When set to false, the InvokeHandlersAfterFailure property short-circuits the execution of handlers when context.Fail is called.
    ///  InvokeHandlersAfterFailure defaults to true, in which case all handlers are called.
    /// 
    ///  !!! Authorization handlers are called even if authentication fails. !!!
    /// 
    /// Reference:
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-5.0
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0
    /// </summary>

    public class BaseHandler : AuthorizationHandler<BaseRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BaseRequirement requirement)
        {
            try
            {
                if (context.Resource is HttpContext httpContext) 
                { 
                    var endpoint = httpContext.GetEndpoint(); 
                    var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>(); 
                }
                
                if (context.Resource is AuthorizationFilterContext mvcContext) 
                { 
                    // MVC-specific
                }          

                ClaimsPrincipal principal = context.User;

                if (!principal.HasClaim(c => c.Type == "Login")) return Task.CompletedTask;

                bool login = Convert.ToBoolean(principal.FindFirst("Login").Value);

                if (login == requirement.Login) context.Succeed(requirement);

                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
        }
    }

}




