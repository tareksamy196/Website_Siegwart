namespace Website.Siegwart.BLL.Dtos.User
{
    public class UserCategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}