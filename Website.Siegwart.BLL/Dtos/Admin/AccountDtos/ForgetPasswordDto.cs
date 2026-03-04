using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.BLL.Dtos.Account
{
    /// <summary>
    /// DTO for requesting password reset (Forgot Password).
    /// </summary>
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        public void Normalize()
        {
            Email = Email?.Trim().ToLowerInvariant() ?? string.Empty;
        }
    }
}