using System.Globalization;
using Microsoft.Extensions.Options;

namespace Website.Siegwart.PL.Helper;

public sealed class LanguageMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LanguageOptions _options;

    public LanguageMiddleware(RequestDelegate next, IOptions<LanguageOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var lang = ResolveLanguage(context);

        var culture = lang.Equals("ar", StringComparison.OrdinalIgnoreCase)
            ? new CultureInfo("ar-EG")
            : new CultureInfo("en-US");

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        // Expose current language for views & helpers
        context.Items["lang"] = lang;

        // Persist only when query-string is used
        if (context.Request.Query.ContainsKey(_options.QueryStringKey))
        {
            context.Response.Cookies.Append(
                _options.CookieName,
                lang,
                new CookieOptions
                {
                    HttpOnly = false,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = context.Request.IsHttps,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
        }

        await _next(context);
    }

    private string ResolveLanguage(HttpContext context)
    {
        // 1) QueryString
        var qs = context.Request.Query[_options.QueryStringKey].ToString();
        if (IsSupported(qs)) return Normalize(qs);

        // 2) Cookie
        if (context.Request.Cookies.TryGetValue(_options.CookieName, out var cookieLang) && IsSupported(cookieLang))
            return Normalize(cookieLang);

        // 3) Accept-Language header (optional)
        var accept = context.Request.Headers.AcceptLanguage.ToString();
        if (!string.IsNullOrWhiteSpace(accept) &&
            accept.Contains("ar", StringComparison.OrdinalIgnoreCase) &&
            IsSupported("ar"))
            return "ar";

        // 4) Default
        return Normalize(_options.DefaultLanguage);
    }

    private bool IsSupported(string? lang)
    {
        if (string.IsNullOrWhiteSpace(lang)) return false;
        return _options.SupportedLanguages.Contains(Normalize(lang), StringComparer.OrdinalIgnoreCase);
    }

    private static string Normalize(string lang) => lang.Trim().ToLowerInvariant();
}