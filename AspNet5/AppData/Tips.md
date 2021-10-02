# Tips


* [AllowAnonymous] bypasses all authorization statements. If you combine [AllowAnonymous] and any [Authorize] attribute, the [Authorize] attributes are ignored. For example if you apply [AllowAnonymous] at the controller level, any [Authorize] attributes on the same controller (or on any action within it) is ignored.
* The AuthorizeAttribute can not be applied to Razor Page handlers. For example, [Authorize] can't be applied to OnGet, OnPost, or any other page handler. Consider using an ASP.NET Core MVC controller for pages with different authorization requirements for different handlers. Using an MVC controller when different authorization requirements are required. If you decide not to use an MVC controller, and use Razor pages, use one of the following two approaches to apply authorization to Razor Page handler methods seperately: EITHER Use separate pages for page handlers requiring different authorization OR write a filter that performs authorization as part of IAsyncPageFilter.OnPageHandlerSelectionAsync.

``` csharp
// https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/security/authorization/simple/samples/3.1/PageHandlerAuth/AuthorizeIndexPageHandlerFilter.cs
[TypeFilter(typeof(AuthorizeIndexPageHandlerFilter))]
public class IndexModel : PageModel
{
    public void OnGet() { }

    public void OnPost() { }

    // https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/security/authorization/simple/samples/3.1/PageHandlerAuth/Pages/Index.cshtml.cs#L16
    [AuthorizePageHandler]
    public void OnPostAuthorized() { }
}


``` 


dotnet dev-certs https --clean
dotnet dev-certs https --trust




[FromQuery] - Gets values from the query string.
[FromRoute] - Gets values from route data.
[FromForm] - Gets values from posted form fields.
[FromBody] - Gets values from the request body.
[FromHeader] - Gets values from HTTP headers.