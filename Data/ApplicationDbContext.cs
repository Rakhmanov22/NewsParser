using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NewsParser.Models;

namespace NewsParser.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
