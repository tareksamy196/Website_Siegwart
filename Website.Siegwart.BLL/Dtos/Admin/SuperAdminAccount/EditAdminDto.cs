using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.BLL.Dtos.Admin.SuperAdminAccount
{
    public class EditAdminDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\-\._@+]+$", ErrorMessage = "Username contains invalid characters.")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}