using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NewsParser.Data;
using NewsParser.Interfaces;
using NewsParser.Models;
using System.Collections.Generic;
using System.Data;

namespace NewsParser.Repository
{
    public class NewsParseRepository : INewsParseRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public NewsParseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddNews(List<Post> posts)
        {
            // Перебираем посты и добавляем их в DbSet в контексте
            foreach (var post in posts)
            {
                _dbContext.Posts.Add(post);
            }

            // Сохраняем изменения в базе данных
            _dbContext.SaveChanges();
        }
    }
}
