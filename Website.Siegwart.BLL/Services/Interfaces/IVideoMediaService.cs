using Website.Siegwart.BLL.Dtos.Admin.VideoMedia;

namespace Website.Siegwart.BLL.Services.Interfaces
{
    public interface IVideoMediaService
    {
        Task<VideoMediaDto> CreateAsync(VideoMediaCreateDto dto, CancellationToken ct = default);
        Task<VideoMediaDto> UpdateAsync(VideoMediaUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default); 
        Task<VideoMediaDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<VideoMediaListItemDto>> GetPublishedAsync(int take = 12, CancellationToken ct = default);
        Task<(int Total, VideoMediaListItemDto[] Items)> GetPagedAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task TogglePublishAsync(int id, bool publish, CancellationToken ct = default);
    }
}