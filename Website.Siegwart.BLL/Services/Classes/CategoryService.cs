using AutoMapper;
using Microsoft.Extensions.Logging;
using Website.Siegwart.BLL.Dtos.Admin.CategoryDtos;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.BLL.Services.Classes
{
    /// <summary>
    /// Service for managing product categories
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> CreateCategoryAsync(CreateCategoryDto input)
        {
            _logger.LogInformation("Creating category: {NameEn}", input.NameEn);

            try
            {
                // Check for duplicate names
                var exists = await _unitOfWork.CategoryRepository
                    .AnyAsync(c => c.NameEn == input.NameEn || c.NameAr == input.NameAr);

                if (exists)
                {
                    _logger.LogWarning("Category already exists: {NameEn} / {NameAr}", input.NameEn, input.NameAr);
                    throw new InvalidOperationException("A category with this name already exists.");
                }

                var category = _mapper.Map<Category>(input);
                await _unitOfWork.CategoryRepository.AddAsync(category);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category created successfully: {Id} - {NameEn}", category.Id, category.NameEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {NameEn}", input.NameEn);
                throw;
            }
        }

        public async Task<int> UpdateCategoryAsync(UpdateCategoryDto input)
        {
            _logger.LogInformation("Updating category: {Id} - {NameEn}", input.Id, input.NameEn);

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(input.Id);

                if (category == null || category.IsDeleted)
                {
                    _logger.LogWarning("Category not found: {Id}", input.Id);
                    throw new KeyNotFoundException($"Category with ID {input.Id} not found.");
                }

                // Check for duplicate names (excluding current category)
                var exists = await _unitOfWork.CategoryRepository
                    .AnyAsync(c => (c.NameEn == input.NameEn || c.NameAr == input.NameAr) && c.Id != input.Id);

                if (exists)
                {
                    _logger.LogWarning("Category name already exists: {NameEn} / {NameAr}", input.NameEn, input.NameAr);
                    throw new InvalidOperationException("A category with this name already exists.");
                }

                _mapper.Map(input, category);
                await _unitOfWork.CategoryRepository.UpdateAsync(category);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category updated successfully: {Id} - {NameEn}", category.Id, category.NameEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category: {Id}", input.Id);
                throw;
            }
        }

        public async Task<int> DeleteCategoryAsync(int id)
        {
            _logger.LogInformation("Deleting category: {Id}", id);

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category == null || category.IsDeleted)
                {
                    _logger.LogWarning("Category not found: {Id}", id);
                    return 0;
                }

                // Check if category has products
                var hasProducts = await _unitOfWork.ProductRepository
                    .AnyAsync(p => p.CategoryId == id);

                if (hasProducts)
                {
                    _logger.LogWarning("Cannot delete category {Id} - has products", id);
                    throw new InvalidOperationException("Cannot delete category that has products.");
                }

                await _unitOfWork.CategoryRepository.RemoveAsync(category);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category deleted successfully: {Id}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category: {Id}", id);
                throw;
            }
        }

        public async Task<List<CategoryDetailsDto>> GetAllCategoriesAsync()
        {
            _logger.LogDebug("Getting all categories");

            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

                var result = _mapper.Map<List<CategoryDetailsDto>>(categories);

                _logger.LogDebug("Retrieved {Count} categories", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                throw;
            }
        }

        public async Task<CategoryDetailsDto?> GetCategoryByIdAsync(int id)
        {
            _logger.LogDebug("Getting category by ID: {Id}", id);

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category == null || category.IsDeleted)
                {
                    _logger.LogDebug("Category not found: {Id}", id);
                    return null;
                }

                var result = _mapper.Map<CategoryDetailsDto>(category);

                _logger.LogDebug("Category retrieved: {Id} - {NameEn}", id, result.NameEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by ID: {Id}", id);
                throw;
            }
        }

        public async Task<UpdateCategoryDto?> GetCategoryForEditAsync(int id)
        {
            _logger.LogDebug("Getting category for edit: {Id}", id);

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category == null || category.IsDeleted)
                {
                    _logger.LogDebug("Category not found for edit: {Id}", id);
                    return null;
                }

                var result = _mapper.Map<UpdateCategoryDto>(category);

                _logger.LogDebug("Category retrieved for edit: {Id} - {NameEn}", id, result.NameEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category for edit: {Id}", id);
                throw;
            }
        }
    }
}