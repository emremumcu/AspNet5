using AspNet5.AppLib.Abstract;
using AspNet5.AppLib.Concrete;
using AspNet5.AppLib.Evaluators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AspNet5.AppLib.StartupExt
{
    public static class MockExtension
    {
        /// <summary>
        /// Requires services.AddAuthentication, services.AddAuthorization
        /// </summary>
        public static IServiceCollection _MockAuthorize(this IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IWebHostEnvironment environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            
            if (!environment.IsProduction())
            {
                services.AddSingleton<IPolicyEvaluator, MockPolicyEvaluator>();
            }
            else
            {
                throw new InvalidOperationException("MockAuthorize in Prod is NOT allowed!");
            }

            return services;
        }

        /// <summary>
        /// Requires app.UseAuthentication, app.UseAuthorization
        /// </summary>
        public static IApplicationBuilder _Mock(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
