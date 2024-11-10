using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DynamicRoleBasedAuthorization.ViewModels;
using DynamicRoleBasedAuthorization.Models;
using Microsoft.EntityFrameworkCore;
using VimeoDotNet.Models;
using System;
using System.Security.Claims;
using DynamicRoleBasedAuthorization.Models.MetaVideos;
using DynamicRoleBasedAuthorization.Services;

namespace DynamicRoleBasedAuthorization.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;  // Use ApplicationUser here
        private readonly SignInManager<ApplicationUser> _signInManager;  // Use ApplicationUser here
        private ApplicationDbContext _context;
        private IMailService _mailService;


        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext metavideosContext, SignInManager<ApplicationUser> signInManager, IMailService mailService)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _context = metavideosContext;
            _mailService = mailService;

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
                    await _userManager.AddToRoleAsync(user, "User");   
                    Bouns obj = new Bouns();
                    obj.UserId = user.Id;
                    obj.TotalVideosViewed = 0;
                    obj.TotalBouns = 100;

                    _context.Bouns.Add(obj);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction("Login", "Account");
                    return RedirectToAction("Index", "Home");
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
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var CheckIfHaveBounceProfile = _context.Bouns.Where(t => t.UserId == userId).Count();

                    if (CheckIfHaveBounceProfile == 0)
                    {
                        var Bouns = new Bouns()
                        {
                            UserId = userId,
                            TotalBouns = 100,
                            TotalVideosViewed = 0

                        };
                        _context.Add(Bouns);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
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
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            try
            {
                await _mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }
    }
}
