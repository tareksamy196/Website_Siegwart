using Website.Siegwart.BLL.Dtos.Admin.CategoryDtos;

namespace Website.Siegwart.BLL.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing categories
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Create a new category
        /// </summary>
        Task<int> CreateCategoryAsync(CreateCategoryDto input);

        /// <summary>
        /// Update an existing category
        /// </summary>
        Task<int> UpdateCategoryAsync(UpdateCategoryDto input);

        /// <summary>
        /// Delete a category (soft delete)
        /// </summary>
        Task<int> DeleteCategoryAsync(int id);

        /// <summary>
        /// Get all categories
        /// </summary>
        Task<List<CategoryDetailsDto>> GetAllCategoriesAsync();

        /// <summary>
        /// Get category by ID
        /// </summary>
        Task<CategoryDetailsDto?> GetCategoryByIdAsync(int id);

        /// <summary>
        /// Get category for edit form
        /// </summary>
        Task<UpdateCategoryDto?> GetCategoryForEditAsync(int id);
    }
}