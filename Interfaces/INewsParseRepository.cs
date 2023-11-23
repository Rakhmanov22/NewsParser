using NewsParser.Models;

namespace NewsParser.Interfaces
{
    public interface INewsParseRepository
    {
        void AddNews(List<Post> post);        
    }
}
