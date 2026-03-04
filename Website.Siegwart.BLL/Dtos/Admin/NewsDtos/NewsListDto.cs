namespace Website.Siegwart.BLL.Dtos.Admin.NewsDtos
{
    public class NewsListDto
    {
        public int Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime PublishedOn { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}