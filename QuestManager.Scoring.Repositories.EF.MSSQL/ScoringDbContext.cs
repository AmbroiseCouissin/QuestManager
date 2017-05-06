using Microsoft.EntityFrameworkCore;

namespace QuestManager.Scoring.Repositories.EF.MSSQL
{
    public class ScoringDbContext : DbContext
    {
        public ScoringDbContext(DbContextOptions<ScoringDbContext> options) 
            : base(options)
        {
        }

        public DbSet<ScoringDto> Scorings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScoringDto>().ToTable("Scorings");
        }
    }
}
