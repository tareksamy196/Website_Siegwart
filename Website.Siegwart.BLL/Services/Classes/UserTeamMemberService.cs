using Microsoft.Extensions.Logging;


namespace Website.Siegwart.BLL.Services.Classes;

public class UserTeamMemberService : IUserTeamMemberService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserTeamMemberService> _logger;

    public UserTeamMemberService(
        AppDbContext context,
        IMapper mapper,
        ILogger<UserTeamMemberService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all active team members for public display
    /// </summary>
    public async Task<List<UserTeamMemberListDto>> GetActiveTeamMembersAsync()
    {
        try
        {
            var teamMembers = await _context.TeamMembers
                .AsNoTracking()
                .Where(t => t.IsActive && !t.IsDeleted)
                .OrderBy(t => t.Order)
                .ThenBy(t => t.NameEn)
                .ToListAsync();

            // ✅ Maps to User.TeamMemberListDto
            var result = _mapper.Map<List<UserTeamMemberListDto>>(teamMembers);

            _logger.LogInformation("Retrieved {Count} active team members for public view", result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active team members");
            throw;
        }
    }

    /// <summary>
    /// Get team member details by ID for public display
    /// </summary>
    public async Task<TeamMemberViewDto?> GetTeamMemberDetailsAsync(int id)
    {
        try
        {
            var teamMember = await _context.TeamMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive && !t.IsDeleted);

            if (teamMember == null)
            {
                _logger.LogWarning("Team member with ID {Id} not found or inactive", id);
                return null;
            }

            // ✅ Maps to User.TeamMemberViewDto
            var result = _mapper.Map<TeamMemberViewDto>(teamMember);

            _logger.LogInformation("Retrieved team member details for ID {Id}", id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team member details for ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Get team members grouped by category
    /// </summary>
    public async Task<Dictionary<string, List<UserTeamMemberListDto>>> GetTeamMembersByCategoryAsync()
    {
        try
        {
            var teamMembers = await _context.TeamMembers
                .AsNoTracking()
                .Where(t => t.IsActive && !t.IsDeleted)
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Order)
                .ToListAsync();

            var dtos = _mapper.Map<List<UserTeamMemberListDto>>(teamMembers);

            var result = dtos
                .GroupBy(t => t.CategoryName)
                .ToDictionary(g => g.Key, g => g.ToList());

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team members by category");
            throw;
        }
    }

    /// <summary>
    /// Get count of active team members
    /// </summary>
    public async Task<int> GetActiveTeamMembersCountAsync()
    {
        try
        {
            return await _context.TeamMembers
                .AsNoTracking()
                .CountAsync(t => t.IsActive && !t.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting active team members");
            throw;
        }
    }
}