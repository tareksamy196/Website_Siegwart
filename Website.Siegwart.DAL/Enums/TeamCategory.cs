using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.DAL.Enums
{
    public enum TeamCategory
    {
        [Display(Name = "Top Management")]
        TopManagement = 1,

        [Display(Name = "Department Manager")]
        DepartmentManager = 2,

        [Display(Name = "Technical Expert")]
        TechnicalExpert = 3,

        [Display(Name = "Administrative Support")]
        AdminSupport = 4
    }
}