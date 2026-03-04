using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Repositories.Interfaces
{
    public interface ISeoRepository<T> : IGenericRepository<T> where T : SeoEntity
    {
        Task<T?> GetBySlugAsync(string slug);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    }
}