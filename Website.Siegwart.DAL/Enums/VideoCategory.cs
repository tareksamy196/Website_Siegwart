using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.DAL.Enums
{
    // ✅ NEW — was missing, used by VideoMedia model
    public enum VideoCategory
    {
        [Display(Name = "Company Overview")]
        CompanyOverview = 1,

        [Display(Name = "Products")]
        Products = 2,

        [Display(Name = "Projects")]
        Projects = 3,

        [Display(Name = "News & Events")]
        NewsAndEvents = 4
    }
}