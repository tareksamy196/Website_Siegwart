using Website.Siegwart.BLL.Dtos.User;

namespace Website.Siegwart.BLL.Services.Interfaces;

/// <summary>
/// Service for public/user-facing team member operations
/// </summary>
public interface IUserTeamMemberService
{
    /// <summary>
    /// Get all active team members (for list/grid)
    /// </summary>
    Task<List<UserTeamMemberListDto>> GetActiveTeamMembersAsync();

    /// <summary>
    /// Get team member details by ID (for single view)
    /// </summary>
    Task<TeamMemberViewDto?> GetTeamMemberDetailsAsync(int id);

    /// <summary>
    /// Get team members grouped by category
    /// </summary>
    Task<Dictionary<string, List<UserTeamMemberListDto>>> GetTeamMembersByCategoryAsync();

    /// <summary>
    /// Get count of active team members
    /// </summary>
    Task<int> GetActiveTeamMembersCountAsync();
}