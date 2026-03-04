using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Repositories.Interfaces
{
    public interface INewsRepository : ISeoRepository<News>
    {
        Task<List<News>> GetPublishedAsync(int take = 10);
        Task<(int Total, List<News> Items)> GetPagedAsync(
            int page = 1,
            int pageSize = 10);
    }
}