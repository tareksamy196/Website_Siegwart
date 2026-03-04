using AutoMapper;
using Microsoft.Extensions.Logging;
using Website.Siegwart.BLL.Dtos.Admin.ProductDtos;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.BLL.Services.Classes
{
    /// <summary>
    /// Service for managing products
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAttachmentService attachmentService,
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _logger = logger;
        }

        public async Task<int> CreateProductAsync(CreateProductDto input)
        {
            _logger.LogInformation("Creating product: {TitleEn}", input.TitleEn);

            try
            {
                // Upload image if provided
                string? savedImagePath = null;
                if (input.ImageFile != null && input.ImageFile.Length > 0)
                {
                    savedImagePath = await _attachmentService.UploadAsync(input.ImageFile, "uploads/products");
                }

                var product = _mapper.Map<Product>(input);
                product.ImageUrl = savedImagePath;

                // Generate SEO fields
                product.Slug = GenerateSlug(input.TitleEn);
                product.SeoTitleEn = string.IsNullOrWhiteSpace(input.SeoTitleEn) ? input.TitleEn : input.SeoTitleEn;
                product.SeoTitleAr = string.IsNullOrWhiteSpace(input.SeoTitleAr) ? input.TitleAr : input.SeoTitleAr;
                product.SeoDescriptionEn = string.IsNullOrWhiteSpace(input.SeoDescriptionEn)
                    ? BuildSeoDescription(input.DescriptionEn, input.TitleEn)
                    : input.SeoDescriptionEn;
                product.SeoDescriptionAr = string.IsNullOrWhiteSpace(input.SeoDescriptionAr)
                    ? BuildSeoDescription(input.DescriptionAr, input.TitleAr)
                    : input.SeoDescriptionAr;

                await _unitOfWork.ProductRepository.AddAsync(product);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product created successfully: {Id} - {TitleEn}", product.Id, product.TitleEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {TitleEn}", input.TitleEn);
                throw;
            }
        }

        public async Task<int> UpdateProductAsync(UpdateProductDto input)
        {
            _logger.LogInformation("Updating product: {Id} - {TitleEn}", input.Id, input.TitleEn);

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(input.Id);

                if (product == null || product.IsDeleted)
                {
                    _logger.LogWarning("Product not found: {Id}", input.Id);
                    throw new KeyNotFoundException($"Product with ID {input.Id} not found.");
                }

                _mapper.Map(input, product);

                // Update SEO fields
                product.Slug = GenerateSlug(input.TitleEn);
                product.SeoTitleEn = string.IsNullOrWhiteSpace(input.SeoTitleEn) ? input.TitleEn : input.SeoTitleEn;
                product.SeoTitleAr = string.IsNullOrWhiteSpace(input.SeoTitleAr) ? input.TitleAr : input.SeoTitleAr;
                product.SeoDescriptionEn = string.IsNullOrWhiteSpace(input.SeoDescriptionEn)
                    ? BuildSeoDescription(input.DescriptionEn, input.TitleEn)
                    : input.SeoDescriptionEn;
                product.SeoDescriptionAr = string.IsNullOrWhiteSpace(input.SeoDescriptionAr)
                    ? BuildSeoDescription(input.DescriptionAr, input.TitleAr)
                    : input.SeoDescriptionAr;

                // Handle image update
                if (input.ImageFile != null && input.ImageFile.Length > 0)
                {
                    // Delete old image
                    if (!string.IsNullOrEmpty(product.ImageUrl) &&
                        !product.ImageUrl.Contains("no-image", StringComparison.OrdinalIgnoreCase))
                    {
                        _attachmentService.Delete(product.ImageUrl);
                    }

                    // Upload new image
                    string? savedImagePath = await _attachmentService.UploadAsync(input.ImageFile, "uploads/products");
                    product.ImageUrl = savedImagePath;
                }

                await _unitOfWork.ProductRepository.UpdateAsync(product);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product updated successfully: {Id} - {TitleEn}", product.Id, product.TitleEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {Id}", input.Id);
                throw;
            }
        }

        public async Task<int> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Deleting product: {Id}", id);

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

                if (product == null || product.IsDeleted)
                {
                    _logger.LogWarning("Product not found: {Id}", id);
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                // Delete image
                if (!string.IsNullOrEmpty(product.ImageUrl) &&
                    !product.ImageUrl.Contains("no-image", StringComparison.OrdinalIgnoreCase))
                {
                    _attachmentService.Delete(product.ImageUrl);
                }

                await _unitOfWork.ProductRepository.RemoveAsync(product);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product deleted successfully: {Id}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product: {Id}", id);
                throw;
            }
        }

        public async Task<List<ProductListDto>> GetAllProductsAsync()
        {
            _logger.LogDebug("Getting all products");

            try
            {
                var products = await _unitOfWork.ProductRepository.GetAllAsync();

                var result = _mapper.Map<List<ProductListDto>>(products);

                _logger.LogDebug("Retrieved {Count} products", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<ProductDetailsDto?> GetProductByIdAsync(int id)
        {
            _logger.LogDebug("Getting product by ID: {Id}", id);

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

                if (product == null || product.IsDeleted)
                {
                    _logger.LogDebug("Product not found: {Id}", id);
                    return null;
                }

                var result = _mapper.Map<ProductDetailsDto>(product);

                _logger.LogDebug("Product retrieved: {Id} - {TitleEn}", id, result.TitleEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID: {Id}", id);
                throw;
            }
        }

        public async Task<UpdateProductDto?> GetProductForEditAsync(int id)
        {
            _logger.LogDebug("Getting product for edit: {Id}", id);

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

                if (product == null || product.IsDeleted)
                {
                    _logger.LogDebug("Product not found for edit: {Id}", id);
                    return null;
                }

                var result = _mapper.Map<UpdateProductDto>(product);

                _logger.LogDebug("Product retrieved for edit: {Id} - {TitleEn}", id, result.TitleEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product for edit: {Id}", id);
                throw;
            }
        }

        #region Helper Methods

        private string GenerateSlug(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var slug = text.Trim().ToLowerInvariant();
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\u0600-\u06FF\s-]", "");
            slug = slug.Replace(" ", "-").Replace("--", "-").Trim('-');

            return slug;
        }

        private string BuildSeoDescription(string? value, string? fallback)
        {
            var src = !string.IsNullOrWhiteSpace(value) ? value : fallback;
            if (string.IsNullOrWhiteSpace(src)) return string.Empty;

            return src.Length > 290 ? src.Substring(0, 290) + "..." : src;
        }

        #endregion
    }
}