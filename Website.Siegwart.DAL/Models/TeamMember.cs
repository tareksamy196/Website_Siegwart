using System.ComponentModel.DataAnnotations;
using Website.Siegwart.DAL.Enums;

namespace Website.Siegwart.DAL.Models
{
    public class TeamMember : SeoEntity
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public TeamCategory Category { get; set; }
        public string? ImageUrl { get; set; }
        public int Order { get; set; } = 100;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Url]
        public string? LinkedInUrl { get; set; }

        public string? BioEn { get; set; }
        public string? BioAr { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool IsActive { get; set; } = true;
    }
}