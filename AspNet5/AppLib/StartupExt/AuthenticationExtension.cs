namespace AspNet5.AppLib.StartupExt
{
    using AspNet5.AppLib.Abstract;
    using AspNet5.AppLib.Concrete;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public static class AuthenticationExtension
    {
        public static IServiceCollection _AddAuthentication(this IServiceCollection services)
        {
            Action<CookieAuthenticationOptions> configureOptions = (options) => {                
                options.ClaimsIssuer = "AppClaimsIssuer";
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ReturnUrlParameter = "Return_Url";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.Cookie.Name = "AppCookieName";
                options.Cookie.HttpOnly = true; /// false makes xss vulnerability
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.EventsType = typeof(CustomCookieAuthenticationEvents); // registered below
                // options.Events.OnValidatePrincipal = CookieValidator.ValidateAsync;
                    //options.Events.OnRedirectToLogin = (context) =>
                    //{
                    //    // Ajax Request:
                    //    // context.Response.Headers["Location"] = context.RedirectUri;
                    //    // context.Response.StatusCode = 401;
                    //    // context.Response.Redirect(context.RedirectUri);
                    //    return Task.CompletedTask;
                    //};
                    //options.Events.OnRedirectToLogout = (context) =>
                    //{
                    //    return Task.CompletedTask;
                    //};
                    //options.Events.OnRedirectToAccessDenied = (context) =>
                    //{
                    //    return Task.CompletedTask;
                    //};
                    //options.Events.OnSignedIn = (context) =>
                    //{
                    //    return Task.CompletedTask;
                    //};                

                options.Validate();
            };

            services
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;                    
                })
                .AddCookie(configureOptions)
            ;

            services.AddScoped<CustomCookieAuthenticationEvents>();


            // TODO : Set Authenticator
            services.AddSingleton<IAuthenticate, TestAuthenticate>();

            return services;
        }

        /// <summary>
        /// Authentication and Authorization must be in between Routing and Endpoints.
        /// </summary>
        public static IApplicationBuilder _UseAuthentication(this IApplicationBuilder app)
        {
            //app.UseCookiePolicy(new CookiePolicyOptions()
            //{
            //    MinimumSameSitePolicy = SameSiteMode.Lax
            //});
                        
            app.UseAuthentication();

            return app;
        }
    }

    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        /// <summary>
        /// 
        /// ValidatePrincipal method runs in every request.
        /// 
        /// Once an authentication cookie is created, it becomes the single source of identity. 
        /// If a user account is invalidated and not valid anymore in backend, the app's cookie 
        /// authentication system continues to process requests based on the authentication cookie.
        /// 
        /// The user remains signed into the app as long as the authentication cookie is valid.
        /// The ValidatePrincipal event can be used to intercept and override validation of the 
        /// cookie identity. Validating the cookie on every request mitigates the risk of revoked 
        /// users accessing the app.         
        /// 
        /// </summary>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            try
            {
                if (context.HttpContext.Session.IsAvailable)
                {
                    ClaimsPrincipal user = context.Principal;

                    bool login = context.HttpContext.Session.GetKey<bool>("login");

                    /// Other checks here !!!

                    if (!(user.Identity.IsAuthenticated && login))
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                }
                else
                {
                    context.RejectPrincipal();
                }
            }
            catch
            {
                /// Session has not been configured yet!
            }
        }
    }

    public static class CookieValidator
    {
        public static async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            if (!await ValidateCookieAsync(context))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        private static async Task<bool> ValidateCookieAsync(CookieValidatePrincipalContext context)
        {
            var claimsPrincipal = context.Principal;

            var uid = (from c in claimsPrincipal.Claims where c.Type == ClaimTypes.NameIdentifier select c.Value).FirstOrDefault();

            var rowVersionString = (from c in claimsPrincipal.Claims where c.Type == ClaimTypes.UserData select c.Value).FirstOrDefault();

            byte[] rowVersion = Encoding.Unicode.GetBytes(rowVersionString);

            return await Task.FromResult<bool>(true);
        }
    }

}


//Action act = () =>
//{
//    Console.WriteLine("No Parameter");
//};

//Action<int> actWithOneParameter = (arg1) =>
//{
//    Console.WriteLine("Par: " + arg1);
//};

//Action<int, int> actWithTwoParameter = (arg1, arg2) =>
//{
//    Console.WriteLine("Par1: " + arg1 + ", Par2: " + arg2);
//};

//////static void Main(string[] args)
//////{
//////    Action<string> action = new Action<string>(Display);
//////    action("Hello!!!");
//////    Console.Read();
//////}
//////static void Display(string message)
//////{
//////    Console.WriteLine(message);
//////}


//////static void Main(string[] args)
//////{
//////    Func<int, double> func = new Func<int, double>(CalculateHra);
//////    Console.WriteLine(func(50000));
//////    Console.Read();
//////}
//////static double CalculateHra(int basic)
//////{
//////    return (double)(basic * .4);
//////}
///

//////static void Main(string[] args)
//////{
//////    List<Customer> custList = new List<Customer>();
//////    custList.Add(new Customer { Id = 1, FirstName = "Joydip", LastName = "Kanjilal", State = "Telengana", City = "Hyderabad", Address = "Begumpet", Country = "India" });
//////    custList.Add(new Customer { Id = 2, FirstName = "Steve", LastName = "Jones", State = "OA", City = "New York", Address = "Lake Avenue", Country = "US" });
//////    Predicate<Customer> hydCustomers = x => x.Id == 1;
//////    Customer customer = custList.Find(hydCustomers);
//////    Console.WriteLine(customer.FirstName);
//////    Console.Read();
//////}

//.AddCookie(options =>
//{                    
//    //options.LoginPath = "/Account/Login";
//    //options.LogoutPath = "/Account/Logout";
//    //options.AccessDeniedPath = "/Account/AccessDenied";
//    //options.ReturnUrlParameter = "ReturnUrl";
//    //options.Cookie.Name = ".AuthCookie";
//    //options.ClaimsIssuer = ".AppIssuer";                    
//    //options.SlidingExpiration = true;
//    //options.Cookie.HttpOnly = true; /// false makes xss vulnerability
//    // options.ExpireTimeSpan = TimeSpan.FromSeconds(1200);
//    //options.Cookie.SameSite = SameSiteMode.Lax;
//    //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
//    options.Validate();
//    options.EventsType = typeof(CustomCookieAuthenticationEvents); // Registered below

//    //options.Events.OnRedirectToLogin = (context) =>
//    //{
//    //    // Ajax Request:
//    //    // context.Response.Headers["Location"] = context.RedirectUri;
//    //    // context.Response.StatusCode = 401;
//    //    // context.Response.Redirect(context.RedirectUri);
//    //    return Task.CompletedTask;
//    //};
//    //options.Events.OnRedirectToLogout = (context) =>
//    //{
//    //    return Task.CompletedTask;
//    //};
//    //options.Events.OnRedirectToAccessDenied = (context) =>
//    //{
//    //    return Task.CompletedTask;
//    //};
//    //options.Events.OnSignedIn = (context) =>
//    //{
//    //    return Task.CompletedTask;
//    //};
//    Events = new CookieAuthenticationEvents()
//    {
//    // in custom function set the session expiration
//    // via the DB and reset it everytime this is called
//    // if the session is still active
//    // otherwise, you can redirect if it's invalid
//    OnValidatePrincipal = < custom function here >
//    }
//})