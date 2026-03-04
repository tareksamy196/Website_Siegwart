namespace Website.Siegwart.BLL.Dtos.Admin.ProductDtos
{
    public class ProductListDto
    {
        public int Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryNameEn { get; set; } = string.Empty;  
        public string CategoryNameAr { get; set; } = string.Empty;  
        public DateTime CreatedOn { get; set; }
    }
}