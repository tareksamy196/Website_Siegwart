namespace Website.Siegwart.BLL.Services.Interfaces
{
    public interface IUserProductService
    {
        Task<List<UserProductDto>> GetActiveProductsAsync();
        Task<UserProductDto?> GetProductDetailsAsync(int id);
        Task<List<UserProductDto>> GetProductsByCategoryAsync(int categoryId);
    }
}