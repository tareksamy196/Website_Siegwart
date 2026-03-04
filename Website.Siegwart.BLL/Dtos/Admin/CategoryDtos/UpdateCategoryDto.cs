using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.BLL.Dtos.Admin.CategoryDtos
{
    public class UpdateCategoryDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name in English is required")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters")]
        [Display(Name = "Name (English)")]
        public string NameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name in Arabic is required")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters")]
        [Display(Name = "Name (Arabic)")]
        public string NameAr { get; set; } = string.Empty;
    }
}