using Website.Siegwart.DAL.Enums;

namespace Website.Siegwart.DAL.Models
{
    public class VideoMedia : SeoEntity
    {
        public string VideoId { get; set; } = string.Empty;

        public string? SourceUrl { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int SortOrder { get; set; } = 100;
        public VideoCategory Category { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}