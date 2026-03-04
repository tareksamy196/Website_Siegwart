using System;

namespace Website.Siegwart.BLL.Dtos.Admin.VideoMedia
{
    public class VideoMediaDto
    {
        public int Id { get; set; }

        // YouTube VideoId (11 chars)
        public string VideoId { get; set; } = string.Empty;

        // Original URL entered by admin (optional)
        public string? SourceUrl { get; set; }

        // Thumbnail URL (may be null; client can fallback to YouTube)
        public string? ThumbnailUrl { get; set; }

        // Titles and descriptions (admin-facing)
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }

        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }

        // Publication + order
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }

        // Audit
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}