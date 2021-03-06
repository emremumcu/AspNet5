namespace AspNet5.AppLib.Evaluators
{
    using AspNet5.AppLib.Abstract;
    using AspNet5.AppLib.Concrete;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Policy;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
   
    // https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authorization.Policy/PolicyEvaluator.cs
    // https://github.com/dotnet/aspnetcore/blob/main/src/Shared/SecurityHelper/SecurityHelper.cs
    // https://github.com/dotnet/aspnetcore/tree/main/src

    public class MockPolicyEvaluator : IPolicyEvaluator
    {
        NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        private readonly IAuthorize _authorizer;
        private readonly IAuthorizationService _authorization;

        public MockPolicyEvaluator(IAuthorize authorizer, IAuthorizationService authorization)
        {
            _authorizer = authorizer;
            _authorization = authorization;
        }

        /// <summary>
        /// Does authentication for <see cref="AuthorizationPolicy.AuthenticationSchemes"/> and sets the resulting
        /// <see cref="ClaimsPrincipal"/> to <see cref="HttpContext.User"/>.  If no schemes are set, this is a no-op.
        /// </summary>
        /// <param name="policy">The <see cref="AuthorizationPolicy"/>.</param>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns><see cref="AuthenticateResult.Success"/> unless all schemes specified by <see cref="AuthorizationPolicy.AuthenticationSchemes"/> failed to authenticate.
        public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            try
            {
                // If user requested the Account controller, do NOT engage
                if (context.Request.Path.Value.StartsWith("/Account"))
                    return await Task.FromResult(AuthenticateResult.NoResult());

                AuthenticationTicket ticket = _authorizer.GetTicket("usertest");

                // Set User
                context.User = ticket.Principal;

                // Return success
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                //context.Response.Redirect("/Account/Login");
                return await Task.FromResult(AuthenticateResult.Fail(ex.Message));
            }
        }        

        /// <summary>
        /// Attempts authorization for a policy using <see cref="IAuthorizationService"/>.
        /// </summary>
        /// <param name="policy">The <see cref="AuthorizationPolicy"/>.</param>
        /// <param name="authenticationResult">The result of a call to <see cref="AuthenticateAsync(AuthorizationPolicy, HttpContext)"/>.</param>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="resource">
        /// An optional resource the policy should be checked with.
        /// If a resource is not required for policy evaluation you may pass null as the value.
        /// </param>
        /// <returns>Returns <see cref="PolicyAuthorizationResult.Success"/> if authorization succeeds.
        /// Otherwise returns <see cref="PolicyAuthorizationResult.Forbid"/> if <see cref="AuthenticateResult.Succeeded"/>, otherwise
        /// returns  <see cref="PolicyAuthorizationResult.Challenge"/></returns>
        public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            if (policy == null)            
                throw new ArgumentNullException(nameof(policy));
            
            var result = await _authorization.AuthorizeAsync(context.User, resource, policy);

            if (result.Succeeded)            
                return PolicyAuthorizationResult.Success();
            
            // If authentication was successful, return forbidden, otherwise challenge
            return (authenticationResult.Succeeded) ? PolicyAuthorizationResult.Forbid() : PolicyAuthorizationResult.Challenge();
        }
    }
}
