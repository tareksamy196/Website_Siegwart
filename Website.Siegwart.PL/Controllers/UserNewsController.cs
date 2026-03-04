using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
 
    [Route("news")]
    public class UserNewsController : Controller
    {
        private readonly IUserNewsService _newsService;
        private readonly ILogger<UserNewsController> _logger;

        public UserNewsController(
            IUserNewsService newsService,
            ILogger<UserNewsController> logger)
        {
            _newsService = newsService;
            _logger = logger;
        }

        /// <summary>
        /// Display all published news articles
        /// </summary>
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogDebug("Loading news page");

                var newsList = await _newsService.GetPublishedNewsAsync();

                return View(newsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news list");
                return View(new List<BLL.Dtos.User.NewsViewDto>());
            }
        }

        /// <summary>
        /// Display news article details
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogDebug("Loading news details: {Id}", id);

                var news = await _newsService.GetNewsByIdAsync(id);

                if (news == null)
                {
                    _logger.LogWarning("News not found: {Id}", id);
                    return NotFound();
                }

                return View(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news details: {Id}", id);
                return NotFound();
            }
        }
    }
}