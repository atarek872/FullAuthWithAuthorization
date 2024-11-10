using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using DynamicRoleBasedAuthorization.Models;
using Microsoft.AspNetCore.Authorization;
using DynamicRoleBasedAuthorization.ViewModels;

namespace DynamicRoleBasedAuthorization.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // List all users and their roles
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = await Task.WhenAll(users.Select(async user => new
            {
                user,
                roles = await _userManager.GetRolesAsync(user)
            }));

            return View(userRoles);
        }

        // Edit roles for a user
        public async Task<IActionResult> EditRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            // Get all roles in the system
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var viewModel = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = allRoles.Select(role => new UserRole
                {
                    RoleName = role,
                    IsSelected = roles.Contains(role)
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(EditUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if (role.IsSelected && !roles.Contains(role.RoleName))
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                }
                else if (!role.IsSelected && roles.Contains(role.RoleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                }
            }

            return RedirectToAction("Index");
        }

    }


}
