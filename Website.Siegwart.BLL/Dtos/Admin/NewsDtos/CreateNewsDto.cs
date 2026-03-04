using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.BLL.Dtos.Admin.NewsDtos
{
    public class CreateNewsDto
    {
        [Required(ErrorMessage = "Title in English is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        [Display(Name = "Title (English)")]
        public string TitleEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title in Arabic is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        [Display(Name = "Title (Arabic)")]
        public string TitleAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content in English is required")]
        [Display(Name = "Content (English)")]
        public string ContentEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content in Arabic is required")]
        [Display(Name = "Content (Arabic)")]
        public string ContentAr { get; set; } = string.Empty;

        [Display(Name = "Featured Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Publish Date")]
        public DateTime PublishedOn { get; set; } = DateTime.Now;

        [Display(Name = "Published")]
        public bool IsPublished { get; set; } = false;

        // SEO Fields
        [StringLength(220)]
        [Display(Name = "SEO Slug")]
        public string? Slug { get; set; }

        [StringLength(180)]
        [Display(Name = "SEO Title (English)")]
        public string? SeoTitleEn { get; set; }

        [StringLength(180)]
        [Display(Name = "SEO Title (Arabic)")]
        public string? SeoTitleAr { get; set; }

        [StringLength(300)]
        [Display(Name = "SEO Description (English)")]
        public string? SeoDescriptionEn { get; set; }

        [StringLength(300)]
        [Display(Name = "SEO Description (Arabic)")]
        public string? SeoDescriptionAr { get; set; }
    }
}