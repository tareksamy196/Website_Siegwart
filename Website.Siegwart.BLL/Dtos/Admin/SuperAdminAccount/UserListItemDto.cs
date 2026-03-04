using System.Collections.Generic;

namespace Website.Siegwart.BLL.Dtos.Admin.SuperAdminAccount
{
    public class UserListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}