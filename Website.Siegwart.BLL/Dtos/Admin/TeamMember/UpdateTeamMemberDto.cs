using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Website.Siegwart.DAL.Enums;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.BLL.Dtos.Admin.TeamMember
{
    public class UpdateTeamMemberDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name in English is required")]
        [StringLength(120, ErrorMessage = "Name must not exceed 120 characters")]
        [Display(Name = "Name (English)")]
        public string NameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name in Arabic is required")]
        [StringLength(120, ErrorMessage = "Name must not exceed 120 characters")]
        [Display(Name = "Name (Arabic)")]
        public string NameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title in English is required")]
        [StringLength(90, ErrorMessage = "Title must not exceed 90 characters")]
        [Display(Name = "Job Title (English)")]
        public string TitleEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title in Arabic is required")]
        [StringLength(90, ErrorMessage = "Title must not exceed 90 characters")]
        [Display(Name = "Job Title (Arabic)")]
        public string TitleAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public TeamCategory Category { get; set; }

        [StringLength(300)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Change Profile Photo")]
        public IFormFile? ImageFile { get; set; }

        [Range(1, 9999, ErrorMessage = "Order must be between 1 and 9999")]
        [Display(Name = "Display Order")]
        public int Order { get; set; } = 1;

        // Contact Information
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Url(ErrorMessage = "Invalid LinkedIn URL")]
        [StringLength(200)]
        [Display(Name = "LinkedIn Profile")]
        public string? LinkedInUrl { get; set; }

        // Biography
        [StringLength(1000, ErrorMessage = "Biography must not exceed 1000 characters")]
        [Display(Name = "Biography (English)")]
        public string? BioEn { get; set; }

        [StringLength(1000, ErrorMessage = "Biography must not exceed 1000 characters")]
        [Display(Name = "Biography (Arabic)")]
        public string? BioAr { get; set; }

        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
        [Display(Name = "Years of Experience")]
        public int? YearsOfExperience { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}