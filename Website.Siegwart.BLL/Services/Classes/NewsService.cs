using AutoMapper;
using Microsoft.Extensions.Logging;
using Website.Siegwart.BLL.Dtos.Admin.NewsDtos;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.BLL.Services.Classes
{
    /// <summary>
    /// Service for managing news articles
    /// </summary>
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<NewsService> _logger;

        public NewsService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAttachmentService attachmentService,
            ILogger<NewsService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _logger = logger;
        }

        public async Task<int> CreateNewsAsync(CreateNewsDto input)
        {
            _logger.LogInformation("Creating news: {TitleEn}", input.TitleEn);

            try
            {
                string? savedImagePath = null;
                if (input.ImageFile != null && input.ImageFile.Length > 0)
                {
                    savedImagePath = await _attachmentService.UploadAsync(input.ImageFile, "uploads/news");
                }

                var news = _mapper.Map<News>(input);
                news.ImageUrl = savedImagePath;

                // Generate SEO fields
                news.Slug = GenerateSlug(input.TitleEn);
                news.SeoTitleEn = string.IsNullOrWhiteSpace(input.SeoTitleEn) ? input.TitleEn : input.SeoTitleEn;
                news.SeoTitleAr = string.IsNullOrWhiteSpace(input.SeoTitleAr) ? input.TitleAr : input.SeoTitleAr;
                news.SeoDescriptionEn = string.IsNullOrWhiteSpace(input.SeoDescriptionEn)
                    ? BuildSeoDescription(input.ContentEn, input.TitleEn)
                    : input.SeoDescriptionEn;
                news.SeoDescriptionAr = string.IsNullOrWhiteSpace(input.SeoDescriptionAr)
                    ? BuildSeoDescription(input.ContentAr, input.TitleAr)
                    : input.SeoDescriptionAr;

                await _unitOfWork.NewsRepository.AddAsync(news);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("News created successfully: {Id} - {TitleEn}", news.Id, news.TitleEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news: {TitleEn}", input.TitleEn);
                throw;
            }
        }

        public async Task<List<NewsListDto>> GetAllNewsAsync()
        {
            _logger.LogDebug("Getting all news");

            try
            {
                var newsList = (await _unitOfWork.NewsRepository.GetAllAsync())
                    .Where(n => !n.IsDeleted)
                    .OrderByDescending(n => n.PublishedOn)
                    .ToList();

                var result = _mapper.Map<List<NewsListDto>>(newsList);

                _logger.LogDebug("Retrieved {Count} news articles", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all news");
                throw;
            }
        }

        public async Task<NewsDetailsDto?> GetNewsByIdAsync(int id)
        {
            _logger.LogDebug("Getting news by ID: {Id}", id);

            try
            {
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(id);
                if (news == null || news.IsDeleted)
                {
                    _logger.LogDebug("News not found: {Id}", id);
                    return null;
                }

                var result = _mapper.Map<NewsDetailsDto>(news);

                _logger.LogDebug("News retrieved: {Id} - {TitleEn}", id, result.TitleEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by ID: {Id}", id);
                throw;
            }
        }

        public async Task<UpdateNewsDto?> GetNewsForEditAsync(int id)
        {
            _logger.LogDebug("Getting news for edit: {Id}", id);

            try
            {
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(id);
                if (news == null || news.IsDeleted)
                {
                    _logger.LogDebug("News not found for edit: {Id}", id);
                    return null;
                }

                var result = _mapper.Map<UpdateNewsDto>(news);

                _logger.LogDebug("News retrieved for edit: {Id} - {TitleEn}", id, result.TitleEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news for edit: {Id}", id);
                throw;
            }
        }

        public async Task<int> UpdateNewsAsync(UpdateNewsDto input)
        {
            _logger.LogInformation("Updating news: {Id} - {TitleEn}", input.Id, input.TitleEn);

            try
            {
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(input.Id);
                if (news == null)
                {
                    _logger.LogWarning("News not found: {Id}", input.Id);
                    throw new KeyNotFoundException($"News with ID {input.Id} not found.");
                }

                _mapper.Map(input, news);

                news.Slug = GenerateSlug(input.TitleEn);
                news.SeoTitleEn = string.IsNullOrWhiteSpace(input.SeoTitleEn) ? input.TitleEn : input.SeoTitleEn;
                news.SeoTitleAr = string.IsNullOrWhiteSpace(input.SeoTitleAr) ? input.TitleAr : input.SeoTitleAr;
                news.SeoDescriptionEn = string.IsNullOrWhiteSpace(input.SeoDescriptionEn)
                    ? BuildSeoDescription(input.ContentEn, input.TitleEn)
                    : input.SeoDescriptionEn;
                news.SeoDescriptionAr = string.IsNullOrWhiteSpace(input.SeoDescriptionAr)
                    ? BuildSeoDescription(input.ContentAr, input.TitleAr)
                    : input.SeoDescriptionAr;

                if (input.ImageFile != null && input.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(news.ImageUrl) &&
                        !news.ImageUrl.Contains("no-image", System.StringComparison.OrdinalIgnoreCase))
                    {
                        _attachmentService.Delete(news.ImageUrl);
                    }
                    string? savedImagePath = await _attachmentService.UploadAsync(input.ImageFile, "uploads/news");
                    news.ImageUrl = savedImagePath;
                }

                await _unitOfWork.NewsRepository.UpdateAsync(news);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("News updated successfully: {Id} - {TitleEn}", news.Id, news.TitleEn);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news: {Id}", input.Id);
                throw;
            }
        }

        public async Task<int> DeleteNewsAsync(int id)
        {
            _logger.LogInformation("Deleting news: {Id}", id);

            try
            {
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(id);
                if (news == null)
                {
                    _logger.LogWarning("News not found: {Id}", id);
                    throw new KeyNotFoundException($"News with ID {id} not found.");
                }

                if (!string.IsNullOrEmpty(news.ImageUrl) &&
                    !news.ImageUrl.Contains("no-image", System.StringComparison.OrdinalIgnoreCase))
                {
                    _attachmentService.Delete(news.ImageUrl);
                }

                await _unitOfWork.NewsRepository.RemoveAsync(news);
                var result = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("News deleted successfully: {Id}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news: {Id}", id);
                throw;
            }
        }

        #region Helper Methods

        private string GenerateSlug(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var slug = text.Trim().ToLowerInvariant();
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\u0600-\u06FF\s-]", "");
            slug = slug.Replace(" ", "-").Replace("--", "-").Trim('-');
            return slug;
        }

        private string BuildSeoDescription(string? value, string? fallback)
        {
            var src = !string.IsNullOrWhiteSpace(value) ? value : fallback;
            if (string.IsNullOrWhiteSpace(src)) return string.Empty;
            return src.Length > 290 ? src.Substring(0, 290) + "..." : src;
        }

        #endregion
    }
}