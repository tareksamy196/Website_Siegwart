using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.BLL.Dtos.Admin.ProductDtos
{
    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title in English is required")]
        [StringLength(150, ErrorMessage = "Title must not exceed 150 characters")]
        [Display(Name = "Title (English)")]
        public string TitleEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title in Arabic is required")]
        [StringLength(150, ErrorMessage = "Title must not exceed 150 characters")]
        [Display(Name = "Title (Arabic)")]
        public string TitleAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description in English is required")]
        [Display(Name = "Description (English)")]
        public string DescriptionEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description in Arabic is required")]
        [Display(Name = "Description (Arabic)")]
        public string DescriptionAr { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }  // ✅ الصورة الحالية

        [Display(Name = "Change Image")]
        public IFormFile? ImageFile { get; set; }  // ✅ صورة جديدة

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

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