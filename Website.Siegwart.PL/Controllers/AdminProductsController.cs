using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Website.Siegwart.BLL.Dtos.Admin.ProductDtos;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
   
    [Authorize(Roles = "Admin")]
    [Route("admin/products")]
    public class AdminProductsController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<AdminProductsController> _logger;
        private readonly IWebHostEnvironment _env;

        public AdminProductsController(
            IProductService productService,
            ICategoryService categoryService,
            ILogger<AdminProductsController> logger,
            IWebHostEnvironment env)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Display all products
        /// </summary>
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Index), _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Display create product form
        /// </summary>
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateCategoriesDropdownAsync();
                return View(new CreateProductDto());
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Create), _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesDropdownAsync();
                return View(dto);
            }

            try
            {
                var result = await _productService.CreateProductAsync(dto);

                if (result > 0)
                {
                    SetSuccessMessage($"Product '{dto.TitleEn}' created successfully!");
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to create product.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {@Dto}", dto);

                var errorMsg = _env.IsDevelopment()
                    ? $"Error: {ex.Message}"
                    : "An unexpected error occurred.";

                ModelState.AddModelError(string.Empty, errorMsg);
            }

            await PopulateCategoriesDropdownAsync();
            return View(dto);
        }

        /// <summary>
        /// Display edit product form
        /// </summary>
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _productService.GetProductForEditAsync(id);

                if (dto == null)
                {
                    SetErrorMessage("Product not found!");
                    return RedirectToAction(nameof(Index));
                }

                await PopulateCategoriesDropdownAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"{nameof(Edit)} - ID: {id}", _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto dto)
        {
            if (id != dto.Id)
            {
                SetErrorMessage("Invalid request. ID mismatch.");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await PopulateCategoriesDropdownAsync();
                return View(dto);
            }

            try
            {
                var result = await _productService.UpdateProductAsync(dto);

                if (result > 0)
                {
                    SetSuccessMessage($"Product '{dto.TitleEn}' updated successfully!");
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to update product.");
            }
            catch (KeyNotFoundException)
            {
                SetErrorMessage($"Product with ID {id} not found.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {@Dto}", dto);

                var errorMsg = _env.IsDevelopment()
                    ? $"Error: {ex.Message}"
                    : "An unexpected error occurred.";

                ModelState.AddModelError(string.Empty, errorMsg);
            }

            await PopulateCategoriesDropdownAsync();
            return View(dto);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);

                if (result > 0)
                    SetSuccessMessage("Product deleted successfully!");
                else
                    SetWarningMessage("Product not found or already deleted.");
            }
            catch (KeyNotFoundException)
            {
                SetErrorMessage($"Product with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"{nameof(Delete)} - ID: {id}", _env.IsDevelopment());
            }

            return RedirectToAction(nameof(Index));
        }

        #region Helper Methods

        private async Task PopulateCategoriesDropdownAsync()
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            var categories = await _categoryService.GetAllCategoriesAsync();

            ViewBag.Categories = new SelectList(
                categories,
                "Id",
                lang == "ar" ? "NameAr" : "NameEn"
            );
        }

        #endregion
    }
}