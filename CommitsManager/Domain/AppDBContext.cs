using CommitsManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommitsManager.Domain
{
    public class AppDBContext: DbContext
    {
        public DbSet<Commit> Commits { get; set; }
        public DbSet<Repository> Repositories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-ODVNM0A\SQLEXPRESS; Database=CommitsManager; Persist Security Info=false; User ID='sa'; Password='sa'; MultipleActiveResultSets=True; ;TrustServerCertificate=true; Trusted_Connection=True;");
        }
    }
}
