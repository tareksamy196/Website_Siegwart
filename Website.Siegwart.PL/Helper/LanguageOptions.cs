namespace Website.Siegwart.PL.Helper;

public sealed class LanguageOptions
{
    public string DefaultLanguage { get; set; } = "en";
    public string[] SupportedLanguages { get; set; } = ["en", "ar"];
    public string QueryStringKey { get; set; } = "lang";
    public string CookieName { get; set; } = "siegwart.lang";
}





