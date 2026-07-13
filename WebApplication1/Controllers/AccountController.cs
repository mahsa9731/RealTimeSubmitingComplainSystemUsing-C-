using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private string TranslateToFarsi(string englishMessage)
        {
            if(englishMessage.Contains("Passwords must be at least 6 characters."))
                return "کلمه عبور باید حداقل 6 حرف داشته باشد.";
            if (englishMessage.Contains("Passwords must have at least one non alphanumeric character"))
                return "کلمه عبور باید حداقل یک کاراکتر غیر از حرف داشته باشد";
            if (englishMessage.Contains("Passwords must have at least one lowercase"))
                return "کلمه عبور باید حداقل یک حرف کوچک ('a'-'z') داشته باشد";
            if (englishMessage.Contains("Passwords must have at least one uppercase"))
                return "کلمه عبور باید حداقل یک حرف بزرگ ('A'-'Z') داشته باشد";

            
            return englishMessage; 
        }

        #region Register
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email.ToLower());
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "ایمیل وارد شده قبلا ثبت نام کرده است.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower(),
                IsAdmin = false,
                RegisterDate = DateTime.Now
            };

           
           
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", TranslateToFarsi(error.Description));
                }

                    
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return View("SuccessRegister", model);
        }
        #endregion

        #region Login
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email.ToLower(), model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "اطلاعات وارد شده صحیح نمی باشد.");
                return View(model);
            }

            var claims = new List<Claim>
            {
            
            new Claim(ClaimTypes.Name, model.Email),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : (DateTimeOffset?)null,
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(principal, properties);
            return RedirectToAction("Index", "Home");
        }
        #endregion

        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

