using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.BLL.Dtos.User;

namespace Website.Siegwart.BLL.Services.Classes
{
    /// <summary>
    /// Service for public/user-facing product operations
    /// </summary>
    public class UserProductService : IUserProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProductService> _logger;

        public UserProductService(
            AppDbContext context,
            IMapper mapper,
            ILogger<UserProductService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all active products
        /// </summary>
        public async Task<List<UserProductDto>> GetActiveProductsAsync()
        {
            try
            {
                _logger.LogDebug("Fetching active products");

                var products = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && !p.IsDeleted)
                    .OrderByDescending(p => p.CreatedOn)
                    .ToListAsync();

                var result = _mapper.Map<List<UserProductDto>>(products);

                _logger.LogInformation("Retrieved {Count} active products", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active products");
                throw;
            }
        }

        /// <summary>
        /// Get product details by ID
        /// </summary>
        public async Task<UserProductDto?> GetProductDetailsAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching product details for ID: {Id}", id);

                var product = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive && !p.IsDeleted);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found or inactive", id);
                    return null;
                }

                var result = _mapper.Map<UserProductDto>(product);

                _logger.LogInformation("Retrieved product details for ID {Id}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product details for ID {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        public async Task<List<UserProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                _logger.LogDebug("Fetching products for category: {CategoryId}", categoryId);

                var products = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && !p.IsDeleted && p.CategoryId == categoryId)
                    .OrderByDescending(p => p.CreatedOn)
                    .ToListAsync();

                var result = _mapper.Map<List<UserProductDto>>(products);

                _logger.LogInformation("Retrieved {Count} products for category {CategoryId}", result.Count, categoryId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for category {CategoryId}", categoryId);
                throw;
            }
        }
    }
}