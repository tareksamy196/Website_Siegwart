namespace Website.Siegwart.DAL.Models
{
    // Base for any entity that has its own public page
    public abstract class SeoEntity : BaseEntity
    {
        public string Slug { get; set; } = string.Empty;
        public string SeoTitleEn { get; set; } = string.Empty;
        public string SeoTitleAr { get; set; } = string.Empty;
        public string SeoDescriptionEn { get; set; } = string.Empty;
        public string SeoDescriptionAr { get; set; } = string.Empty;
        public string? SeoKeywords { get; set; }
        public string? OgImageUrl { get; set; }
    }
}