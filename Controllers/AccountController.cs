using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DynamicRoleBasedAuthorization.ViewModels;
using DynamicRoleBasedAuthorization.Models;

namespace DynamicRoleBasedAuthorization.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;  // Use ApplicationUser here
        private readonly SignInManager<ApplicationUser> _signInManager;  // Use ApplicationUser here

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Automatically sign the user in after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Assign a role to the new user (for example, "User" role)
                    await _userManager.AddToRoleAsync(user, "User");  // You can customize this logic based on your needs

                    return RedirectToAction("Login", "Account"); // Redirect to Login page after successful registration
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home"); // Redirect to home after successful login
                }

                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken] // This is important for security to prevent CSRF attacks
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account"); // Redirect to Login after logout
        }
    }
}
