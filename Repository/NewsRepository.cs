using NewsParser.Data;
using NewsParser.Interfaces;
using NewsParser.Models;
using System.Text.RegularExpressions;

namespace NewsParser.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public NewsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Post> GetNewsByDateRange()
        {
            return _dbContext.Posts.ToList();
        }

        public List<string> GetTopTenWords()
        {
            // Разделяем текст новостей на слова и подсчитываем частоту каждого слова
            var wordFrequency = _dbContext.Posts
                .Where(n => n.Content != null)
                .AsEnumerable() // Это приводит к выполнению запроса на стороне клиента
                .SelectMany(n => Regex.Split(n.Content.ToLower(), @"\W+"))
                .Where(word => !string.IsNullOrEmpty(word))
                .GroupBy(word => word)
                .OrderByDescending(group => group.Count())
                .Take(10)
                .Select(group => group.Key)
                .ToList();
            ;

            return wordFrequency;
        }

        public List<Post> SearchNewsByText(string text)
        {
            return _dbContext.Posts
                .Where(n => n.Content.Contains(text))
                .ToList();
        }
    }
}
