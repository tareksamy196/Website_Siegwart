using System.Text.RegularExpressions;

namespace Website.Siegwart.Core.Helpers
{
    public static class YouTubeHelper
    {
        private static readonly Regex IdRegex = new Regex(
            @"(?:youtube\.com/(?:.*v=|v/|embed/|shorts/)|youtu\.be/)([A-Za-z0-9_-]{11})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string? ExtractVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            var m = IdRegex.Match(url);
            if (m.Success && m.Groups.Count > 1) return m.Groups[1].Value;
            // fallback if admin provides id only
            if (url.Length == 11 && Regex.IsMatch(url, @"^[A-Za-z0-9_-]{11}$")) return url;
            return null;
        }

        public static string GetThumbnail(string videoId, bool preferMaxRes = false)
        {
            return preferMaxRes
                ? $"https://img.youtube.com/vi/{videoId}/maxresdefault.jpg"
                : $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
        }

        public static string GetEmbedUrl(string videoId)
        {
            return $"https://www.youtube-nocookie.com/embed/{videoId}";
        }
    }
}