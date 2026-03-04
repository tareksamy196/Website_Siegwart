using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Repositories.Interfaces
{
    public interface ICategoryRepository : ISeoRepository<Category>
    {
        Task<List<Category>> GetActiveWithProductsAsync();
    }
}