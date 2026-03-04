using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Enums;

namespace Website.Siegwart.DAL.Repositories.Interfaces
{
    public interface ITeamMemberRepository : ISeoRepository<TeamMember>
    {
        Task<List<TeamMember>> GetActiveAsync();
        Task<List<TeamMember>> GetByCategoryAsync(TeamCategory category);
    }
}