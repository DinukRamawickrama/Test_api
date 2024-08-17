using Microsoft.EntityFrameworkCore;
using WebAPP.Model;

namespace WebAPP
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        public DbSet<Model.Task> Tasks { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

    }
}
