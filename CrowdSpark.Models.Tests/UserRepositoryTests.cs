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
            // If using real DB rollback transaction
            context.Database.RollbackTransaction();
            context.Dispose();
        }

        [Fact]
        public async void CreateAsync_GivenValidUser_ReturnsNewUserID()
        {
            var user = new UserDTO
            {
                Firstname = "Bob",
                Surname = "Smith",
                Mail = "bobsmith@example.com" 
            };
            //var contextMock = new Mock<ICrowdSparkContext>();

            //contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(1);
            //contextMock.Setup(c => c.Users.Add(It.IsAny<User>())).Returns(1);

            using (var repository = new UserRepository(context))
            {
                var id = await repository.CreateAsync(user);
                Assert.Equal((await context.Users.FirstAsync()).Id, id);
            }
        }

        [Fact]
        public async void CreateAsync_GivenValidUserDeletesIt_ReturnsSuccess()
        {
            var user = new UserDTO
            {
                Firstname = "Bob",
                Surname = "Smith",
                Mail = "bobsmith@example.com"
            };

            using (var repository = new UserRepository(context))
            {
                var id = await repository.CreateAsync(user);
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
            //TODO, see what excpetion will actually be thrown if that DB is offline
            var user = new UserDTO
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
                await Assert.ThrowsAsync<DbUpdateException>(async () => { await repository.CreateAsync(user); });
            }
        }

        [Fact]
        public async void CreateAsync_GivenDatabaseSaveDoesNotChangeRecords_ReturnsDbUpdateException()
        {
            var userDTO = new UserDTO
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
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.CreateAsync(userDTO));
            }
        }

        [Fact]
        public void CreateAsync_GivenValidUser_SuccesfullyCreatesDBRecord()
        {
            // Create in memory or SQLlite DB
        }
    }
}
