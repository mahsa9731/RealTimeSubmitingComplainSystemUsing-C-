using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class User : IdentityUser
    {
        //[Key]
        //public int UserId { get; set; }
        //[Required]
        //[MaxLength(300)]
        //public string Email { get; set; }

        //[Required]
        //[MaxLength(50)]
        //public string Password { get; set; }

        //public bool IsAdmin { get; set; }
        //[Required]
        //public DateTime RegisterDate { get; set; }

        public bool IsAdmin { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
