using Microsoft.EntityFrameworkCore;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.DAL.Repositories.Classes
{
    public class CategoryRepository : SeoRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<List<Category>> GetActiveWithProductsAsync()
            => await _dbSet
                .AsNoTracking()
                .Where(c => c.IsActive)
                .Include(c => c.Products.Where(p => p.IsActive))
                .OrderBy(c => c.NameEn)
                .ToListAsync();
    }
}