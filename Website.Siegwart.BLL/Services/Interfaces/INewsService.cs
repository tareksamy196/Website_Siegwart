using Website.Siegwart.BLL.Dtos.Admin.NewsDtos;

namespace Website.Siegwart.BLL.Services.Interfaces;

public interface INewsService
{
    Task<int> CreateNewsAsync(CreateNewsDto input);
    Task<int> UpdateNewsAsync(UpdateNewsDto input);
    Task<int> DeleteNewsAsync(int id);
    Task<List<NewsListDto>> GetAllNewsAsync();
    Task<NewsDetailsDto?> GetNewsByIdAsync(int id);
    Task<UpdateNewsDto?> GetNewsForEditAsync(int id);
}