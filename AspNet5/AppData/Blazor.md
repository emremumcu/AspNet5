# Interactive web UI with C#

Blazor lets you build interactive web UIs using C# instead of JavaScript. Blazor apps are composed of reusable web UI components implemented using C#, HTML, and CSS. Both client and server code is written in C#, allowing you to share code and libraries.

Blazor can run your client-side C# code directly in the browser, using WebAssembly. Because it's real .NET running on WebAssembly, you can re-use code and libraries from server-side parts of your application.

Alternatively, Blazor can run your client logic on the server. Client UI events are sent back to the server using SignalR - a real-time messaging framework. Once execution completes, the required UI changes are sent to the client and merged into the DOM.The document object model(DOM) is a programming interface that represents all elements on an HTML page as nodes in a tree structure. Using the DOM, elements can be updated, added, and removed from the page.

Blazor uses open web standards without plug-ins or code transpilation. Blazor works in all modern web browsers, including mobile browsers.

Code running in the browser executes in the same security sandbox as JavaScript frameworks. Blazor code executing on the server has the flexibility to do anything you would normally do on the server, such as connecting directly to a database.

Your C# code can easily call JavaScript APIs and libraries. You can continue to use the large ecosystem of JavaScript libraries that exist for client side UI while writing your logic in C#.

When using server-side code execution, Blazor takes care of seamlessly executing any JavaScript code on the client.

Get productive fast with re-usable UI components from top component vendors like Telerik, DevExpress, Syncfusion, Radzen, Infragistics, GrapeCity, jQWidgets, and others. Or use one of the many open-source component libraries from the Blazor community.

# How we develop web applications today?

* For server-side development, we use programming languages like C#, Java, PHP etc. These are the server-side programming languages.
* For the client-side development we use JavaScript frameworks like Angular, React, Vue etc. There’s no doubt these JavaScript frameworks dominated client-side development up until recently. 

With Blazor we can now build interactive web UIs using C# instead of JavaScript. C# code can be executed both on the server and in the client browser. This means existing .Net developers can reuse their c# skills rather than learning new JavaScript frameworks and their huge learning curve.

Browsers understand and execute only JavaScript. How can we execute c# code in the client browser? 

Blazor can run C# code directly in the browser, using WebAssembly. It runs in the same security sandbox as JavaScript frameworks like Angular, React, Vue etc. Not just C#, in fact, we can run any type of code in the browser using WebAssembly.

WebAssembly is based on open web standards. So it is a native part of all modern browsers including mobile browsers. This means for the blazor application to work, there is no need to install any special plugin like back in the days of silver light and flash.

Blazor offers 2 hosting models. Blazor WebAssembly and Blazor Server.

## Blazor WebAssembly

This is also called the client-side hosting model and in this model, the application runs directly in the browser on WebAssembly. So, everything the application needs i.e the compiled application code itself, it's dependencies and the .NET runtime are downloaded to the browser. We use the Blazor WebAssembly App template, to create a Blazor application with the client-side hosting model. 

With this hosting model, the application runs directly in the browser on WebAssembly. So, everything the application needs i.e the compiled application, it's dependencies and the .NET runtime are downloaded to the client browser from the server. A Blazor WebAssembly app can run entirely on the client without a connection to the server or we can optionally configure it to interact with the server using web API calls or SignalR.

**Blazor WebAssembly hosting model benefits :**

* A Blazor WebAssembly app can run entirely on the client machine. So, after the application is downloaded, a connection to the server is not required. This means there is no need for your server to be up and running 24X7. 
* Work is offloaded from the server to the client. It is the client resources and capabilities that are being used.
* We do not need a full-blown ASP.NET Core web server to host the application. We just need a server somewhere, that can deliver the application to the client browser. This means we can host the application on our own server on the internet somewhere, in the cloud, on Azure as a static website or even on a CDN Content Delivery Network.

**Downsides of Blazor WebAssembly hosting :**

* The very first request usually takes longer as the entire app, it's dependencies and the .NET runtime must be downloaded to the client browser. Keep in mind, it's only the first request that takes longer than usual. If that same client visits the application later, it usually starts fast because the browser caches the files.
* Since the app runs entirely on the client browser, it is restricted to the capabilities of the browser.
* Depending on the nature of the application, capable client hardware and software is required. From software standpoint, for example, at least a browser with WebAssembly support is required.

## Blazor server

This is also called the server hosting model and in this model, the application is executed on the server from within an ASP.NET Core application. Between the client and the server, a SignalR connection is established. When an event occurs on the client such as a button click, for example, the information about the event is sent to the server over the SignalR connection. The server handles the event and for the generated HTML a diff (difference) is calculated. The entire HTML is not sent back again to the client, it's only the diff that is sent to the client over the established SignalR connection. The browser then updates the UI. Blazor embraces the single page application architecture which rewrites the same page dynamically in response to the user action. Since only the diff is applied to update the UI, the application feels faster and more responsive to the user.

With this hosting model, the application is executed on the server. Between the client and the server a SignalR connection is established. When an event occurs on the client such as a button click for example, the information about the event is sent to the server over the SignalR connection. The server handles the event and for the generated HTML a diff (difference) is calculated. The entire HTML is not sent again to the client, it's only the diff that is sent to the client over the SignalR connection. The browser then updates the UI. Since only the diff is applied to update the UI, the application feels faster and more responsive to the user.

**Blazor Server hosting model benefits :**

* The app loads much faster as the download size is significantly smaller than a Blazor WebAssembly app
* Since the app runs on the server, it can take full advantage of server capabilities, including use of any .NET Core compatible APIs.
* All the client needs, to use the app is a browser. Even browsers that don't support WebAssembly can be used.
* More secure because the app's .NET/C# code isn't served to clients.

**Downsides of Blazor Server hosting :**

* A full-blown ASP.NET Core server is required to host the application. Serverless deployment scenarios such as serving the app from a CDN aren't possible.
* An active connection to the server is always required. This means there is a need to keep the server up and running 24X7. If the server is down, the application stops working.
* As every user interaction involves a round trip to the server a higher latency usually exists when compared with Blazor WebAssembly hosting.
* Scalability can be challenging especially for the apps that have many users as the server must manage multiple client connections and handle client state. However, we can overcome this scalability issue, by using Azure SignalR Service with a Blazor Server app. This service allows a Blazor Server app to scale really well by supporting a large number of concurrent SignalR connections.

As the template names imply, use the Blazor Server App template to create a blazor application with server hosting model and Blazor WebAssembly template to create a blazor application with client hosting model.

## Blazor project structure

**Program.cs**  
This file contains the Main() method which is the entry point for both the project types (i.e Blazor WebAssembly and Blazor Server). 

In a Blazor server project, the Main() method calls CreateHostBuilder() method which sets up the ASP.NET Core host. 

In a Blazor WebAssembly project, the App component, which is the root component of the application, is specified in the Main method. This root component is present in the root project folder in App.razor file.

**wwwroot**  
For both the project types, this folder contains the static files like images, stylesheets etc. 

**App.razor**  
This is the root component of the application. It uses the built-in Router component and sets up client-side routing. It is this Router component that intercepts browser navigation and renders the page that matches the requested address. The Router uses the Found property to display the content when a match is found. If a match is not found, the NotFound property is used to display the  message - Sorry, there's nothing at this address.

**Pages folder**  
This folder contains the _Host razor page and the routable components that make up the Blazor app. The components have the .razor extension.

**_Imports.razor**  
This is like the _ViewImports.cshtml file in an asp.net core MVC project. This file contains the common namespaces so we do not have to include them in every razor component.

**wwwroot/index.html**  
This is the root page in a Blazor WebAssembly project and is implemented as an html page. When a first request hits the application, it is this page, that is initially served. It has the standard HTML, HEAD and BODY tags. It specifies where the root application component App.razor should be rendered. You can find this App.razor root component in the root project folder. It is included on the page as an HTML element `<app>`. 

This index.html page also loads the blazor WebAssembly JavaScript file (_framework/blazor.webassembly.js). It is this file that is responsible for downloading 

The compiled blazor application, it's dependencies and the .NET runtime
It also initializes the runtime to run the blazor app in the browser

**Startup.cs**  
This file is present only in the Blazor Server project. As the name implies it contains the applications's startup logic. The Startup class contains the following two methods.

ConfigureServices  
Configures the applications DI i.e dependency injection services. 

Configure  
Configures the app's request processing pipeline. Depending on what we want the Blazor application to be capable of doing we add or remove the respective middleware components from request processing pipeline. 

MapBlazorHub sets up the endpoint for the SignalR connection with the client browser.

**Pages/_Host.cshtml**  
This is the root page of the application and is specified by calling MapFallbackToPage("/_Host") method. It is implemented as a razor page.

It is this page, that is initially served when a first request hits the application. It has the standard HTML, HEAD and BODY tags. It also specifies where the root application component, App component (App.razor) must be rendered. Finally, it also loads the blazor.server.js JavaScript file, which sets up the real-time SignalR connection between the server and the client browser. This connection is used to exchange information between the client and the server. SignalR, is a great framework for adding real-time web functionality to apps. 

**Data folder (Blazor Server)**  
Contains code files related to the sample WeatherForecast service

**appsettings.json (Blazor Server)**  
Just like an asp.net core mvc project, a blazor project also uses this file to store the Configuration settings.

## ASP.NET core razor components

Blazor is a component-driven framework, meaning components are the fundamental building blocks of a Blazor application. They can be nested, reused, and if implemented properly, can even be shared across multiple projects. Component files have the extension .razor

The following is the Counter component that we get when we create a new Blazor project.

``` csharp
@page "/counter"
<h1>Counter</h1>
<p>Current count: @currentCount</p>
<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
@code 
{
    private int currentCount = 0;
    private void IncrementCount()
    {
        currentCount++;
    }
}
```

It is a combination of two things

* HTML markup which defines the user interface of the component i.e the look and feel.
* C# code which defines the processing logic

When the application is compiled, the HTML and C# code converted into a component class. The name of the generated class matches the name of the component file. A component file name must start with an uppercase character. If you add a component file that starts with a lower case character, the code will fail to compile and you get the following compiler error.

> Component names cannot start with a lowercase character

Remember, blazor server project runs on the server.

* A SignalR connection is established between the server and the client browser. After the counter component is initially rendered and when the user clicks the button.
* The information about the click event is sent to the server over the SignalR connection.
* In response to the event, the component is regenerated, but the entire HTML is not sent back to the client. It's only the diff, i.e the difference in the render tree, in this case, the new counter value that is sent to the client browser.
* Since only the changed part of the page is updated instead of reloading and updating the entire page, the application feels faster and more responsive to the user.

## Nesting razor components
One way to have the Counter component rendered is by navigating to /counter in the browser. This path is specified by the @page directive at the top of the component.

``` csharp
@page "/counter"
```

A component can also be nested inside another component using HTML syntax. For example, use `<Counter />` to nest the Counter component in the Index component.

Components can be placed anywhere within a blazor project. It's a good practice to place components that produce webpages in the Pages folder and reusable non-page components in the Shared folder. If you want to, you can also place them in a completely different custom folder within your project.

## Split razor component

There are 2 approaches, to split component HTML and C# code into their own separate files.

* Partial files approach
* Base class approach

### Single file or Mixed file approach

Both the HTML markup and C# code are in a single file.

``` csharp
// Counter.razor
@page "/counter"
<h1>Counter</h1>
<p>Current count: @currentCount</p>
<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

### Partial files approach

The HTML markup remains in Counter.razor file. 

``` csharp
// Counter.razor
@page "/counter"
<h1>Counter</h1>
<p>Current count: @currentCount</p>
<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
```

Remember, when the component is compiled a class with the same name as the component file is generated. Create another class file with name Counter.razor.cs and include the following code in it. Notice the class in this file is implemented as a partial class.

``` csharp
// Counter.razor.cs
public partial class Counter
{
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

### Base class approach

Just like the partial class approach, even with the base class approach, the HTML markup remains in Counter.razor file.

``` csharp
// Counter.razor
@page "/counter"
<h1>Counter</h1>
<p>Current count: @currentCount</p>
<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
```

Move the C# code to a separate class. I named it CounterBase. You can name the class anything you want, but it is a common convention to have the same name as the component but suffixed with the word Base. 

In this example, the component name is Counter. So the class that contains the c# code is named CounterBase. The class has to inherit from the built-in ComponentBase class. This class is in Microsoft.AspNetCore.Components namespace.

The access modifier must be at least protected if you wish to access the class member from the HTML.

``` csharp
// CounterBase.cs
public class CounterBase : ComponentBase
{
    protected int currentCount = 0;

    protected void IncrementCount()
    {
        currentCount++;
    }
}
```

Finally, in Counter.razor file do not forget to include the following inherits directive.

``` csharp
@inherits CounterBase
```

# Hosting models
Blazor currently has three hosting models:

* *Blazor WebAssembly (client side).
* *Blazor Server (server side).
* *ASP.NET Core.

Microsoft launched the Blazor server-side hosting model in September 2019 and WebAssembly model in May 2020.

## Blazor WebAssembly (Client Side)
According to Microsoft’s official documentation , a client-side Blazor WebAssembly (Wasm) application runs in the browser. When a user opens a web page or web application, all the code related to the client-side logic will be downloaded. This means that all the dependencies will also be downloaded.  So, the necessary execution time will be relative to run the application. Once we download everything, if we were to disconnect there would be no problem. Since the Blazor WebAssembly hosting model allows us to continue using the application in offline mode and we can synchronize the changes later.

## Blazor Server (Server Side)
If we work with the server-side hosting model, the Blazor application will obviously run on the server and every change or event that happens on the client side will be sent to the server through SignalR communication. The server will then process the events or changes and update the client-side UI if necessary. This means that the UI rendering happens on the server side.

## ASP.NET Core
The last but not least hosting model is the ASP.NET Core. It is an improved version of the client-side hosting model and perfectly suites to browsers. This hosting model sends the client part of the Blazor application to the browser and connects to the server using SignalR communication.

So the simplest way to host Blazor WebAssembly app would be to also use a ASP.NET Core web app to serve it.

# References

* https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor
* https://github.com/AdrienTorris/awesome-blazor?WT.mc_id=dotnet-35129-website#libraries--extensions
* https://www.pragimtech.com/blog/blazor/what-is-blazor/
* https://www.syncfusion.com/blogs/post/3-blazor-hosting-models.aspx