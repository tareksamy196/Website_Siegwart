using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.BLL.Dtos.User;

namespace Website.Siegwart.BLL.Services.Classes
{
    /// <summary>
    /// Service for public/user-facing news operations
    /// </summary>
    public class UserNewsService : IUserNewsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserNewsService> _logger;

        public UserNewsService(
            AppDbContext context,
            IMapper mapper,
            ILogger<UserNewsService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all published news
        /// </summary>
        public async Task<List<NewsViewDto>> GetPublishedNewsAsync()
        {
            try
            {
                _logger.LogDebug("Fetching published news");

                var news = await _context.News
                    .AsNoTracking()
                    .Where(n => n.IsPublished && !n.IsDeleted)
                    .OrderByDescending(n => n.PublishedOn)
                    .ToListAsync();

                var result = _mapper.Map<List<NewsViewDto>>(news);

                _logger.LogInformation("Retrieved {Count} published news articles", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving published news");
                throw;
            }
        }

        /// <summary>
        /// Get news details by ID
        /// </summary>
        public async Task<NewsViewDto?> GetNewsByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching news details for ID: {Id}", id);

                var news = await _context.News
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id && n.IsPublished && !n.IsDeleted);

                if (news == null)
                {
                    _logger.LogWarning("News with ID {Id} not found or not published", id);
                    return null;
                }

                var result = _mapper.Map<NewsViewDto>(news);

                _logger.LogInformation("Retrieved news details for ID {Id}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving news details for ID {Id}", id);
                throw;
            }
        }
    }
}