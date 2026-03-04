using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Website.Siegwart.BLL.Dtos.User;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.PL.Services;

namespace Website.Siegwart.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserNewsService _userNewsService;
        private readonly IContactService _contactService;

        public HomeController(
            ILogger<HomeController> logger,
            IUserNewsService userNewsService,
            IContactService contactService)
        {
            _logger = logger;
            _userNewsService = userNewsService;
            _contactService = contactService;
        }

        /// <summary>
        /// Homepage with latest news
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogDebug("Loading homepage");

                var sliderNews = await _userNewsService.GetPublishedNewsAsync();
                ViewBag.SliderNews = sliderNews
                    .OrderByDescending(n => n.PublishedOn)
                    .Take(6)
                    .ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading homepage");
                ViewBag.SliderNews = new List<object>(); // keep DTO type if needed
                return View();
            }
        }

        /// <summary>
        /// About page
        /// </summary>
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Contact page (GET)
        /// </summary>
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// Contact form POST handler
        /// Accepts UserContactFormDto (visitor-side DTO)
        /// Supports AJAX (returns JSON with validation errors) and normal POST (TempData + redirect)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact(UserContactFormDto dto)
        {
            // Honeypot check (hidden field in form named "MiddleName")
            var honeypot = Request.Form["MiddleName"].ToString();
            if (!string.IsNullOrWhiteSpace(honeypot))
            {
                _logger.LogWarning("Contact honeypot triggered. Possible bot. IP: {IP}", HttpContext.Connection.RemoteIpAddress);
                // For bots return a generic success (do not reveal)
                return IsAjaxRequest()
                    ? Json(new { success = true, message = "Thank you." })
                    : RedirectToAction(nameof(Contact));
            }

            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest())
                {
                    var errors = ModelState
                        .Where(kvp => kvp.Value.Errors.Any())
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new { success = false, errors });
                }

                // Non-AJAX: re-display contact view with model errors
                return View("Contact", dto);
            }

            // Map DTO -> ContactFormViewModel (service expects this)
            var vm = new UserContactFormDto
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Subject = dto.Subject,
                Message = dto.Message
            };

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers["User-Agent"].ToString();

            try
            {
                await _contactService.SaveAsync(vm, ip, ua);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save contact message from {Email}", dto.Email);

                if (IsAjaxRequest())
                    return StatusCode(500, new { success = false, message = "Server error. Please try again later." });

                TempData["Error"] = "An error occurred while sending your message. Please try again later.";
                return RedirectToAction(nameof(Contact));
            }

            // Success response
            if (IsAjaxRequest())
            {
                return Json(new { success = true, message = "Thank you � your message has been received." });
            }

            TempData["Success"] = "Thank you � your message has been received.";
            return RedirectToAction(nameof(Contact));
        }

        /// <summary>
        /// Privacy Policy page
        /// </summary>
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Terms of Service page
        /// </summary>
        [HttpGet]
        public IActionResult Terms()
        {
            return View();
        }

        /// <summary>
        /// 404 Not Found page
        /// </summary>
        [HttpGet]
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        /// <summary>
        /// Error page
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorDto
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        // Helper to detect AJAX requests
        private bool IsAjaxRequest()
        {
            if (Request.Headers != null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") return true;
                var accept = Request.Headers["Accept"].ToString();
                if (!string.IsNullOrEmpty(accept) && accept.Contains("application/json")) return true;
            }
            return false;
        }
    }
}