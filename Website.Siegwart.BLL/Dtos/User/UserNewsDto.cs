namespace Website.Siegwart.BLL.Dtos.User
{
    public class NewsViewDto
    {
        public int Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime PublishedOn { get; set; }

        // SEO Fields (للروابط الصديقة لمحركات البحث)
        public string Slug { get; set; } = string.Empty;
        public string? SeoTitleEn { get; set; }
        public string? SeoTitleAr { get; set; }
        public string? SeoDescriptionEn { get; set; }
        public string? SeoDescriptionAr { get; set; }
        public string? OgImageUrl { get; set; }
    }
}