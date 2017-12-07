using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Entitites
{
    public class CrowdSparkContext : DbContext, ICrowdSparkContext
    {
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Category> Categorys { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<Spark> Sparks { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Location> Locations { get; set; }

        public CrowdSparkContext()
        {
             
        }

        public CrowdSparkContext(DbContextOptions<CrowdSparkContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=CrowdSparkDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Spark>()
                .HasKey(e => new { e.ProjectId, e.UserId });
      //      modelBuilder.Entity<EpisodeCharacter>()
      //          .HasKey(e => new { e.EpisodeId, e.CharacterId });
        }
    }
}
