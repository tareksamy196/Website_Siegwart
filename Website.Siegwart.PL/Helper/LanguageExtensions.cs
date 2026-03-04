namespace Website.Siegwart.PL.Helper;

public static class LanguageExtensions
{
    public static IServiceCollection AddSiegwartLanguage(this IServiceCollection services, Action<LanguageOptions>? configure = null)
    {
        if (configure is not null)
            services.Configure(configure);
        else
            services.Configure<LanguageOptions>(_ => { });

        return services;
    }

    public static IApplicationBuilder UseSiegwartLanguage(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LanguageMiddleware>();
    }

    public static string GetCurrentLanguage(this HttpContext httpContext)
    {
        if (httpContext.Items.TryGetValue("lang", out var langObj) &&
            langObj is string lang &&
            !string.IsNullOrWhiteSpace(lang))
        {
            return lang;
        }

        return "en";
    }
}