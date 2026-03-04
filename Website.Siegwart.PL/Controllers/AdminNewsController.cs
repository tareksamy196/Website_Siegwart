using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Siegwart.BLL.Dtos.Admin.NewsDtos;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
  
    [Authorize(Roles = "Admin")]
    [Route("admin/news")]
    public class AdminNewsController : BaseAdminController
    {
        private readonly INewsService _newsService;
        private readonly ILogger<AdminNewsController> _logger;
        private readonly IWebHostEnvironment _env;

        public AdminNewsController(
            INewsService newsService,
            ILogger<AdminNewsController> logger,
            IWebHostEnvironment env)
        {
            _newsService = newsService;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Display all news articles
        /// </summary>
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var news = await _newsService.GetAllNewsAsync();
                return View(news);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Index), _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Display create news form
        /// </summary>
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new CreateNewsDto { PublishedOn = DateTime.Now });
        }

        /// <summary>
        /// Create a new news article
        /// </summary>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNewsDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _newsService.CreateNewsAsync(model);

                SetSuccessMessage($"News article '{model.TitleEn}' created successfully.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news: {@Model}", model);

                var errorMsg = _env.IsDevelopment()
                    ? $"Error: {ex.Message}"
                    : "An error occurred while creating the news article.";

                ModelState.AddModelError(string.Empty, errorMsg);
                return View(model);
            }
        }

        /// <summary>
        /// Display edit news form
        /// </summary>
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await _newsService.GetNewsForEditAsync(id);

                if (model == null)
                {
                    SetErrorMessage("News article not found.");
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"{nameof(Edit)} - ID: {id}", _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Update an existing news article
        /// </summary>
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateNewsDto model)
        {
            if (id != model.Id)
            {
                SetErrorMessage("Invalid request. ID mismatch.");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _newsService.UpdateNewsAsync(model);

                SetSuccessMessage($"News article '{model.TitleEn}' updated successfully.");

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                SetErrorMessage($"News article with ID {id} not found.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news: {@Model}", model);

                var errorMsg = _env.IsDevelopment()
                    ? $"Error: {ex.Message}"
                    : "An error occurred while updating the news article.";

                ModelState.AddModelError(string.Empty, errorMsg);
                return View(model);
            }
        }

        /// <summary>
        /// Delete a news article
        /// </summary>
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _newsService.DeleteNewsAsync(id);

                SetSuccessMessage("News article deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                SetErrorMessage($"News article with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"{nameof(Delete)} - ID: {id}", _env.IsDevelopment());
            }

            return RedirectToAction(nameof(Index));
        }
    }
}