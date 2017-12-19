using System;
using System.Threading;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class SparkRepositoryTests
    {
        private readonly CrowdSparkContext context;

        public SparkRepositoryTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<CrowdSparkContext>()
                .UseSqlite(connection);

            context = new CrowdSparkContext(builder.Options);
            context.Database.EnsureCreated();

            context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            context.Database.RollbackTransaction();
            context.Dispose();
        }

        [Fact]
        public async void CreateAsync_GivenProjectAndUserExist_ReturnsSparkId()
        {
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.Projects.FindAsync(1)).ReturnsAsync(new Project() { Id = 1});
            contextMock.Setup(c => c.Users.FindAsync(1)).ReturnsAsync(new User() { Id = 1 });
            contextMock.Setup(c => c.Sparks.Add(It.IsAny<Spark>()));
            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(1);

            using (var repo = new SparkRepository(contextMock.Object))
            {
                Assert.Equal((1, 1), await repo.CreateAsync(1,1));
            }
        }

        [Fact]
        public async void CreateAsync_GivenDBReturnsNoChanges_ThrowsDbUpdateException()
        {
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.Projects.FindAsync(1)).ReturnsAsync(new Project() { Id = 1 });
            contextMock.Setup(c => c.Users.FindAsync(1)).ReturnsAsync(new User() { Id = 1 });
            contextMock.Setup(c => c.Sparks.Add(It.IsAny<Spark>()));
            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(0);

            using (var repo = new SparkRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateAsync(1,1));
            }
        }

        [Fact]
        public async void CreateAsync_GivenDBException_ThrowsDbUpdateException()
        {
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.Projects.FindAsync(1)).ReturnsAsync(new Project() { Id = 1 });
            contextMock.Setup(c => c.Users.FindAsync(1)).ReturnsAsync(new User() { Id = 1 });
            contextMock.Setup(c => c.Sparks.Add(It.IsAny<Spark>()));
            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ThrowsAsync(new System.Data.DataException() { });

            using (var repo = new SparkRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateAsync(1, 1));
            }
        }
    }
}
