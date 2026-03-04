using System;
using System.ComponentModel.DataAnnotations;

namespace Website.Siegwart.DAL.Models
{
    /// <summary>
    /// Contact form submission from website
    /// </summary>
    public class ContactMessage : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [StringLength(300)]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Has admin read this message?
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Has admin replied to this message?
        /// </summary>
        public bool IsReplied { get; set; } = false;

        /// <summary>
        /// Admin's internal notes
        /// </summary>
        [StringLength(1000)]
        public string? AdminNotes { get; set; }

        /// <summary>
        /// User's IP address (for spam prevention)
        /// </summary>
        [StringLength(50)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent (browser info)
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }
    }
}