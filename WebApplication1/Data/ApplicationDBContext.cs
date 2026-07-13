using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using WebApplication1.Models;


namespace WebApplication1.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<ComplaintModel> Complaint { get; set; }
        public DbSet<ComplaintMessageModel> ComplaintMessage { get; set; }
        //public DbSet<User> User { get; set; }
    }
}
