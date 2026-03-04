using System.Threading.Tasks;

namespace Website.Siegwart.BLL.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string performedByUserId, string action, string targetUserId, string? details = null);
    }
}