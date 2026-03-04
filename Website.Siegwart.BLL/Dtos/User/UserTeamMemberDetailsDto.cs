using Website.Siegwart.DAL.Enums;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.BLL.Dtos.User;

/// <summary>
/// DTO for Team Member Details - Public Frontend
/// </summary>
public class TeamMemberViewDto
{
    public int Id { get; set; }

    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;

    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;

    public TeamCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    // Contact Information
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }

    // Biography
    public string? BioEn { get; set; }
    public string? BioAr { get; set; }

    public int? YearsOfExperience { get; set; }

    public int Order { get; set; }
}