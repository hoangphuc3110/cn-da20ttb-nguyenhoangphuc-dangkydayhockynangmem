using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Models
{
    public class UserDAO
    {
        private readonly SchoolContext _context;

        public UserDAO(SchoolContext context)
        {
            _context = context;
        }
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserByUserName(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
        public User GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }
    }
}
