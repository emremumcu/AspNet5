using AspNet5.AppLib.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet5.AppLib.StartupExt
{
    public static class JWTAuthExtension
    {
        public static IServiceCollection _(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.LoginPath = "/JWT/Login/";
                options.AccessDeniedPath = "/JWT/Forbidden/";
            })
            .AddJwtBearer(x =>
            {
                x.Audience = "*";
                x.ClaimsIssuer = "https://mumcu.net";
                x.RequireHttpsMetadata = false; // must be set as true in prod env
                x.SaveToken = true;
                x.TokenValidationParameters = new JWTFactory().GetTokenValidationParameters();
                x.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = (context) =>
                    {
                        if (context.Request.Headers.ContainsKey("Authorization"))
                        {
                            Microsoft.Extensions.Primitives.StringValues accessTokens = context.Request.Headers["Authorization"];

                                        // we have token control it :

                                        List<string> tkns = accessTokens.ToList(); // schema token

                                        bool tokenvalid = new JWTFactory().ValidateToken(tkns[0].Split(' ')[1], out string message);

                            if (tokenvalid)
                                return Task.CompletedTask;
                            else
                                context.Fail("Invalid token");
                        }

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = (context) =>
                    {
                        IdentityModelEventSource.ShowPII = true;

                                    //context.Principal.Identity is ClaimsIdentity	                
                                    //So casting it to ClaimsIdentity provides all generated claims	                
                                    //And for an extra token validation they might be usefull	
                                    ClaimsPrincipal user = context.Principal;
                        ClaimsIdentity claims = context.Principal.Identity as ClaimsIdentity;

                        if (!claims.IsAuthenticated)
                        {
                            context.Fail("Unauthorized");
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = ctx =>
                    {
                        IdentityModelEventSource.ShowPII = true;

                        Console.WriteLine("Exception: {0}", ctx.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });


            return services;
        }

        public static IApplicationBuilder _(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
