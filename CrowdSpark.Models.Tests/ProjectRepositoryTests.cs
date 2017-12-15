using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class ProjectRepositoryTests
    {
        private readonly CrowdSparkContext context;       
        
        public ProjectRepositoryTests()
        {
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

        [Fact]
        public async void CreateAsync_GivenValidProject_ReturnsNewProjectID()
        {
            var loc = new LocationCreateDTO()
            {
                City = "Copenhagen",
                Country = "Denmark"
            };
            
            using (var repo = new LocationRepository(context))
            {
                var id = await repo.CreateAsync(loc);
                var location = await repo.FindAsync(id);

                var project = new CreateProjectDTO
                {
                    Title = "Super hot flaming bicycle",
                    Description = "This project is about creating the coolest bike ever.",
                    Location = location
                };

                using (var repo1 = new ProjectRepository(context))
                {
                    var idproj = await repo1.CreateAsync(project);
                    Assert.NotNull(await context.Locations.FindAsync(idproj));
                }
            }
        }

        [Fact]
        public async void CreateAsync_GivenValidInformation_CheckIfProjectWasCreatedThenDelete()
        {
            var loc = new LocationCreateDTO()
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            using (var repo = new LocationRepository(context))
            {
                var id = await repo.CreateAsync(loc);
                var location = await repo.FindAsync(id);

                var project = new CreateProjectDTO
                {
                    Title = "Super hot flaming bicycle",
                    Description = "This project is about creating the coolest bike ever.",
                    Location = location
                };

                using (var repo1 = new ProjectRepository(context))
                {
                    var idproj = await repo1.CreateAsync(project);
                    Assert.NotNull(await context.Projects.FindAsync(idproj));
                    Assert.True(await repo1.DeleteAsync(idproj));
                }
            }
        }

        [Fact]
        public async void SearchAsync_GivenProjectsExist_ReturnsProjects()
        {
            var projects = new Project[]
            {
                new Project() { Title = "TestOne", Description = "Descritpion", CreatedDate = System.DateTime.UtcNow },
                new Project() { Title = "T3st", Description = "TestTwo", CreatedDate = System.DateTime.UtcNow },
                new Project() { Title = "Title", Description = "Descritpion", CreatedDate = System.DateTime.UtcNow }
            };

            context.Projects.AddRange(projects);
            context.SaveChanges();

            //SanityCheck
            Assert.Equal(3, await context.Projects.CountAsync());

            using (var repo1 = new ProjectRepository(context))
            {
                var results = await repo1.SearchAsync("test");

                Assert.Equal(2, results.Count());
                Assert.Equal("TestOne", results.ToArray()[0].Title);
                Assert.Equal("T3st", results.ToArray()[1].Title);
            }
        }
    }
}
