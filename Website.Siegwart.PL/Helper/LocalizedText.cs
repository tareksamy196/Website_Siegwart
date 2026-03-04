namespace Website.Siegwart.PL.Helper;

public static class LocalizedText
{
    public static string Pick(HttpContext httpContext, string? en, string? ar, string fallback = "")
    {
        var lang = httpContext.GetCurrentLanguage();

        if (lang == "ar")
            return string.IsNullOrWhiteSpace(ar) ? string.IsNullOrWhiteSpace(en) ? fallback : en! : ar!;

        return string.IsNullOrWhiteSpace(en) ? string.IsNullOrWhiteSpace(ar) ? fallback : ar! : en!;
    }

    public static bool IsArabic(HttpContext httpContext)
        => httpContext.GetCurrentLanguage().Equals("ar", StringComparison.OrdinalIgnoreCase);
}