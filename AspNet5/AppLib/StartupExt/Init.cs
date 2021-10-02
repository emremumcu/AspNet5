namespace AspNet5.AppLib.StartupExt
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;

    public static class Init
    {
        public static IServiceCollection _InitMVC(this IServiceCollection services)
        {
            IMvcBuilder mvcBuilder =

                services
                    .AddControllersWithViews(config =>
                    {
                        /// Anti-forgery tokens are a security mechanism to defend against cross-site request forgery (CSRF) attacks.
                        /// This attribute causes validation of antiforgery tokens for all unsafe HTTP methods. 
                        /// An antiforgery token is required for HTTP methods other than GET, HEAD, OPTIONS, and TRACE.
                        /// Using this global filter is equivalent of applying [ValidateAntiForgeryToken] attribute for all POST action methods
                        config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        config.Filters.Add(new AuthorizeFilter());                        
                        //config.Filters.Add<GlobalExceptionFilter>();
                    })

                    /// The cookie-based TempData provider is enabled by default.
                    /// But the maximum cookie size is less than 4096 bytes due to encryption and chunking. 
                    /// And the cookie data isn't compressed because compressing encrypted data can lead to security problems such as the CRIME and BREACH attacks. 
                    /// Instead of cookie-based TempData provider, use session-based TempData.
                    /// To enable the session-based TempData provider:
                    .AddSessionStateTempDataProvider()
                ;

            /// The TempData provider cookie is not essential by default. 
            /// Make it essential so TempData is functional when tracking is disabled by user (GDPR).
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IWebHostEnvironment environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            /// Without using IServiceCollection
            /// if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            if (!environment.IsProduction())
            {
                /// https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-compilation?view=aspnetcore-3.1&tabs=visual-studio
                /// When enabled, runtime compilation complements build-time compilation, 
                /// allowing Razor files to be updated if they're edited.
                /// Requires: Install-Package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation   
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            /// IHttpContextAccessor isn't always added to the service container by default, 
            /// so register it in ConfigureServices just to be safe
            /// services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();

            mvcBuilder.AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            services.AddRazorPages();

            services.AddSignalR();

            services.AddServerSideBlazor();

            return services;
        }
 
        public static IApplicationBuilder _InitApp(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
