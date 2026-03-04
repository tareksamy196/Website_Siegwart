using Website.Siegwart.DAL.Enums;

namespace Website.Siegwart.BLL.Dtos.Admin.TeamMember;

/// <summary>
/// DTO for Team Members List/Grid (Simplified)
/// </summary>
public class AdminTeamMemberDto
{
    public int Id { get; set; }

    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;

    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;

    public TeamCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string? Email { get; set; }
    public string? Phone { get; set; }

    public int Order { get; set; }
}