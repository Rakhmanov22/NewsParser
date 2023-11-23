using NewsParser.Models;

namespace NewsParser.Interfaces
{
    public interface INewsRepository
    {
        List<Post> GetNewsByDateRange();
        List<string> GetTopTenWords();
        List<Post> SearchNewsByText(string text);
    }
}
