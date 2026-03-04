using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Repositories.Interfaces
{
    public interface IProductRepository : ISeoRepository<Product>
    {
        Task<List<Product>> GetByCategoryAsync(int categoryId);
        Task<List<Product>> GetActiveAsync();
        Task<(int Total, List<Product> Items)> GetPagedAsync(
            int page = 1,
            int pageSize = 12,
            int? categoryId = null);
    }
}