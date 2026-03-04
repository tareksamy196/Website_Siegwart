using System;
using System.Collections.Generic;

namespace Website.Siegwart.BLL.Dtos.Admin.SuperAdminAccount
{
    public class UserDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public DateTime? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
    }
}