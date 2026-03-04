using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Website.Siegwart.DAL.Enums;

namespace Website.Siegwart.PL.Helper
{
    public static class TeamCategoryHelper
    {
        public static string GetLocalizedName(TeamCategory category, bool isArabic)
        {
            return isArabic ? GetArabicName(category) : GetEnglishName(category);
        }

        public static string GetEnglishName(TeamCategory category)
        {
            var field = category.GetType().GetField(category.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? category.ToString();
        }
        public static string GetArabicName(TeamCategory category)
        {
            return category switch
            {
                TeamCategory.TopManagement => "الإدارة العليا",
                TeamCategory.DepartmentManager => "مديرو الادارات",
                TeamCategory.TechnicalExpert => "الخبراء الفنيون",
                TeamCategory.AdminSupport => "الدعم الإداري",
                _ => category.ToString()
            };
        }
    }
}