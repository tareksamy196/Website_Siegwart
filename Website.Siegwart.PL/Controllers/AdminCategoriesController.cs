using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Siegwart.BLL.Dtos.Admin.CategoryDtos;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("admin/categories")]
    public class AdminCategoriesController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<AdminCategoriesController> _logger;
        private readonly IWebHostEnvironment _env;

        public AdminCategoriesController(
            ICategoryService categoryService,
            ILogger<AdminCategoriesController> logger,
            IWebHostEnvironment env)
        {
            _categoryService = categoryService;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Display all categories
        /// </summary>
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Index), _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Display create category form
        /// </summary>
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new CreateCategoryDto());
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var result = await _categoryService.CreateCategoryAsync(dto);

                if (result > 0)
                {
                    SetSuccessMessage($"Category '{dto.NameEn}' created successfully!");
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to create category.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error creating category");
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {@Dto}", dto);

                var errorMsg = _env.IsDevelopment()
                    ? $"Error: {ex.Message}"
                    : "An unexpected error occurred.";

                ModelState.AddModelError(string.Empty, errorMsg);
            }

            return View(dto);
        }

        /// <summary>
        /// Display edit category form
        /// </summary>
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _categoryService.GetCategoryForEditAsync(id);

                if (dto == null)
                {
                    SetErrorMessage("Category not found.");
                    return RedirectToAction(nameof(Index));
                }

                return View(dto);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"{nameof(Edit)} - ID: {id}", _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDto dto)
        {
            if (id != dto.Id)
            {
                SetErrorMessage("Invalid request. ID mismatch.");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var result = await _categoryService.UpdateCategoryAsync(dto);

                if (result > 0)
                {
                    SetSuccessMessage($"Category '{dto.NameEn}' updated successfully!");
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to update category.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating category");
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (KeyNotFoundException)
            {
                SetErrorMessage($"Category with ID {id} not found.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category: {@Dto}", dto);

                var errorMsg = _env.IsDevelopment()
                    ? $"Error: {ex.Message}"
                    : "An unexpected error occurred.";

                ModelState.AddModelError(string.Empty, errorMsg);
            }

            return View(dto);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (result > 0)
                    SetSuccessMessage("Category deleted successfully!");
                else
                    SetWarningMessage("Category not found or already deleted.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete category: {Id}", id);
                SetErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"{nameof(Delete)} - ID: {id}", _env.IsDevelopment());
            }

            return RedirectToAction(nameof(Index));
        }
    }
}