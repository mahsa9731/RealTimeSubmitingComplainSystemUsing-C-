using WebApplication1.Models;
namespace WebApplication1.Data.Repository
{
    public interface IUserRepository
    {
        public bool isExistUserByEmail(string Email);
        void AddUser(User user);
        User GetUserForLogin(string Email, string Password);
    }

    public class UserRepository : IUserRepository
    {
        private ApplicationDBContext _context;
        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserForLogin(string Email, string Password)
        {
            throw new NotImplementedException();
        }

        //public User GetUserForLogin(string email, string password)
        //{
        // return _context.Users.SingleOrDefault(u => u.Email == email && u.PasswordHash == password);
        //}

        public bool isExistUserByEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

    }
}