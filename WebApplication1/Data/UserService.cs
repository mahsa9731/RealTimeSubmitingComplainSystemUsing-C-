using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        public async Task<SignInResult> LoginAsync(string email, string password)
        {
           
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return SignInResult.Failed;

            
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            return result;
        }
    }
}

