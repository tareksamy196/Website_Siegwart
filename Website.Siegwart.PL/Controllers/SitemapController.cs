using Microsoft.AspNetCore.Mvc;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IUserProductService _productService;
        private readonly IUserNewsService _newsService;
        private readonly IUserTeamMemberService _teamService;

        public SitemapController(
            IUserProductService productService,
            IUserNewsService newsService,
            IUserTeamMemberService teamService)
        {
            _productService = productService;
            _newsService = newsService;
            _teamService = teamService;
        }

        [Route("sitemap.xml")]
        [ResponseCache(Duration = 86400)] // Cache 24 hours
        public async Task<IActionResult> Index()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var products = await _productService.GetActiveProductsAsync();
            var news = await _newsService.GetPublishedNewsAsync();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

            // Static pages
            var staticPages = new[]
            {
                ("", "1.0", "daily"),
                ("Home/About", "0.9", "monthly"),
                ("Home/Contact", "0.8", "monthly"),
                ("UserProducts/Index", "0.9", "weekly"),
                ("UserNews/Index", "0.8", "daily"),
                ("UserTeam/Index", "0.7", "monthly"),
                ("UserVideoMedia/Index", "0.7", "monthly"),
            };

            foreach (var (path, priority, freq) in staticPages)
            {
                sb.AppendLine("<url>");
                sb.AppendLine($"  <loc>{baseUrl}/{path}</loc>");
                sb.AppendLine($"  <changefreq>{freq}</changefreq>");
                sb.AppendLine($"  <priority>{priority}</priority>");
                // hreflang for EN
                sb.AppendLine($"  <xhtml:link rel=\"alternate\" hreflang=\"en\" href=\"{baseUrl}/en/{path}\"/>");
                // hreflang for AR
                sb.AppendLine($"  <xhtml:link rel=\"alternate\" hreflang=\"ar\" href=\"{baseUrl}/ar/{path}\"/>");
                sb.AppendLine("</url>");
            }

            // Dynamic product pages
            foreach (var product in products)
            {
                sb.AppendLine("<url>");
                sb.AppendLine($"  <loc>{baseUrl}/UserProducts/Details/{product.Id}</loc>");
                sb.AppendLine("  <changefreq>monthly</changefreq>");
                sb.AppendLine("  <priority>0.7</priority>");
                sb.AppendLine("</url>");
            }

            // Dynamic news pages
            foreach (var item in news)
            {
                sb.AppendLine("<url>");
                sb.AppendLine($"  <loc>{baseUrl}/UserNews/Details/{item.Id}</loc>");
                sb.AppendLine("  <changefreq>never</changefreq>");
                sb.AppendLine("  <priority>0.6</priority>");
                sb.AppendLine("</url>");
            }

            sb.AppendLine("</urlset>");

            return Content(sb.ToString(), "application/xml");
        }
    }
}