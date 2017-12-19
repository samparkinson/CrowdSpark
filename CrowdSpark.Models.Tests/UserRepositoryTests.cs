using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class UserRepositoryTests
    {
        private readonly CrowdSparkContext context;

        public UserRepositoryTests()
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
        public async void CreateAsync_GivenValidUser_ReturnsNewUserID()
        {
            var user = new UserCreateDTO
            {
                Firstname = "Bob",
                Surname = "Smith",
                Mail = "bobsmith@example.com" 
            };

            using (var repository = new UserRepository(context))
            {
                var id = await repository.CreateAsync(user, "abcd");
                Assert.Equal((await context.Users.FirstAsync()).Id, id);
            }
        }

        [Fact]
        public async void CreateAsync_GivenValidUserDeletesIt_ReturnsSuccess()
        {
            var user = new UserCreateDTO
            {
                Firstname = "Bob",
                Surname = "Smith",
                Mail = "bobsmith@example.com"
            };

            using (var repository = new UserRepository(context))
            {
                var id = await repository.CreateAsync(user, "abcd");
                Assert.Equal((await context.Users.FirstAsync()).Id, id);
                Assert.True( await repository.DeleteAsync(id) );
            }
        }

        [Fact]
        public async void DeleteAsync_GivenInvalidUserId_ReturnsFailed()
        {
            using (var repository = new UserRepository(context))
            {
                Assert.False(await repository.DeleteAsync(-1));
            }
        }

        [Fact]
        public async void CreateAsync_GivenDatabaseSaveError_ReturnsDbUpdateException()
        {
            var user = new UserCreateDTO
            {
                Firstname = "Bob",
                Surname = "Smith",
                Mail = "bobsmith@example.com"
            };
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ThrowsAsync(new DataException("error"));
            contextMock.Setup(c => c.Users.Add(It.IsAny<User>()));

            using (var repository = new UserRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(async () => { await repository.CreateAsync(user, "abcd"); });
            }
        }

        
        [Fact]
        public async void CreateAsync_GivenDatabaseSaveDoesNotChangeRecords_ReturnsDbUpdateException()
        {
            var userDTO = new UserCreateDTO
            {
                Firstname = "Bob",
                Surname = "Smith",
                Mail = "bobsmith@example.com"
            };

            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(0);
            contextMock.Setup(c => c.Users.Add(It.IsAny<User>()));

            using (var repository = new UserRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.CreateAsync(userDTO, "abcd"));
            }
        }
    }
}
