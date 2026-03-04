using Website.Siegwart.BLL.Dtos.Admin.TeamMember;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.BLL.Services.Interfaces;

public interface ITeamMemberService
{
    Task<int> CreateAsync(CreateTeamMemberDto input);
    Task<int> UpdateAsync(UpdateTeamMemberDto input);
    Task<int> DeleteAsync(int id);
    Task<List<AdminTeamMemberDto>> GetAllAsync(); 
    Task<UpdateTeamMemberDto?> GetByIdAsync(int id); 
    Task<TeamMember?> GetEntityByIdAsync(int id); 
}
/// <summary>
/// Service for admin team member operations
/// </summary>