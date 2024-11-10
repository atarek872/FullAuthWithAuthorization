using DynamicRoleBasedAuthorization.Controllers;

namespace DynamicRoleBasedAuthorization.ViewModels
{
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public List<UserRole> Roles { get; set; }
    }
}
