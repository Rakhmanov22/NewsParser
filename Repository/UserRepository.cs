using NewsParser.Data;
using NewsParser.Interfaces;
using NewsParser.Models;
using NewsParser.Utils;

namespace NewsParser.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User Login(string login, string password)
        {
            string hash = password.CreateMD5();
            if (_dbContext.Users.Any(x => x.Username == login && x.Password == hash))
            {
                return _dbContext.Users.SingleOrDefault(x => x.Username == login && x.Password == hash);
            }
            return null;
        }
        public void AddUser(User user)
        {
            user.Password = user.Password.CreateMD5();
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
    }
}
