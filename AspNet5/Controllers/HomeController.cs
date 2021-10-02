﻿using AspNet5.AppLib.Extensions;
using AspNet5.AppLib.Filters;
using AspNet5.AppLib.Options;
using AspNet5.AppLib.StartupExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]// prevent swagger to documentate controller
    [ServiceFilter(typeof(WebExceptionFilter))] // services.AddScoped<WebExceptionFilter>();
    public class HomeController : Controller
    {
        private readonly Config _config;

        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private ISession _session => _httpContextAccessor.HttpContext.Session;



        public HomeController(IHttpContextAccessor httpContextAccessor, IOptions<Config> config)
        {
            _config = config.Value;


            // IHttpContextAccessor requires services.AddHttpContextAccessor(); in Startup
            _httpContextAccessor = httpContextAccessor;           

            // This (IHttpContextAccessor) can be used in any class (other than Controllers)
            _httpContextAccessor.HttpContext.Session.SetKey<DateTime>("LOGIN_DATETIME", DateTime.UtcNow); 
            
            // This can ONLY be used in Controllers
            _session.SetKey<DateTime>("LOGIN_DATETIME", DateTime.UtcNow);    
        }


        [Authorize] // Without arguments, [Authorize] attribute only checks that the user is authenticated (default policy => RequireAuthenticatedUser()) 
        public IActionResult Index([FromServices] IMemoryCache memCache)
        {
            /// 
            /// Client IP
            /// 
            //ViewBag.IP1 = HttpContext.GetRemoteIPAddress().ToString();
            //ViewBag.IP2 = _accessor.HttpContext.Connection.RemoteIpAddress.ToString(); // sorunlu
            //ViewBag.IP3 = (Request.Headers.ContainsKey("X-Forwarded-For") ? (Request.Headers["X-Forwarded-For"].ToString()) : (_accessor.HttpContext.Connection.RemoteIpAddress.ToString()));
            ViewBag.ClientIP = HttpContext.GetRemoteIPAddress().ToString();

            /// 
            /// Memory Cache
            /// 
            if (!memCache.TryGetValue<DateTime>("CACHED_DATETIME", out DateTime now))
                memCache.Set<DateTime>("CACHED_DATETIME", DateTime.Now, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(2)));
            ViewBag.CachedDateTime = memCache.Get<DateTime>("CACHED_DATETIME");


            return View();
        }



        public IActionResult UserInfo() => View();

        public IActionResult Bootstrap() => View();

        public IActionResult SignalRDemo() => View();

        /// <summary>
        /// The Roles property indicates that users in any of the listed roles would be granted access. 
        /// To require multiple roles, you can apply the Authorize attribute multiple times, or write your own filter.
        /// Roles are case-sensitive
        /// </summary>        
        [Authorize(Roles = "admin, system")] // OR condition
        public IActionResult Action1() => Content("You have admin or system role");


        
        /// <summary>
        /// The AuthenticationSchemes property is a comma-separated string listing the authentication middleware 
        /// components that the authorization layer will trust in the current context. In other words, it states 
        /// that access to the Backoffice Controller class is allowed only if the user is authenticated through 
        /// the Cookies scheme and has any of the listed roles.
        /// </summary>        
        [Authorize(Roles = "admin, system", AuthenticationSchemes = "Cookies")]
        public IActionResult Action2() => Content("You have admin or system role and you are authenticated using cookie scheme");



        [Authorize(Roles = "admin")]
        [Authorize(Roles = "system")] // AND condition: Users who belong to both admin and system roles have access
        public IActionResult Action3() => Content("You have admin and system role at the same time");

    }
}
