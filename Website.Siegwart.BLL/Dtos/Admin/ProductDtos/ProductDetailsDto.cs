namespace Website.Siegwart.BLL.Dtos.Admin.ProductDtos
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryNameEn { get; set; } = string.Empty;  
        public string CategoryNameAr { get; set; } = string.Empty;  
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        // SEO Fields
        public string? Slug { get; set; }
        public string? SeoTitleEn { get; set; }
        public string? SeoTitleAr { get; set; }
        public string? SeoDescriptionEn { get; set; }
        public string? SeoDescriptionAr { get; set; }
    }
}