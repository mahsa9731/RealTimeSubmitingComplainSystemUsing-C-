using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Areas.Admin.Models
{
    public class AdminLoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; } 
    }
}
