namespace Website.Siegwart.BLL.Dtos.Admin.VideoMedia
{
    public class VideoMediaListItemDto
    {
        public int Id { get; set; }
        public string VideoId { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}