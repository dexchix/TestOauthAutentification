using CommitsManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommitsManager.Domain
{
    public class AppDBContext: DbContext
    {
        public DbSet<CommitEntity> Commits { get; set; }
        public DbSet<RepositoryEntity> Repositories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=WIN-DH1JES879GL\SQLSERVER; Database=CommitsManager; Persist Security Info=false; User ID='sa'; Password='sa'; MultipleActiveResultSets=True; ;TrustServerCertificate=true; Trusted_Connection=True;");
        }
    }
}
