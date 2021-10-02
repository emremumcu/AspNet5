# Authentication & Authorization

Authentication is the process of determining a user's identity. Authorization is the process of determining whether a user has access to a resource. In ASP.NET Core, authentication is handled by the IAuthenticationService, which is used by authentication middleware. The authentication service uses registered authentication handlers to complete authentication-related actions. The registered authentication handlers and their configuration options are called "schemes". Authentication schemes are specified by registering authentication services in Startup.ConfigureServices:

 * By calling a scheme-specific extension method after a call to services.AddAuthentication (such as AddJwtBearer or AddCookie, for example). These extension methods use AuthenticationBuilder.AddScheme to register schemes with appropriate settings.
 * Less commonly, by calling AuthenticationBuilder.AddScheme directly.

For example, the following code registers authentication services and handlers for cookie and JWT bearer authentication schemes:

``` cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => Configuration.Bind("JwtSettings", options))
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => Configuration.Bind("CookieSettings", options));
```

In some cases, the call to AddAuthentication is automatically made by other extension methods. For example, when using ASP.NET Core Identity, AddAuthentication is called internally.

## Authentication concepts

Authentication is responsible for providing the ClaimsPrincipal for authorization to make permission decisions against. There are multiple authentication scheme approaches to select which authentication handler is responsible for generating the correct set of claims:

 * Authentication scheme
 * The default authentication scheme, discussed in the next section.
 * Directly set HttpContext.User.

There is no automatic probing of schemes. If the default scheme is not specified, the scheme must be specified in the authorize attribute, otherwise, the following error is thrown:

 > InvalidOperationException: No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(`Action<AuthenticationOptions>` configureOptions).

An authentication scheme is a name which corresponds to:

 * An authentication handler.
 * Options for configuring that specific instance of the handler.

Schemes are useful as a mechanism for referring to the authentication, challenge, and forbid behaviors of the associated handler.

An authentication handler:

 * Is a type that implements the behavior of a scheme.
 * Is derived from IAuthenticationHandler or AuthenticationHandler<TOptions>.
 * Has the primary responsibility to authenticate users.

Based on the authentication scheme's configuration and the incoming request context, authentication handlers:

 * Construct AuthenticationTicket objects representing the user's identity if authentication is successful.
 * Return 'no result' or 'failure' if authentication is unsuccessful.
 * Have methods for challenge and forbid actions for when users attempt to access resources:
   * They are unauthorized to access (forbid).
   * When they are unauthenticated (challenge).

### Authenticate

An authentication scheme's authenticate action is responsible for constructing the user's identity based on request context. It returns an AuthenticateResult indicating whether authentication was successful and, if so, the user's identity in an authentication ticket. Authenticate examples include:

 * A cookie authentication scheme constructing the user's identity from cookies.
 * A JWT bearer scheme deserializing and validating a JWT bearer token to construct the user's identity.

## Challenge
An authentication challenge is invoked by Authorization when an unauthenticated user requests an endpoint that requires authentication. An authentication challenge is issued, for example, when an anonymous user requests a restricted resource or clicks on a login link. Authorization invokes a challenge using the specified authentication scheme(s), or the default if none is specified. Authentication challenge examples include:

 * A cookie authentication scheme redirecting the user to a login page.
 * A JWT bearer scheme returning a 401 result with a www-authenticate: bearer header.

A challenge action should let the user know what authentication mechanism to use to access the requested resource.

### Forbid

An authentication scheme's forbid action is called by Authorization when an authenticated user attempts to access a resource they are not permitted to access. See ForbidAsync. Authentication forbid examples include:

 * A cookie authentication scheme redirecting the user to a page indicating access was forbidden.
 * A JWT bearer scheme returning a 403 result.
 * A custom authentication scheme redirecting to a page where the user can request access to the resource.

A forbid action can let the user know:

 * They are authenticated.
 * They aren't permitted to access the requested resource.

# Role-based authorization in ASP.NET Core

Role-based authorization checks are declarative—the developer embeds them within their code, against a controller or an action within a controller, specifying roles which the current user must be a member of to access the requested resource.


``` csharp
[Authorize(Roles = "Administrator, Admin")]
public class AdministrationController : Controller
{
}
```
This controller would be only accessible by users who are members of the Administrator role or the Admin role.

If you apply multiple attributes then an accessing user must be a member of all the roles specified; the following sample requires that a user must be a member of both the PowerUser and ControlPanelUser role.

``` csharp
[Authorize(Roles = "PowerUser")]
[Authorize(Roles = "ControlPanelUser")]
public class ControlPanelController : Controller
{
}
```

You can further limit access by applying additional role authorization attributes at the action level:

``` csharp
[Authorize(Roles = "Administrator, PowerUser")]
public class ControlPanelController : Controller
{
    public ActionResult SetTime() { }

    [Authorize(Roles = "Administrator")]
    public ActionResult ShutDown() { }
}
```

In the previous code snippet members of the Administrator role or the PowerUser role can access the controller and the SetTime action, but only members of the Administrator role can access the ShutDown action.

You can also lock down a controller but allow anonymous, unauthenticated access to individual actions.

``` csharp
[Authorize]
public class ControlPanelController : Controller
{
    public ActionResult SetTime() { }

    [AllowAnonymous]
    public ActionResult Login() { }
}
```

For Razor Pages, the AuthorizeAttribute can be applied by either using a convention, or
applying the AuthorizeAttribute to the PageModel instance.

``` csharp
[Authorize(Policy = "RequireAdministratorRole")]
public class UpdateModel : PageModel
{
    public ActionResult OnPost()
    {
    }
}
```

*Filter attributes, including AuthorizeAttribute, can only be applied to PageModel and cannot be applied to specific page handler methods.*

## Policy based role checks

Role requirements can also be expressed using the new Policy syntax, where a developer registers a policy at startup as part of the Authorization service configuration. This normally occurs in ConfigureServices() in your Startup.cs file.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();

    services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdministratorRole",
             policy => policy.RequireRole("Administrator"));
    });
}
```

Policies are applied using the Policy property on the AuthorizeAttribute attribute:

``` csharp
[Authorize(Policy = "RequireAdministratorRole")]
public IActionResult Shutdown()
{
    return View();
}
```

If you want to specify multiple allowed roles in a requirement then you can specify them as parameters to the RequireRole method:

``` csharp
options.AddPolicy("ElevatedRights", policy =>
                  policy.RequireRole("Administrator", "PowerUser", "BackupAdministrator"));
```

This example authorizes users who belong to the Administrator, PowerUser or BackupAdministrator roles.

# Claims-based authorization in ASP.NET Core
A claim is a name value pair that represents what the subject is, not what the subject can do. An identity can contain multiple claims with multiple values and can contain multiple claims of the same type.

Claim based authorization checks are declarative - the developer embeds them within their code, against a controller or an action within a controller, specifying claims which the current user must possess, and optionally the value the claim must hold to access the requested resource. Claims requirements are policy based, the developer must build and register a policy expressing the claims requirements.

The simplest type of claim policy looks for the presence of a claim and doesn't check the value.

Build and register the policy. This takes place as part of the Authorization service configuration, which normally takes part in ConfigureServices() in your Startup.cs file.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();

    services.AddAuthorization(options =>
    {
        options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
    });
}
```

Call UseAuthorization in Configure. 

In this case the EmployeeOnly policy checks for the presence of an EmployeeNumber claim on the current identity.

``` csharp
[Authorize(Policy = "EmployeeOnly")]
public class VacationController : Controller
{
    public ActionResult VacationBalance()
    {
    }
}

[Authorize(Policy = "EmployeeOnly")]
public IActionResult VacationBalance()
{
    return View();
}
```

The AuthorizeAttribute attribute can be applied to an entire controller, in this instance only identities matching the policy will be allowed access to any Action on the controller.

Most claims come with a value. You can specify a list of allowed values when creating the policy. The following example would only succeed for employees whose employee number was 1, 2, 3, 4 or 5.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();

    services.AddAuthorization(options =>
    {
        options.AddPolicy("Founders", policy =>
                          policy.RequireClaim("EmployeeNumber", "1", "2", "3", "4", "5"));
    });
}
```

If you apply multiple policies to a controller or action, then all policies must pass before access is granted.

``` csharp
[Authorize(Policy = "EmployeeOnly")]
public class SalaryController : Controller
{
    public ActionResult Payslip()
    {
    }

    [Authorize(Policy = "HumanResources")]
    public ActionResult UpdateSalary()
    {
    }
}
```

In the above example any identity which fulfills the EmployeeOnly policy can access the Payslip action as that policy is enforced on the controller. However in order to call the UpdateSalary action the identity must fulfill both the EmployeeOnly policy and the HumanResources policy.

# Policy-based authorization in ASP.NET Core

Underneath the covers, role-based authorization and claims-based authorization use a requirement, a requirement handler, and a pre-configured policy. These building blocks support the expression of authorization evaluations in code. The result is a richer, reusable, testable authorization structure.

An authorization policy consists of one or more requirements. It's registered as part of the authorization service configuration, in the Startup.ConfigureServices method:

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();
    services.AddAuthorization(options =>
    {
        options.AddPolicy("AtLeast21", policy =>
            policy.Requirements.Add(new MinimumAgeRequirement(21)));
    });
}
```

In the preceding example, an "AtLeast21" policy is created. It has a single requirement—that of a minimum age, which is supplied as a parameter to the requirement.

``` csharp
[Authorize(Policy = "AtLeast21")]
public class AlcoholPurchaseController : Controller
{
    public IActionResult Index() => View();
}

// Policies can not be applied at the Razor Page handler level, they must be applied to the Page.
[Authorize(Policy = "AtLeast21")]
public class AlcoholPurchaseModel : PageModel
{
}
```

## Requirements

An authorization requirement is a collection of data parameters that a policy can use to evaluate the current user principal. In our "AtLeast21" policy, the requirement is a single parameter—the minimum age. A requirement implements IAuthorizationRequirement, which is an empty marker interface. A parameterized minimum age requirement could be implemented as follows:

``` csharp
using Microsoft.AspNetCore.Authorization;

public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}
```

If an authorization policy contains multiple authorization requirements, all requirements must pass in order for the policy evaluation to succeed. In other words, multiple authorization requirements added to a single authorization policy are treated on an AND basis.

A requirement doesn't need to have data or properties.

## Authorization handlers

An authorization handler is responsible for the evaluation of a requirement's properties. The authorization handler evaluates the requirements against a provided AuthorizationHandlerContext to determine if access is allowed.

A requirement can have multiple handlers. A handler may inherit `AuthorizationHandler<TRequirement>`, where TRequirement is the requirement to be handled. Alternatively, a handler may implement IAuthorizationHandler to handle more than one type of requirement.

The following is an example of a one-to-one relationship in which a minimum age handler utilizes a single requirement:

``` csharp
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PoliciesAuthApp1.Services.Requirements;

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   MinimumAgeRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth &&
                                        c.Issuer == "http://contoso.com"))
        {
            //TODO: Use the following if targeting a version of
            //.NET Framework older than 4.6:
            //      return Task.FromResult(0);
            return Task.CompletedTask;
        }

        var dateOfBirth = Convert.ToDateTime(
            context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth && 
                                        c.Issuer == "http://contoso.com").Value);

        int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
        {
            calculatedAge--;
        }

        if (calculatedAge >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
        }

        //TODO: Use the following if targeting a version of
        //.NET Framework older than 4.6:
        //      return Task.FromResult(0);
        return Task.CompletedTask;
    }
}
```

The following is an example of a one-to-many relationship in which a permission handler can handle three different types of requirements:

``` csharp
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PoliciesAuthApp1.Services.Requirements;

public class PermissionHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is ReadPermission)
            {
                if (IsOwner(context.User, context.Resource) ||
                    IsSponsor(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement is EditPermission ||
                     requirement is DeletePermission)
            {
                if (IsOwner(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }
        }

        //TODO: Use the following if targeting a version of
        //.NET Framework older than 4.6:
        //      return Task.FromResult(0);
        return Task.CompletedTask;
    }

    private bool IsOwner(ClaimsPrincipal user, object resource)
    {
        // Code omitted for brevity

        return true;
    }

    private bool IsSponsor(ClaimsPrincipal user, object resource)
    {
        // Code omitted for brevity

        return true;
    }
}
```

Handlers are registered in the services collection during configuration. For example:

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();
    services.AddAuthorization(options =>
    {
        options.AddPolicy("AtLeast21", policy =>
            policy.Requirements.Add(new MinimumAgeRequirement(21)));
    });

    services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
}
```

### What should a handler return

* A handler indicates success by calling context.Succeed(IAuthorizationRequirement requirement), passing the requirement that has been successfully validated.

* A handler doesn't need to handle failures generally, as other handlers for the same requirement may succeed.

* To guarantee failure, even if other requirement handlers succeed, call context.Fail.

If a handler calls context.Succeed or context.Fail, all other handlers are still called. This allows requirements to produce side effects, such as logging, which takes place even if another handler has successfully validated or failed a requirement. When set to false, the InvokeHandlersAfterFailure property short-circuits the execution of handlers when context.Fail is called. InvokeHandlersAfterFailure defaults to true, in which case all handlers are called.

Authorization handlers are called even if authentication fails.

### Use a func to fulfill a policy

There may be situations in which fulfilling a policy is simple to express in code. It's possible to supply a Func<AuthorizationHandlerContext, bool> when configuring your policy with the RequireAssertion policy builder.

For example, the previous BadgeEntryHandler could be rewritten as follows:

``` csharp
services.AddAuthorization(options =>
{
     options.AddPolicy("BadgeEntry", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "BadgeId" ||
                 c.Type == "TemporaryBadgeId") &&
                 c.Issuer == "https://microsoftsecurity")));
});
```

### Access MVC request context in handlers

The HandleRequirementAsync method you implement in an authorization handler has two parameters: an AuthorizationHandlerContext and the TRequirement you are handling. Frameworks such as MVC or SignalR are free to add any object to the Resource property on the AuthorizationHandlerContext to pass extra information.

When using endpoint routing, authorization is typically handled by the Authorization Middleware. In this case, the Resource property is an instance of HttpContext. The context can be used to access the current endpoint, which can be used to probe the underlying resource to which you're routing. For example:

``` csharp
if (context.Resource is HttpContext httpContext)
{
    var endpoint = httpContext.GetEndpoint();
    var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
    // ...
}
```

With traditional routing, or when authorization happens as part of MVC's authorization filter, the value of Resource is an AuthorizationFilterContext instance. This property provides access to HttpContext, RouteData, and everything else provided by MVC and Razor Pages.

The use of the Resource property is framework specific. Using information in the Resource property limits your authorization policies to particular frameworks. You should cast the Resource property using the is keyword, and then confirm the cast has succeeded to ensure your code doesn't crash with an InvalidCastException when run on other frameworks:

``` csharp
// Requires the following import:
//     using Microsoft.AspNetCore.Mvc.Filters;
if (context.Resource is AuthorizationFilterContext mvcContext)
{
    // Examine MVC-specific things like routing data.
}
```

## Authorize with a specific scheme

In some scenarios, such as Single Page Applications (SPAs), it's common to use multiple authentication methods. For example, the app may use cookie-based authentication to log in and JWT bearer authentication for JavaScript requests.

An authentication scheme is named when the authentication service is configured during authentication. For example:

``` csharp
services.AddAuthentication()
    .AddCookie(options => {
        options.LoginPath = "/Account/Unauthorized/";
        options.AccessDeniedPath = "/Account/Forbidden/";
    })
    .AddJwtBearer(options => {
        options.Audience = "http://localhost:5001/";
        options.Authority = "http://localhost:5000/";
    });
```

Specifying the default scheme results in the HttpContext.User property being set to that identity. If that behavior isn't desired, disable it by invoking the parameterless form of AddAuthentication.

At the point of authorization, the app indicates the handler to be used. Select the handler with which the app will authorize by passing a comma-delimited list of authentication schemes to [Authorize]. The [Authorize] attribute specifies the authentication scheme or schemes to use regardless of whether a default is configured. For example:

``` csharp
[Authorize(AuthenticationSchemes = AuthSchemes)]
public class MixedController : Controller
    // Requires the following imports:
    // using Microsoft.AspNetCore.Authentication.Cookies;
    // using Microsoft.AspNetCore.Authentication.JwtBearer;
    private const string AuthSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme;
```

In the preceding example, both the cookie and bearer handlers run and have a chance to create and append an identity for the current user. By specifying a single scheme only, the corresponding handler runs.

``` csharp
[Authorize(AuthenticationSchemes = 
    JwtBearerDefaults.AuthenticationScheme)]
public class MixedController : Controller
```

In the preceding code, only the handler with the "Bearer" scheme runs. Any cookie-based identities are ignored.

If you prefer to specify the desired schemes in policy, you can set the AuthenticationSchemes collection when adding your policy:

``` csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("Over18", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new MinimumAgeRequirement());
    });
});
```

In the preceding example, the "Over18" policy only runs against the identity created by the "Bearer" handler. Use the policy by setting the [Authorize] attribute's Policy property:

``` csharp
[Authorize(Policy = "Over18")]
public class RegistrationController : Controller
```

Some apps may need to support multiple types of authentication. For example, your app might authenticate users from Azure Active Directory and from a users database. Another example is an app that authenticates users from both Active Directory Federation Services and Azure Active Directory B2C. In this case, the app should accept a JWT bearer token from several issuers.

Add all authentication schemes you'd like to accept. For example, the following code in Startup.ConfigureServices adds two JWT bearer authentication schemes with different issuers:

``` csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Audience = "https://localhost:5000/";
        options.Authority = "https://localhost:5000/identity/";
    })
    .AddJwtBearer("AzureAD", options =>
    {
        options.Audience = "https://localhost:5000/";
        options.Authority = "https://login.microsoftonline.com/eb971100-6f99-4bdc-8611-1bc8edd7f436/";
    });
```

Only one JWT bearer authentication is registered with the default authentication scheme JwtBearerDefaults.AuthenticationScheme. Additional authentication has to be registered with a unique authentication scheme.

The next step is to update the default authorization policy to accept both authentication schemes. For example:

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    // Code omitted for brevity

    services.AddAuthorization(options =>
    {
        var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
            JwtBearerDefaults.AuthenticationScheme,
            "AzureAD");
        defaultAuthorizationPolicyBuilder = 
            defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
        options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
    });
}
```

As the default authorization policy is overridden, it's possible to use the [Authorize] attribute in controllers. The controller then accepts requests with JWT issued by the first or second issuer.

# Examples

## Handler (with a requirement Requirement) Sample

``` csharp
/// 
/// https://stackoverflow.com/a/37578388
/// We don't want you writing custom authorize attributes. 
/// If you need to do that we've done something wrong. 
/// Instead, you should be writing authorization requirements.
/// 
/// Authorization acts upon Identities. Identities are created by authentication.
/// If you wanted to use the Authorize attribute you'd write an authentication middleware and turn it into an authenticated ClaimsPrincipal. 
/// You would then check that inside an authorization requirement. Authorization requirements can be as complicated as you like.
/// 

public class Over18Requirement : AuthorizationHandler<Over18Requirement>, IAuthorizationRequirement
{
    public override void Handle(AuthorizationHandlerContext context, Over18Requirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
        {
            context.Fail();
            return;
        }

        var dateOfBirth = Convert.ToDateTime(context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth).Value);
        int age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-age))
        {
            age--;
        }

        if (age >= 18)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}

/// Then in your ConfigureServices() function you'd wire it up
services.AddAuthorization(options => {
    options.AddPolicy("Over18",
        policy => policy.Requirements.Add(new Authorization.Over18Requirement()));
});

/// And finally, apply it to a controller or action method with
[Authorize(Policy = "Over18")]
```

## Custom Authorize Attribute Sample

``` csharp

/*
Note that if your OnAuthorization implementation needs to await an async method, you should implement IAsyncAuthorizationFilter instead of IAuthorizationFilter otherwise your filter will execute synchronously and your controller action will execute regardless of the outcome of the filter.
*/

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _someFilterParameter;

    public CustomAuthorizeAttribute(string someFilterParameter)
    {
        _someFilterParameter = someFilterParameter;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            // it isn't needed to set unauthorized result 
            // as the base class already requires the user to be authenticated
            // this also makes redirect to a login page work properly
            // context.Result = new UnauthorizedResult();
            return;
        }

        // you can also use registered services
        var someService = context.HttpContext.RequestServices.GetService<ISomeService>();

        var isAuthorized = someService.IsUserAuthorized(user.Identity.Name, _someFilterParameter);
        if (!isAuthorized)
        {
            context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            return;
        }
    }
}
```

## Custom Authorization Handler

``` csharp
/*
You can create your own AuthorizationHandler that will find custom attributes on your Controllers and Actions, and pass them to the HandleRequirementAsync method.
*/

public abstract class AttributeAuthorizationHandler<TRequirement, TAttribute> : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement where TAttribute : Attribute
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
    {
        var attributes = new List<TAttribute>();

        var action = (context.Resource as AuthorizationFilterContext)?.ActionDescriptor as ControllerActionDescriptor;
        if (action != null)
        {
            attributes.AddRange(GetAttributes(action.ControllerTypeInfo.UnderlyingSystemType));
            attributes.AddRange(GetAttributes(action.MethodInfo));
        }

        return HandleRequirementAsync(context, requirement, attributes);
    }

    protected abstract Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, IEnumerable<TAttribute> attributes);

    private static IEnumerable<TAttribute> GetAttributes(MemberInfo memberInfo)
    {
        return memberInfo.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
    }
}

/*
Then you can use it for any custom attributes you need on your controllers or actions. For example to add permission requirements. Just create your custom attribute.
*/

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermissionAttribute : AuthorizeAttribute
{
    public string Name { get; }

    public PermissionAttribute(string name) : base("Permission")
    {
        Name = name;
    }
}

// Then create a Requirement to add to your Policy

public class PermissionAuthorizationRequirement : IAuthorizationRequirement
{
    Add any custom requirement properties if you have them
}

/*
Then create the AuthorizationHandler for your custom attribute, inheriting the AttributeAuthorizationHandler that we created earlier. It will be passed an IEnumerable for all your custom attributes in the HandleRequirementsAsync method, accumulated from your Controller and Action.
*/

public class PermissionAuthorizationHandler : AttributeAuthorizationHandler<PermissionAuthorizationRequirement, PermissionAttribute>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement, IEnumerable<PermissionAttribute> attributes)
    {
        foreach (var permissionAttribute in attributes)
        {
            if (!await AuthorizeAsync(context.User, permissionAttribute.Name))
            {
                return;
            }
        }

        context.Succeed(requirement);
    }

    private Task<bool> AuthorizeAsync(ClaimsPrincipal user, string permission)
    {
        Implement your custom user permission logic here
    }
}

/*
And finally, in your Startup.cs ConfigureServices method, add your custom AuthorizationHandler to the services, and add your Policy.
*/

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

services.AddAuthorization(options =>
{
    options.AddPolicy("Permission", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new PermissionAuthorizationRequirement());
    });
});

/*
Now you can simply decorate your Controllers and Actions with your custom attribute.
*/

[Permission("AccessCustomers")]
public class CustomersController
{
    [Permission("AddCustomer")]
    IActionResult AddCustomer([FromBody] Customer customer)
    {
        Add customer
    }
}
```

# References
 * https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-5.0
 * https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-5.0
 * https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0
 * https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-5.0
 * https://docs.microsoft.com/en-us/aspnet/core/security/authorization/claims?view=aspnetcore-5.0
 * https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0
 * https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-5.0
 * https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-core-web-api-with-json-web-tokens/
 * https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core