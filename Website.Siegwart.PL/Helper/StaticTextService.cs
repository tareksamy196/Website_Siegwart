using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace Website.Siegwart.PL.Helper;

public interface IStaticTextService
{
    string Header(string key);
    string Footer(string key);
    string About(string path);
    string Home(string path);
    string Contact(string path);
    string Seo(string key);
}

public sealed class StaticTextService : IStaticTextService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _http;

    private JsonDocument? _headerDoc;
    private JsonDocument? _footerDoc;
    private JsonDocument? _aboutDoc;
    private JsonDocument? _homeDoc;
    private JsonDocument? _seoDoc;
    private JsonDocument? _ContactDoc;

    public StaticTextService(IWebHostEnvironment env, IHttpContextAccessor http)
    {
        _env = env;
        _http = http;
    }

    public string Header(string key)
    {
        _headerDoc ??= LoadJson("header.json");
        return GetValue(_headerDoc, key);
    }

    public string Footer(string key)
    {
        _footerDoc ??= LoadJson("footer.json");
        return GetValue(_footerDoc, key);
    }

    public string About(string path)
    {
        _aboutDoc ??= LoadJson("about.json");
        return GetValue(_aboutDoc, path);
    }

    public string Home(string path)
    {
        _homeDoc ??= LoadJson("home.json");
        return GetValue(_homeDoc, path);
    }

    public string Seo(string key)
    {
        _seoDoc ??= LoadJson("Seo.json");
        return GetValue(_seoDoc, key);
    }
    public string Contact(string path)
    {
        _ContactDoc ??= LoadJson("Contact.json");
        return GetValue(_ContactDoc, path);
    }

    private string GetValue(JsonDocument? doc, string path)
    {
        if (doc == null) return path;

        var lang = CurrentLang();

        // Try current language first
        var val = TryGetJsonPath(doc.RootElement, $"{lang}.{path}");

        // Fallback to English
        if (val == null && lang != "en")
        {
            val = TryGetJsonPath(doc.RootElement, $"en.{path}");
        }

        return val ?? path;
    }

    private string CurrentLang()
    {
        var lang = _http.HttpContext?.GetCurrentLanguage() ?? "en";
        return lang == "ar" ? "ar" : "en";
    }

    private JsonDocument LoadJson(string fileName)
    {
        // Try new path first
        var newPath = Path.Combine(_env.WebRootPath, "Resources", "StaticTexts", fileName);

        if (File.Exists(newPath))
        {
            var json = File.ReadAllText(newPath);
            return JsonDocument.Parse(json);
        }

        // Fallback to old path for backward compatibility
        var oldPath = Path.Combine(_env.WebRootPath, "i18n", fileName);

        if (File.Exists(oldPath))
        {
            var json = File.ReadAllText(oldPath);
            return JsonDocument.Parse(json);
        }

        throw new FileNotFoundException($"Static text file not found: {fileName}");
    }

    private static string? TryGetJsonPath(JsonElement root, string path)
    {
        var current = root;

        foreach (var token in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            if (current.ValueKind == JsonValueKind.Object)
            {
                if (!current.TryGetProperty(token, out var next))
                    return null;
                current = next;
                continue;
            }

            if (current.ValueKind == JsonValueKind.Array)
            {
                if (!int.TryParse(token, out var index))
                    return null;
                if (index < 0 || index >= current.GetArrayLength())
                    return null;
                current = current[index];
                continue;
            }

            return null;
        }

        return current.ValueKind switch
        {
            JsonValueKind.String => current.GetString(),
            JsonValueKind.Number => current.ToString(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => null
        };
    }

}