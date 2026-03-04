using System.ComponentModel;

namespace Website.Siegwart.BLL.Dtos.Admin.CategoryDtos
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [DisplayName("Name (English)")]
        public string NameEn { get; set; } = string.Empty;

        [DisplayName("Name (Arabic)")]
        public string NameAr { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } 
    }
}