using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.BLL.Dtos.Admin.AccountDtos
{
    /// <summary>
    /// DTO for login (sign in) form.
    /// </summary>
    public class LoginAdminDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; } = false;
        public string? ReturnUrl { get; set; }
        public void Normalize()
        {
            Email = Email?.Trim().ToLowerInvariant() ?? string.Empty;
            ReturnUrl = ReturnUrl?.Trim();
        }
    }
}