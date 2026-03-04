using Microsoft.EntityFrameworkCore;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.DAL.Repositories.Classes
{
    public class NewsRepository : SeoRepository<News>, INewsRepository
    {
        public NewsRepository(AppDbContext context) : base(context) { }

        public async Task<List<News>> GetPublishedAsync(int take = 10)
            => await _dbSet
                .AsNoTracking()
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishedOn)
                .Take(take)
                .ToListAsync();

        public async Task<(int Total, List<News> Items)> GetPagedAsync(
            int page = 1,
            int pageSize = 10)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishedOn);

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, items);
        }
    }
}