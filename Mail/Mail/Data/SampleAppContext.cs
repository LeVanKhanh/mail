using Microsoft.EntityFrameworkCore;
using Mail.Models;

namespace Mail.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppFile> File { get; set; }
    }
}
