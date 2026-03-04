namespace Website.Siegwart.BLL.Dtos.User
{
    public class UserProductDto
    {
        public int Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryNameEn { get; set; } = string.Empty;  
        public string CategoryNameAr { get; set; } = string.Empty;  

        // SEO Fields
        public string Slug { get; set; } = string.Empty;
        public string? SeoTitleEn { get; set; }
        public string? SeoTitleAr { get; set; }
        public string? SeoDescriptionEn { get; set; }
        public string? SeoDescriptionAr { get; set; }
        public string? OgImageUrl { get; set; }
    }
}