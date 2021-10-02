// HACK Warnings
//#if DEBUG 
//#warning WARNING: Running in DEBUG
//#else
//#warning WARNING: Running in PROD
//#endif

namespace AspNet5
{
    using AspNet5.AppLib.Filters;
    using AspNet5.AppLib.StartupExt;
    using AspNet5.Hubs;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services._AddCORS();

            services._InitMVC();

            services._ConfigureForwardedHeaderOptions();

            services._ConfigureCookiePolicyOptions();

            services._AddSession();

            services._ConfigureViewLocationExpander();

            services._AddAuthentication();
            
            services._AddAuthorization();

            services._MockAuthorize();

            services._AddOptions();            

            services._ConfigureEF();

            services.AddMemoryCache();

            

            services.AddScoped<WebExceptionFilter>();

            services.AddHttpClient();

            // PM> Install-Package Swashbuckle.AspNetCore
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNet5", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            

            // if (env.IsDevelopment() || env.IsStaging()) app._UseDevExceptions(); else app._UseProdExceptions();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNet5 v1"));
            }

            app._InitApp();

            app._UseCookiePolicy();

            app._UseSCP();

            app._UseForwardedHeaderOptions();

            app._CheckService<IWebHostEnvironment>();            

            app.UseStaticFiles();

            app.UseRouting();

            app._UseCORS();

            app._UseSession();            

            app._UseAuthentication();

            app._UseAuthorization();

            app._Mock();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/license", async context => { await context.Response.WriteAsync(System.IO.File.ReadAllText("license.md")); });
                
                endpoints.MapControllerRoute(name: "admin", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
                
                endpoints.MapBlazorHub();

                // 
                // endpoints.MapFallbackToPage("/_Host");

                endpoints.MapHub<MainHub>("/mainhub");
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("No handlers found");
            });
        }
    }
}