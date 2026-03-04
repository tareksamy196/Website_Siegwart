namespace Website.Siegwart.DAL.Models
{
    public class Product : SeoEntity
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 100;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}