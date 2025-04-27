using GitHubRepoAdmin.Domain;
using Microsoft.EntityFrameworkCore;

namespace GitHubRepoAdmin.Infra.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Favorite>().HasKey(m => m.Id);
            base.OnModelCreating(builder);
        }
    }
}
