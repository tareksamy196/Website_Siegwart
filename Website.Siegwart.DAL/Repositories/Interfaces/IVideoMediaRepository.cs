using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Repositories.Interfaces
{
    public interface IVideoMediaRepository : ISeoRepository<VideoMedia>
    {
        Task<VideoMedia?> GetByVideoIdAsync(string videoId);
        Task<List<VideoMedia>> GetPublishedAsync(int take = 12);
        Task<(int Total, List<VideoMedia> Items)> GetPagedAsync(
            int page = 1,
            int pageSize = 20);
    }
}