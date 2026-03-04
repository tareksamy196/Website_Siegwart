//using System.Collections.Generic;
//using Website.Siegwart.BLL.Dtos.Admin.VideoMedia;
//using Website.Siegwart.DAL.Enums;

//namespace Website.Siegwart.BLL.Dtos.User
//{
  
//    public class UserVideoGalleryDto
//    {
//        public List<VideoMediaDto> Videos { get; set; } = new();
//        public VideoCategory? SelectedCategory { get; set; }
//        public string SearchQuery { get; set; }
//        public int CurrentPage { get; set; } = 1;
//        public int TotalItems { get; set; }
//        public int PageSize { get; set; } = 12;

//        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
//        public bool HasPreviousPage => CurrentPage > 1;
//        public bool HasNextPage => CurrentPage < TotalPages;

//        public List<CategoryInfo> Categories { get; set; } = new();
//    }

//    public class CategoryInfo
//    {
//        public VideoCategory Category { get; set; }
//        public string NameEn { get; set; }
//        public string NameAr { get; set; }
//        public int Count { get; set; }
//        public bool IsSelected { get; set; }
//    }
//}