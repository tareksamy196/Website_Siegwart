using Website.Siegwart.DAL.Enums;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.BLL.Dtos.User;

/// <summary>
/// DTO for Team Members List - Public Frontend
/// </summary>
public class UserTeamMemberListDto
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