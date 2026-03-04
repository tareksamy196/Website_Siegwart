using Microsoft.EntityFrameworkCore;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Enums;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.DAL.Repositories.Classes
{
    public class TeamMemberRepository : SeoRepository<TeamMember>, ITeamMemberRepository
    {
        public TeamMemberRepository(AppDbContext context) : base(context) { }

        public async Task<List<TeamMember>> GetActiveAsync()
            => await _dbSet
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.Order)
                .ToListAsync();

        public async Task<List<TeamMember>> GetByCategoryAsync(TeamCategory category)
            => await _dbSet
                .AsNoTracking()
                .Where(t => t.IsActive && t.Category == category)
                .OrderBy(t => t.Order)
                .ToListAsync();
    }
}