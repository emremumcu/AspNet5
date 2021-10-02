using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Policies
{
    public static class AuthorizationPolicies
    {
        public static AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder()
           .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
           .RequireAuthenticatedUser()
           .Build();

        public static AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder()
           .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
           .RequireAuthenticatedUser()
           .Build();

        public static AuthorizationPolicy adminPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireRole("admin")
            .Build();

        public static AuthorizationPolicy age18Policy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            //.Requirements.Add(new AgeRequirement(18))
            .Build();

        public static AuthorizationPolicy assertionPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireRole("admin")
            // The Require Assertion method takes a lambda that receives the Http Context object and returns a Boolean value. 
            // Therefore, the assertion is simply a conditional statement.
            .RequireAssertion(ctx => { return ctx.User.HasClaim("editor", "contents") || ctx.User.HasClaim("level", "senior"); })
            .Build();
    }
}
