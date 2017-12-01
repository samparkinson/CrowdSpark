using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CrowdSpark.Entitites
{
    public interface ICrowdSparkContext : IDisposable
    {
        DbSet<Attachment> Attachments { get; set; }

        DbSet<Category> Categorys { get; set; }

        DbSet<Post> Posts { get; set; }

        DbSet<Skill> Skills { get; set; }

        DbSet<Spark> Sparks { get; set; }

        DbSet<User> User { get; set; }

        DbSet<Location> Location { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}
