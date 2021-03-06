﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CrowdSpark.Entitites
{
    public interface ICrowdSparkContext : IDisposable
    {
        DbSet<Attachment> Attachments { get; set; }

        DbSet<Category> Categories { get; set; }

        DbSet<Project> Projects { get; set; }

        DbSet<Skill> Skills { get; set; }

        DbSet<Spark> Sparks { get; set; }

        DbSet<User> Users { get; set; }

        DbSet<Location> Locations { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}
