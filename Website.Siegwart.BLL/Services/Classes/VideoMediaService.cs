using Website.Siegwart.BLL.Dtos.Admin.VideoMedia;
using Website.Siegwart.Core.Helpers;

namespace Website.Siegwart.BLL.Services.Classes
{
    public class VideoMediaService : IVideoMediaService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public VideoMediaService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<VideoMediaDto> CreateAsync(VideoMediaCreateDto dto, CancellationToken ct = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var videoId = YouTubeHelper.ExtractVideoId(dto.VideoUrl ?? string.Empty);
            if (videoId == null) throw new ArgumentException("Invalid YouTube URL or ID.", nameof(dto.VideoUrl));

            // Prevent duplicate VideoId (optional business rule)
            var exists = await _db.VideoMedias.AsNoTracking().AnyAsync(v => v.VideoId == videoId && !v.IsDeleted, ct);
            if (exists) throw new InvalidOperationException("This YouTube video is already added.");

            var entity = _mapper.Map<VideoMedia>(dto);
            entity.VideoId = videoId;
            entity.ThumbnailUrl = YouTubeHelper.GetThumbnail(videoId, preferMaxRes: false);
            entity.SourceUrl = dto.VideoUrl;

            _db.VideoMedias.Add(entity);
            await _db.SaveChangesAsync(ct);

            return _mapper.Map<VideoMediaDto>(entity);
        }

        public async Task<VideoMediaDto> UpdateAsync(VideoMediaUpdateDto dto, CancellationToken ct = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = await _db.VideoMedias.FirstOrDefaultAsync(v => v.Id == dto.Id && !v.IsDeleted, ct);
            if (entity == null) throw new KeyNotFoundException("Video not found.");

            var videoId = YouTubeHelper.ExtractVideoId(dto.VideoUrl ?? string.Empty);
            if (videoId == null) throw new ArgumentException("Invalid YouTube URL or ID.", nameof(dto.VideoUrl));

            // If changing the video id ensure no other record uses it
            if (!string.Equals(entity.VideoId, videoId, StringComparison.OrdinalIgnoreCase))
            {
                var other = await _db.VideoMedias.AsNoTracking().AnyAsync(v => v.VideoId == videoId && v.Id != entity.Id && !v.IsDeleted, ct);
                if (other) throw new InvalidOperationException("Another record uses the same YouTube video.");
            }

            // Map remaining editable fields
            _mapper.Map(dto, entity);

            entity.VideoId = videoId;
            entity.ThumbnailUrl = YouTubeHelper.GetThumbnail(videoId, preferMaxRes: false);
            entity.SourceUrl = dto.VideoUrl;

            _db.VideoMedias.Update(entity);
            await _db.SaveChangesAsync(ct);

            return _mapper.Map<VideoMediaDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.VideoMedias.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, ct);
            if (entity == null) return; // idempotent

            // Soft-delete will be applied by AppDbContext.UpdateAuditFields on SaveChanges
            _db.VideoMedias.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<VideoMediaDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.VideoMedias.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, ct);
            if (entity == null) return null;
            return _mapper.Map<VideoMediaDto>(entity);
        }

        public async Task<IEnumerable<VideoMediaListItemDto>> GetPublishedAsync(int take = 12, CancellationToken ct = default)
        {
            var items = await _db.VideoMedias
                .AsNoTracking()
                .Where(v => v.IsPublished && !v.IsDeleted)
                .OrderBy(v => v.SortOrder)
                .ThenByDescending(v => v.CreatedOn)
                .Take(take)
                .ToListAsync(ct);

            return _mapper.Map<IEnumerable<VideoMediaListItemDto>>(items);
        }

        public async Task<(int Total, VideoMediaListItemDto[] Items)> GetPagedAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var query = _db.VideoMedias.AsNoTracking().Where(v => !v.IsDeleted);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(v => v.SortOrder)
                .ThenByDescending(v => v.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var dtoItems = _mapper.Map<VideoMediaListItemDto[]>(items);
            return (total, dtoItems);
        }

        public async Task TogglePublishAsync(int id, bool publish, CancellationToken ct = default)
        {
            var entity = await _db.VideoMedias.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, ct);
            if (entity == null) throw new KeyNotFoundException("Video not found.");

            entity.IsPublished = publish;
            _db.VideoMedias.Update(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}