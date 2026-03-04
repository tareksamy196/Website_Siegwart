using Microsoft.EntityFrameworkCore;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.DAL.Repositories.Classes
{
    // Base for all SEO entities — adds GetBySlugAsync + SlugExistsAsync
    public class SeoRepository<T> : GenericRepository<T>, ISeoRepository<T>
        where T : SeoEntity
    {
        public SeoRepository(AppDbContext context) : base(context) { }

        public async Task<T?> GetBySlugAsync(string slug)
            => await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Slug == slug);

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
            => await _dbSet.AnyAsync(e =>
                e.Slug == slug &&
                (excludeId == null || e.Id != excludeId));
    }
}