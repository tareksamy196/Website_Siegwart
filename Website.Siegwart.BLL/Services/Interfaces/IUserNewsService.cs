namespace Website.Siegwart.BLL.Services.Interfaces
{
    public interface IUserNewsService
    {
        Task<List<NewsViewDto>> GetPublishedNewsAsync();
        Task<NewsViewDto?> GetNewsByIdAsync(int id);
    }
}