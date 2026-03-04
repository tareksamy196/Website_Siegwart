using Microsoft.EntityFrameworkCore;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.DAL.Repositories.Classes
{
    public class VideoMediaRepository : SeoRepository<VideoMedia>, IVideoMediaRepository
    {
        public VideoMediaRepository(AppDbContext context) : base(context) { }

        public async Task<VideoMedia?> GetByVideoIdAsync(string videoId)
            => await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VideoId == videoId);

        public async Task<List<VideoMedia>> GetPublishedAsync(int take = 12)
            => await _dbSet
                .AsNoTracking()
                .Where(v => v.IsPublished)
                .OrderBy(v => v.SortOrder)
                .ThenByDescending(v => v.CreatedOn)
                .Take(take)
                .ToListAsync();

        public async Task<(int Total, List<VideoMedia> Items)> GetPagedAsync(
            int page = 1,
            int pageSize = 20)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(v => v.IsPublished)
                .OrderBy(v => v.SortOrder)
                .ThenByDescending(v => v.CreatedOn);

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, items);
        }
    }
}