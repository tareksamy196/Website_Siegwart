using Microsoft.EntityFrameworkCore;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.DAL.Repositories.Classes
{
    public class ProductRepository : SeoRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<List<Product>> GetActiveAsync()
            => await _dbSet
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

        public async Task<List<Product>> GetByCategoryAsync(int categoryId)
            => await _dbSet
                .AsNoTracking()
                .Where(p => p.IsActive && p.CategoryId == categoryId)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

        public async Task<(int Total, List<Product> Items)> GetPagedAsync(
            int page = 1,
            int pageSize = 12,
            int? categoryId = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(p => p.IsActive);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.SortOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, items);
        }
    }
}