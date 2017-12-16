using System;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class SparkRepositoryTests
    {
        private readonly CrowdSparkContext context;

        public SparkRepositoryTests()
        {
            // Setup Seed Here
            // If using real DB, begin transaction

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<CrowdSparkContext>()
                .UseSqlite(connection);

            context = new CrowdSparkContext(builder.Options);
            context.Database.EnsureCreated();

            //SEED IN HERE IF YOU WANT

            context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            context.Database.RollbackTransaction();
            context.Dispose();
        }
    }
}
