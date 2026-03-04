namespace Website.Siegwart.BLL.Dtos.Admin.NewsDtos
{
    public class NewsDetailsDto
    {
        public int Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime PublishedOn { get; set; }
        public bool IsPublished { get; set; }

        // SEO
        public string? Slug { get; set; }
        public string? SeoTitleEn { get; set; }
        public string? SeoTitleAr { get; set; }
        public string? SeoDescriptionEn { get; set; }
        public string? SeoDescriptionAr { get; set; }
        public string? OgImageUrl { get; set; }

        // Audit
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}