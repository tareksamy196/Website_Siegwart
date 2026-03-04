using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.BLL.Dtos.Admin.VideoMedia
{
    public class VideoMediaCreateDto
    {
        [Required]
        [Display(Name = "YouTube URL or Id")]
        public string VideoUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string TitleEn { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string TitleAr { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? DescriptionEn { get; set; }

        [MaxLength(4000)]
        public string? DescriptionAr { get; set; }

        public bool IsPublished { get; set; } = true;

        public int SortOrder { get; set; } = 100;
    }
}