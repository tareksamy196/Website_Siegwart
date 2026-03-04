namespace Website.Siegwart.DAL.Models
{
    public class News : SeoEntity
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string SummaryEn { get; set; } = string.Empty;
        public string SummaryAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime? PublishedOn { get; set; }
        public bool IsPublished { get; set; } = false;
    }
}