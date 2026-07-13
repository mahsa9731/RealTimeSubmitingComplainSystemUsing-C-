using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Areas.Admin.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountAdminController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountAdminController(SignInManager<User>signInManager , UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        private const string AdminEmail = "adminMahsaa@gmail.com";
        private const string AdminPassword = "167Sh%ty";


        [HttpGet]
        public IActionResult Login()
        {
            return View(new AdminLoginViewModel());
        }

        
        [HttpPost]
        public async Task<IActionResult> Login( string pass)
        {
            var email = "adminMahsaa@gmail.com";
            var password = "167Sh%ty";
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "DashboardAdmin", new { area = "Admin" });
                }
            }
            ModelState.AddModelError("", "دسترسی غیرمجاز!");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", new { area = "Admin" });

        }
    }
}




