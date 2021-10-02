# Startup.cs

## ConfigureServices Method

ASP.net core has built-in support for Dependency Injection. ConfigureServices method is used to configure Dependency Injection (add / register services to DI container).

When the application is requested for the first time, it calls ConfigureServices method.  At run time, the ConfigureServices method is called before the Configure method. The order of the registered services in ConfigureServices is generally NOT important.

There are three possible types of service registration in terms of the life-cycle:
 
* Singleton: IoC container will create and share a single instance of a service throughout the application's lifetime.
* Transient: The IoC container will create a new instance of the specified service type every time you ask for it.
* Scoped: IoC container will create an instance of the specified service type once per request and will be shared in a single request.

**Examples:**
``` csharp
// Singleton
services.AddSingleton<ILog, MyConsoleLogger>();

// Singleton (default)
services.Add(new ServiceDescriptor(typeof(ILog), new MyConsoleLogger()));

// Transient
services.Add(new ServiceDescriptor(typeof(ILog), typeof(MyConsoleLogger), ServiceLifetime.Transient));
``` 

## Configure Method

This method is used to define how the application will respond on each HTTP request. This method is also used to configure the middlewares in HTTP pipeline.

Use Configure method to set up middlewares, routing rules etc.

Order of the middlewares IS important to build the pipeline.

``` csharp
  using System.ServiceProcess;

// get list of Windows services
ServiceController[] services = ServiceController.GetServices();

// try to find service name
foreach (ServiceController service in services)
{
    if (service.ServiceName == "???")
        return true;
}
```
  
**The following Startup.Configure method adds security-related middleware components in the recommended order:**

``` csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
   if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); app.UseDatabaseErrorPage(); }
   else { app.UseExceptionHandler("/Error"); app.UseHsts(); }
   app.UseHttpsRedirection();
   app.UseStaticFiles();
   app.UseCookiePolicy();
   app.UseRouting();
   app.UseRequestLocalization();
   app.UseCors();
   app.UseAuthentication();
   app.UseAuthorization();
   app.UseSession();
   app.UseResponseCaching();
   app.UseEndpoints(endpoints =>
   {
       endpoints.MapRazorPages();
       endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
   });
}
```



## RequireAuthorization in Routing

**Authotization in Routing:**

The big problem with the AuthorizeFilter approach is that it's an MVC-only feature. 
ASP.NET Core 3.0+ provides a different mechanism for setting authorization on endpoints the RequireAuthorization() extension method on IEndpointConventionBuilder. 
Instead of configuring a global AuthorizeFilter, call RequireAuthorization() when configuring the endpoints of your application, in Configure():

``` csharp
app.UseEndpoints(endpoints => {
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
    endpoints.MapHealthChecks("/health").RequireAuthorization();
    endpoints.MapRazorPages().RequireAuthorization("MyCustomPolicy");
    endpoints.MapHealthChecks("/healthz").RequireAuthorization("OtherPolicy", "MainPolicy");
});  
```

# IServiceProvider
There are three ways to get an instance of IServiceProvider:

``` csharp
// Using IApplicationBuilder
public void Configure(IServiceProvider pro, IApplicationBuilder app, IHostingEnvironment env)
{
    var services = app.ApplicationServices;
    var logger = services.GetService<ILog>();
}

// Using HttpContext
var services = HttpContext.RequestServices;
var log = (ILog)services.GetService(typeof(ILog));

// Using IServiceCollection
public void ConfigureServices(IServiceCollection services)
{
    var serviceProvider = services.BuildServiceProvider();
}
```

# ConfigurationBuilder

``` csharp
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddJsonFile("config.json", optional: true, reloadOnChange: true)
        .AddJsonFile("data.json", optional: true, reloadOnChange: true)
    ;

    IConfiguration cfg = builder.Build();
        // or
    IConfigurationRoot cfgRoot = builder.Build();

    // Getting values from configuration
  
    // { "Version": "1.0.0" }
    var version = configuration["Version"];
  
    // { "Auth": { "Users": ["user1", "user2"] } }
    var users = configuration.GetSection("Auth:Users")[.GetChildren()]
``` 




  
 