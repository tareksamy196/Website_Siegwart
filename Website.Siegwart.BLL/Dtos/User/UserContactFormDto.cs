using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.Siegwart.BLL.Dtos.User
{
    public class UserContactFormDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(300)]
        public string? Subject { get; set; }

        [Required, StringLength(2000, MinimumLength = 10)]
        public string Message { get; set; } = string.Empty;
    }
}
