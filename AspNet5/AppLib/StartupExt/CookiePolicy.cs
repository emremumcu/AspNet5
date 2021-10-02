/// https://docs.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-3.1

namespace AspNet5.AppLib.StartupExt
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.CookiePolicy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;    

    /// <summary>
    /// This is the global cookie policy. Requires: app.UseCookiePolicy() in Configure section.
    /// </summary>
    public static class CookiePolicy
    {
        public static IServiceCollection _ConfigureCookiePolicyOptions(this IServiceCollection services)
        {
            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    // The CheckConsentNeeded option of true will prevent any non-essential cookies 
                    // from being sent to the browser (no Set-Cookie header) without the user's explicit permission.
                    // You can either change this behaviour, or mark your cookie as essential by setting the 
                    // IsEssential property to true when creating it: options.Cookie.IsEssential = true;
                    options.CheckConsentNeeded = context => true; // Enable the default cookie consent feature 
                    // requires using Microsoft.AspNetCore.Http;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                    options.HttpOnly = HttpOnlyPolicy.Always;
                    options.Secure = CookieSecurePolicy.SameAsRequest;
                });

            return services;
        }

        /// <summary>
        /// Before app.UseRouting();
        /// </summary>
        public static IApplicationBuilder _UseCookiePolicy(this IApplicationBuilder app)
        {
            app.UseCookiePolicy();
            return app;
        }
    }
}
