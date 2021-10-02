``` csharp
public static class Init
{
    public static IServiceCollection _(this IServiceCollection services)
    {
        return services;
    }

    public static IApplicationBuilder _(this IApplicationBuilder app)
    {
        return app;
    }
}
```