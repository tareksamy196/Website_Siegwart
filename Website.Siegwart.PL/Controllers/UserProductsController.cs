using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
   
    [Route("products")]
    public class UserProductsController : Controller
    {
        private readonly IUserProductService _productService;
        private readonly ILogger<UserProductsController> _logger;

        public UserProductsController(
            IUserProductService productService,
            ILogger<UserProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Display all active products grouped by category
        /// </summary>
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogDebug("Loading products page");

                var products = await _productService.GetActiveProductsAsync();

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products page");
                return View(new List<BLL.Dtos.User.UserProductDto>());
            }
        }

        /// <summary>
        /// Display products filtered by category
        /// </summary>
        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> ByCategory(int categoryId)
        {
            try
            {
                _logger.LogDebug("Loading products for category: {CategoryId}", categoryId);

                var products = await _productService.GetProductsByCategoryAsync(categoryId);

                return View("Index", products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products for category: {CategoryId}", categoryId);
                return View("Index", new List<BLL.Dtos.User.UserProductDto>());
            }
        }

        /// <summary>
        /// Display product details
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogDebug("Loading product details: {Id}", id);

                var product = await _productService.GetProductDetailsAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product not found: {Id}", id);
                    return NotFound();
                }

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product details: {Id}", id);
                return NotFound();
            }
        }
    }
}