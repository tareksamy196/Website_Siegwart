using System.Threading.Tasks;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.PL.Services
{
    public interface IContactService
    {
        Task<ContactMessage> SaveAsync(UserContactFormDto vm, string? ip = null, string? userAgent = null);
        Task<ContactMessage?> GetByIdAsync(int id);
        Task MarkAsReadAsync(int id);
    }
}