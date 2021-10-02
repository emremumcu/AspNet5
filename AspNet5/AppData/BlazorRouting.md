# ASP.NET Core Blazor routing

## Route templates

The Router component enables routing to Razor components in a Blazor app. The Router component is used in the App component of Blazor apps.

**App.razor:**

``` csharp
<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <p>Sorry, there's nothing at this address.</p>
    </NotFound>
</Router>
```

When a Razor component (.razor) with an @page directive is compiled, the generated component class is provided a RouteAttribute specifying the component's route template.

When the app starts, the assembly specified as the Router's AppAssembly is scanned to gather route information for the app's components that have a RouteAttribute.

At runtime, the RouteView component:

* Receives the RouteData from the Router along with any route parameters.
* Renders the specified component with its layout, including any further nested layouts.

Optionally specify a DefaultLayout parameter with a layout class for components that don't specify a layout with the @layout directive. The framework's Blazor project templates specify the MainLayout component (Shared/MainLayout.razor) as the app's default layout. For more information on layouts, see ASP.NET Core Blazor layouts.

Components support multiple route templates using multiple @page directives. The following example component loads on requests for /BlazorRoute and /DifferentBlazorRoute.

Pages/BlazorRoute.razor:

``` csharp
@page "/BlazorRoute"
@page "/DifferentBlazorRoute"

<h1>Blazor routing</h1>
```

*For URLs to resolve correctly, the app must include a <base> tag in its wwwroot/index.html file (Blazor WebAssembly) or Pages/_Host.cshtml file (Blazor Server) with the app base path specified in the href attribute.*

## Provide custom content when content isn't found

The Router component allows the app to specify custom content if content isn't found for the requested route. In the App component, set custom content in the Router component's NotFound template.

**App.razor:**

``` csharp
<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <h1>Sorry</h1>
        <p>Sorry, there's nothing at this address.</p> b
    </NotFound>
</Router>
```

## Route to components from multiple assemblies

Use the AdditionalAssemblies parameter to specify additional assemblies for the Router component to consider when searching for routable components. Additional assemblies are scanned in addition to the assembly specified to AppAssembly. In the following example, Component1 is a routable component defined in a referenced component class library. The following AdditionalAssemblies example results in routing support for Component1.

**App.razor:**

``` csharp
<Router
    AppAssembly="@typeof(Program).Assembly"
    AdditionalAssemblies="new[] { typeof(Component1).Assembly }">
    @* ... Router component elements ... *@
</Router>
```

## Route parameters

The router uses route parameters to populate the corresponding component parameters with the same name. Route parameter names are case insensitive. In the following example, the text parameter assigns the value of the route segment to the component's Text property. When a request is made for /RouteParameter/amazing, the <h1> tag content is rendered as Blazor is amazing!.

**Pages/RouteParameter.razor:**

``` csharp
@page "/RouteParameter/{text}"

<h1>Blazor is @Text!</h1>

@code {
    [Parameter]
    public string Text { get; set; }
}
```

Optional parameters are supported. In the following example, the text optional parameter assigns the value of the route segment to the component's Text property. If the segment isn't present, the value of Text is set to fantastic.

**Pages/RouteParameter.razor:**

``` csharp
@page "/RouteParameter/{text?}"

<h1>Blazor is @Text!</h1>

@code {
    [Parameter]
    public string Text { get; set; }

    protected override void OnInitialized()
    {
        Text = Text ?? "fantastic";
    }
}
```

Use OnParametersSet instead of OnInitialized{Async} to permit app navigation to the same component with a different optional parameter value. Based on the preceding example, use OnParametersSet when the user should be able to navigate from /RouteParameter to /RouteParameter/amazing or from /RouteParameter/amazing to /RouteParameter:

``` csharp
protected override void OnParametersSet()
{
    Text = Text ?? "fantastic";
}
```

## Route constraints

A route constraint enforces type matching on a route segment to a component. In the following example, the route to the User component only matches if:

* An Id route segment is present in the request URL.
* The Id segment is an integer (int) type.

**Pages/User.razor:**

``` csharp
@page "/user/{Id:int}"

<h1>User Id: @Id</h1>

@code {
    [Parameter]
    public int Id { get; set; }
}
```

The route constraints shown in the following table are available. For the route constraints that match the invariant culture, see the warning below the table for more information.

``` 
Constraint	Example	Example Matches	Invariant
culture
matching
bool	{active:bool}	true, FALSE	No
datetime	{dob:datetime}	2016-12-31, 2016-12-31 7:32pm	Yes
decimal	{price:decimal}	49.99, -1,000.01	Yes
double	{weight:double}	1.234, -1,001.01e8	Yes
float	{weight:float}	1.234, -1,001.01e8	Yes
guid	{id:guid}	CD2C1638-1638-72D5-1638-DEADBEEF1638, {CD2C1638-1638-72D5-1638-DEADBEEF1638}	No
int	{id:int}	123456789, -123456789	Yes
long	{ticks:long}	123456789, -123456789	Yes
```

Route constraints that verify the URL and are converted to a CLR type (such as int or DateTime) always use the invariant culture. These constraints assume that the URL is non-localizable.

Route constraints also work with optional parameters. In the following example, Id is required, but Option is an optional boolean route parameter.

``` csharp
@page "/user/{Id:int}/{Option:bool?}"

<p>
    Id: @Id
</p>

<p>
    Option: @Option
</p>

@code {
    [Parameter]
    public int Id { get; set; }

    [Parameter]
    public bool Option { get; set; }
}
```

## Routing with URLs that contain dots

For hosted Blazor WebAssembly and Blazor Server apps, the server-side default route template assumes that if the last segment of a request URL contains a dot (.) that a file is requested. For example, the URL https://localhost.com:5001/example/some.thing is interpreted by the router as a request for a file named some.thing. Without additional configuration, an app returns a 404 - Not Found response if some.thing was meant to route to a component with an @page directive and some.thing is a route parameter value. To use a route with one or more parameters that contain a dot, the app must configure the route with a custom template.

Consider the following Example component that can receive a route parameter from the last segment of the URL.

**Pages/Example.razor:**

``` csharp
@page "/example/{param?}"

<p>
    Param: @Param
</p>

@code {
    [Parameter]
    public string Param { get; set; }
}
```

To permit the Server app of a hosted Blazor WebAssembly solution to route the request with a dot in the param route parameter, add a fallback file route template with the optional parameter in Startup.Configure.

**Startup.cs:**

``` csharp
endpoints.MapFallbackToFile("/example/{param?}", "index.html");
```

To configure a Blazor Server app to route the request with a dot in the param route parameter, add a fallback page route template with the optional parameter in Startup.Configure.

**Startup.cs:**

``` csharp
endpoints.MapFallbackToPage("/example/{param?}", "/_Host");
```

## Catch-all route parameters

Catch-all route parameters, which capture paths across multiple folder boundaries, are supported in components.

Catch-all route parameters are:

* Named to match the route segment name. Naming isn't case sensitive.
* A string type. The framework doesn't provide automatic casting.
* At the end of the URL.

**Pages/CatchAll.razor:**

``` csharp
@page "/catch-all/{*pageRoute}"

@code {
    [Parameter]
    public string PageRoute { get; set; }
}
```

For the URL /catch-all/this/is/a/test with a route template of /catch-all/{*pageRoute}, the value of PageRoute is set to this/is/a/test.

Slashes and segments of the captured path are decoded. For a route template of /catch-all/{*pageRoute}, the URL /catch-all/this/is/a%2Ftest%2A yields this/is/a/test*.

## URI and navigation state helpers

Use NavigationManager to manage URIs and navigation in C# code. NavigationManager provides the event and methods shown in the following table.

* Uri:	Gets the current absolute URI.
* BaseUri:	Gets the base URI (with a trailing slash) that can be prepended to relative URI paths to produce an absolute URI. Typically, BaseUri corresponds to the href attribute on the document's <base> element in wwwroot/index.html (Blazor WebAssembly) or Pages/_Host.cshtml (Blazor Server).
* NavigateTo:	Navigates to the specified URI. If forceLoad is true:
    * Client-side routing is bypassed.
    * The browser is forced to load the new page from the server, whether or not the URI is normally handled by the client-side router.
* LocationChanged:	An event that fires when the navigation location has changed.
* ToAbsoluteUri:	Converts a relative URI into an absolute URI.
* ToBaseRelativePath:	Given a base URI (for example, a URI previously returned by BaseUri), converts an absolute URI into a URI relative to the base URI prefix.

For the LocationChanged event, LocationChangedEventArgs provides the following information about navigation events:

* Location: The URL of the new location.
* IsNavigationIntercepted: If true, Blazor intercepted the navigation from the browser. If false, NavigationManager.NavigateTo caused the navigation to occur.

The following component:

* Navigates to the app's Counter component (Pages/Counter.razor) when the button is selected using NavigateTo.
* Handles the location changed event by subscribing to NavigationManager.LocationChanged.
    * The HandleLocationChanged method is unhooked when Dispose is called by the framework. Unhooking the method permits garbage collection of the component.
    * The logger implementation logs the following information when the button is selected:

> BlazorSample.Pages.Navigate: Information: URL of new location: https://localhost:5001/counter

**Pages/Navigate.razor:**

``` csharp
@page "/navigate"
@using Microsoft.Extensions.Logging 
@implements IDisposable
@inject ILogger<Navigate> Logger
@inject NavigationManager NavigationManager

<h1>Navigate in component code example</h1>

<button class="btn btn-primary" @onclick="NavigateToCounterComponent">
    Navigate to the Counter component
</button>

@code {
    private void NavigateToCounterComponent()
    {
        NavigationManager.NavigateTo("counter");
    }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += HandleLocationChanged;
    }

    private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
    {
        Logger.LogInformation("URL of new location: {Location}", e.Location);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }
}
```

## Query string and parse parameters

The query string of a request is obtained from the NavigationManager.Uri property:

``` csharp
@inject NavigationManager NavigationManager

var query = new Uri(NavigationManager.Uri).Query;
```

To parse a query string's parameters, one approach is to use URLSearchParams with JavaScript (JS) interop:

``` csharp
export createQueryString = (string queryString) => new URLSearchParams(queryString);
```

## User interaction with `<Navigating>` content

The Router component can indicate to the user that a page transition is occurring.

At the top of the App component (App.razor), add an @using directive for the Microsoft.AspNetCore.Components.Routing namespace:

``` csharp
@using Microsoft.AspNetCore.Components.Routing
```

Add a `<Navigating>` tag to the component with markup to display during page transition events. For more information, see Navigating (API documentation).

In the router element content (`<Router>...</Router>`) of the App component (App.razor):

``` csharp
<Navigating>
    <p>Loading the requested page&hellip;</p>
</Navigating>
```

## Handle asynchronous navigation events with OnNavigateAsync

The Router component supports an OnNavigateAsync feature. The OnNavigateAsync handler is invoked when the user:

* Visits a route for the first time by navigating to it directly in their browser.
* Navigates to a new route using a link or a NavigationManager.NavigateTo invocation.

In the App component (App.razor):

``` csharp
<Router AppAssembly="@typeof(Program).Assembly" 
    OnNavigateAsync="@OnNavigateAsync">
    ...
</Router>

@code {
    private async Task OnNavigateAsync(NavigationContext args)
    {
        ...
    }
}
```

## Handle cancellations in OnNavigateAsync

The NavigationContext object passed to the OnNavigateAsync callback contains a CancellationToken that's set when a new navigation event occurs. The OnNavigateAsync callback must throw when this cancellation token is set to avoid continuing to run the OnNavigateAsync callback on a outdated navigation.

If a user navigates to an endpoint but then immediately navigates to a new endpoint, the app shouldn't continue running the OnNavigateAsync callback for the first endpoint.

In the following App component example:

* The cancellation token is passed in the call to PostAsJsonAsync, which can cancel the POST if the user navigates away from the /about endpoint.
* The cancellation token is set during a product prefetch operation if the user navigates away from the /store endpoint.

**App.razor:**

``` csharp
@inject HttpClient Http
@inject ProductCatalog Products

<Router AppAssembly="@typeof(Program).Assembly" 
    OnNavigateAsync="@OnNavigateAsync">
    ...
</Router>

@code {
    private async Task OnNavigateAsync(NavigationContext context)
    {
        if (context.Path == "/about") 
        {
            var stats = new Stats = { Page = "/about" };
            await Http.PostAsJsonAsync("api/visited", stats, 
                context.CancellationToken);
        }
        else if (context.Path == "/store")
        {
            var productIds = [345, 789, 135, 689];

            foreach (var productId in productIds) 
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                Products.Prefetch(productId);
            }
        }
    }
}
```

## NavLink and NavMenu components

Use a NavLink component in place of HTML hyperlink elements (<a>) when creating navigation links. A NavLink component behaves like an <a> element, except it toggles an active CSS class based on whether its href matches the current URL. The active class helps a user understand which page is the active page among the navigation links displayed. Optionally, assign a CSS class name to NavLink.ActiveClass to apply a custom CSS class to the rendered link when the current route matches the href.

The following NavMenu component creates a Bootstrap navigation bar that demonstrates how to use NavLink components:

``` csharp
<div class="@NavMenuCssClass" @onclick="@ToggleNavMenu">
    <ul class="nav flex-column">
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <span class="oi oi-home" aria-hidden="true"></span> Home
            </NavLink>
        </li>
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="component" Match="NavLinkMatch.Prefix">
                <span class="oi oi-plus" aria-hidden="true"></span> Link Text
            </NavLink>
        </li>
    </ul>
</div>

@code {
    private string NavMenuCssClass;
    private void ToggleNavMenu() {}
}
```

There are two NavLinkMatch options that you can assign to the Match attribute of the `<NavLink>` element:

* NavLinkMatch.All: The NavLink is active when it matches the entire current URL.
* NavLinkMatch.Prefix (default): The NavLink is active when it matches any prefix of the current URL.

In the preceding example, the Home NavLink href="" matches the home URL and only receives the active CSS class at the app's default base path URL (for example, https://localhost:5001/). The second NavLink receives the active class when the user visits any URL with a component prefix (for example, https://localhost:5001/component and https://localhost:5001/component/another-segment).

Additional NavLink component attributes are passed through to the rendered anchor tag. In the following example, the NavLink component includes the target attribute:

``` csharp
<NavLink href="example-page" target="_blank">Example page</NavLink>
```

The following HTML markup is rendered:

``` html
<a href="example-page" target="_blank">Example page</a>
```

Due to the way that Blazor renders child content, rendering NavLink components inside a for loop requires a local index variable if the incrementing loop variable is used in the NavLink (child) component's content:

``` csharp
@for (int c = 0; c < 10; c++)
{
    var current = c;
    <li ...>
        <NavLink ... href="@c">
            <span ...></span> @current
        </NavLink>
    </li>
}
```

Using an index variable in this scenario is a requirement for any child component that uses a loop variable in its child content, not just the NavLink component.

Alternatively, use a foreach loop with Enumerable.Range:

``` csharp
@foreach(var c in Enumerable.Range(0,10))
{
    <li ...>
        <NavLink ... href="@c">
            <span ...></span> @c
        </NavLink>
    </li>
}
```

# ASP.NET Core endpoint routing integration

*This section only applies to Blazor Server apps.*

Blazor Server is integrated into ASP.NET Core Endpoint Routing. An ASP.NET Core app is configured to accept incoming connections for interactive components with MapBlazorHub in Startup.Configure.

**Startup.cs:**

``` csharp
using Microsoft.AspNetCore.Builder;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
```

The typical configuration is to route all requests to a Razor page, which acts as the host for the server-side part of the Blazor Server app. By convention, the host page is usually named _Host.cshtml in the Pages folder of the app.

The route specified in the host file is called a fallback route because it operates with a low priority in route matching. The fallback route is used when other routes don't match. This allows the app to use other controllers and pages without interfering with component routing in the Blazor Server app.

# References

https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing?view=aspnetcore-5.0

# Routing in Blazor

A Route is a URL pattern and Routing is a pattern matching process that monitors the requests and determines what to do with each request.
 
Blazor server app uses ASP.net Core Endpoint Routing. Using MapBlazorHub extension method of endpoint routing, ASP.net Core is start to accept the incoming connection for Blazor component. The Blazor client app provides the client-side Routing. The router is configured in Blazor client app in App.cshtml file.

**Blazor Client app**

``` csharp
<Router AppAssembly="@typeof(Program).Assembly"/>
```

**Blazor Server app**

``` csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)  
{
    app.UseRouting();  
  
    app.UseEndpoints(endpoints =>  
    {  
        endpoints.MapBlazorHub();  
        endpoints.MapFallbackToPage("/_Host");  
    }); 
}
```

The Blazor Server app allows to set fallback route. It operates with low priority in routing matching. The fallback route is only considered when other routes are not matched. The fallback route is usually defined in _Host.cshtml component.

**@page directive**
 
Using @page directive, you can define the routing in Blazor component. The @page directives are internally converted into RouteAttribute when template is compiled.

``` csharp
@page "/route1"
```

In Blazor, Component can have multiple routes. If we require that component can render from multiple route values, we need to define all routes with multiple @page directives.

``` csharp
@page "/route1"
@page "/route2"
```

If you have defined class only component, you can use RouteAttribute.

``` csharp
using Microsoft.AspNetCore.Components;  
  
namespace BlazorServerApp.Pages  
{  
    [Route("/classonly")]  
    public class ClassOnlyComponent: ComponentBase  
    {  
       ...  
    }  
} 
```

**Route Template**
 
The Router mechanism allows to define the routing for each component. The route template is defined in App.razor file. Here, we can define default layout page and default route data.

``` csharp
<Router AppAssembly="@typeof(Program).Assembly">  
    <Found Context="routeData">  
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />  
    </Found>  
    <NotFound>  
        <LayoutView Layout="@typeof(MainLayout)">  
            <p>Sorry, there's nothing at this address.</p>  
        </LayoutView>  
    </NotFound>  
</Router> 
```

In above code, three components are defined under Router component: Found, NotFound and RouteView. The RouteView component receive the route data and default layout. Blazor routing mechanism render the Found component, if any route matched else render NotFound component. The NotFound component allows us to provide custom message when route or content not found.

**Handle Parameter in Route Template**
 
Route may have parameters. The parameters can be defined using curly braces within the routing template in both @page directive or RouteAttribute. The route parameters are automatically bound with component parameters by matching the name. This matching is case insensitive.

``` csharp
<h3>Route Constraints Example</h3>  
@page "/routepara/{Name}"  
  
<h4>Name : @Name </h4>  
  
@code {  
    [Parameter]  
    public string Name { get; set; }  
}
```

Current version of Blazor does not supports the optional parameter , so you must pass parameter in above example.
 
**Route constraints**
 
The Blazor routing also allows Route constraints. It enforces type matching between route parameter and route data. Current version of Blazor supports few route constraints but might supports many more route constraints in the future.

``` csharp
<h3>Route Constraints Example</h3>  
@page "/routecons/{Id:guid}"  
  
<h4>Id : @Id </h4>  
  
@code {  
    [Parameter]  
    public Guid Id { get; set; }  
}  
```

Following route constraints are supported by Blazor

```
Constraint	Invariant culture matching	Example 
 int	    Yes   	 {id:int}
 long 	    Yes	 {id:long}
 float	    Yes	 {mrp:float}
 double	  Yes	 {mrp:double}
 decimal	  Yes	 {mrp:decimal}
 guid       No	    {id:guid}
 bool	     No	    {enabled:bool}
 datetime	 Yes	 {birthdate:datetime}
```

**NavLink Component**
 
Blazor provides NavLink component that generates HTML hyperlink element and it handles the toggle of active CSS class based on NavLink component href match with the current URL.
 
There are two options for assign to Match attribute of NavLink component
* NavLinkMatch.All: Active when it matches the entire current URL
* NavLinkMatch.Prefix: It is a default option. It active when it matches any prefix of the current URL

The NavLink component renders as the anchor tag. You can also include the target attribute.
 
**Programmatically navigate one component to another component**
 
Blazor is also allowed to navigate from one component to another component programmatically using Microsoft.AspNetCore.Components.NavigationManager. The NavigationManager service provides the following events and properties.

```
Event / Method	Description
 NavigateTo	 It navigates to specified URI. It takes parameter "forceload", if its parameter is set to true, client-side routing is bypassed and the browser is forced to load a new page 
 ToAbsoluteUri 	 It converts relative URI to absolute URI
 ToBaseRelativePath	 Returns relative URI with base URI prefix
 NotifyLocationChanged	 This event is fired when browser location has changed
 EnsureInitialized	 This method allows derived class to lazy self-initialized
 Initialize	 This method set base URI and current URI before they are used
 NavigateToCore	 Navigate to specified URI. This is an abstract method hence it must implement in a derived class
 
Properties	Description
 BaseUri	 Get and set the current base URI. It allows representing as an absolute URI end with a slash
 Uri 	 Get and set the current URI. It allows representing as an absolute URI
```

To navigate URI using NavigationManager service, you must inject the service using @inject directive in component. Using the NavigateTo method, you can navigate from one component to another component.

``` csharp
@page "/navexample"  
@inject NavigationManager UriHelper  
<h3>Navigation Example</h3>  
Navigate to other component <a href="" @onclick="NavigatetoNextComponent">Click here</a>  
  
@code {  
    void NavigatetoNextComponent()  
    {  
        UriHelper.NavigateTo("newcomponent");  
    }  
}  
```

You can also capture query string or query parameter value when redirect to another component. Using QueryHelpers class, we can access query string and query parameter of the component. The QueryHelpers.ParseQuery method extract the value from the query string. This method returns the dictionary of type Dictionary<string, StringValues> that contains all query parameter of the route.

``` csharp
<h3>New Component with Parameter</h3>  
@page "/newcomponentwithpara"  
@inject NavigationManager UriHelper  
@using Microsoft.AspNetCore.WebUtilities;  
  
<p> Parameter value : @Name</p>  
  
@code {  
    public string Name { get; set; }  
    protected override void OnInitialized()  
    {  
        var uri = UriHelper.ToAbsoluteUri(UriHelper.Uri);  
        Name = QueryHelpers.ParseQuery(uri.Query).TryGetValue("name", out var type) ? type.First() : "";  
    }  
}  
```

# References

https://www.c-sharpcorner.com/article/routing-in-blazor/
