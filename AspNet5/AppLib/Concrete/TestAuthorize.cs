namespace AspNet5.AppLib.Concrete
{
    using AspNet5.AppLib.Abstract;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    public class TestAuthorize : IAuthorize
    {
        private ClaimsPrincipal GetPrincipal(string userId)
        {
            List<Claim> UserClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, "Test Role")
            };

            return new ClaimsPrincipal(new ClaimsIdentity(UserClaims, CookieAuthenticationDefaults.AuthenticationScheme));
        }

        private AuthenticationProperties GetProperties() => new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
            IsPersistent = true,
            IssuedUtc = DateTimeOffset.UtcNow,
            RedirectUri = "RedirectUri"
        };

        public AuthenticationTicket GetTicket(string userId) => 
            new AuthenticationTicket(GetPrincipal(userId), GetProperties(), CookieAuthenticationDefaults.AuthenticationScheme);
    }
}