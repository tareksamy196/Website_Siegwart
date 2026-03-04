using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Website.Siegwart.BLL.Services.Classes
{
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;

        public AuditService(ILogger<AuditService> logger)
        {
            _logger = logger;
        }

        public Task LogAsync(string performedByUserId, string action, string targetUserId, string? details = null)
        {
            // DO NOT log sensitive values (passwords, reset tokens, etc.)
            _logger.LogInformation("AUDIT: Actor={Actor}, Action={Action}, Target={Target}, Details={Details}",
                performedByUserId ?? "unknown", action, targetUserId ?? "unknown", details ?? string.Empty);
            return Task.CompletedTask;
        }
    }
}