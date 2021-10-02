using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNet5.AppLib.StartupExt
{
    public static class ForwardedHeaderOptions
    {
        /// <summary>
        /// Requires: app._UseForwardedHeaderOptions() in Configure section
        /// </summary>
        public static IServiceCollection _ConfigureForwardedHeaderOptions(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                // options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.RequireHeaderSymmetry = false;
                options.ForwardLimit = null;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            return services;
        }

        /// <summary>
        /// Use at the top after exception settings
        /// </summary>
        public static IApplicationBuilder _UseForwardedHeaderOptions(this IApplicationBuilder app)
        {
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.All,
            //    RequireHeaderSymmetry = false,
            //    ForwardLimit = null,
            //    // KnownProxies = 
            //    KnownNetworks = { new IPNetwork(IPAddress.Parse("::ffff:172.17.0.1"), 104) }
            //});

            app.UseForwardedHeaders();

            return app;
        }
    }
}
