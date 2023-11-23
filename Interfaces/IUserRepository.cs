using NewsParser.Models;

namespace NewsParser.Interfaces
{
    public interface IUserRepository
    {
        User Login(string username, string password);
        void AddUser(User user);
    }
}
