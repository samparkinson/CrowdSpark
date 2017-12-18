using System;
using System.Linq;
using CrowdSpark.Logic;
using CrowdSpark.Models;
using CrowdSpark.Common;
using Moq;
using Xunit;
using System.Collections.Generic;
using CrowdSpark.Entitites;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Logic.Tests
{
    public class ProjectLogicTests
    {
        Mock<IProjectRepository> projectRepositoryMock;
        Mock<ILocationRepository> locationRepositoryMock;
        Mock<ISkillLogic> skillLogicMock;
        Mock<ISparkLogic> sparkLogicMock;
        Mock<ILocationLogic> locationLogicMock;
        Mock<ICategoryLogic> categoryLogicMock;
        UserRepository userRepository;
        SkillRepository skillRepository;
        ProjectRepository projectRepository;
        LocationRepository locationRepository;
        CategoryRepository categoryRepository;
        CrowdSparkContext context;

        public ProjectLogicTests()
        {
            projectRepositoryMock = new Mock<IProjectRepository>();
            locationRepositoryMock = new Mock<ILocationRepository>();
            skillLogicMock = new Mock<ISkillLogic>();
            sparkLogicMock = new Mock<ISparkLogic>();
            locationLogicMock = new Mock<ILocationLogic>();
            categoryLogicMock = new Mock<ICategoryLogic>();
        }

        public void Dispose()
        {
            DisposeContextIfExists();
        }

        void DisposeContextIfExists()
        {
            if (userRepository != null)
            {
                context.Database.RollbackTransaction();
                userRepository?.Dispose();
                skillRepository?.Dispose();
                projectRepository?.Dispose();
                locationRepository?.Dispose();
                categoryRepository?.Dispose();
            }
        }

        [Fact]
        public async void GetApprovedSparskAsync_GivenProjectExistsAndHasSkills_ReturnsProjectSparks()
        {
            var sparks = new SparkDTO[]
            {
                new SparkDTO() { UId = 1, PId = 3, Status = (int)SparkStatus.APPROVED, CreatedDate = System.DateTime.UtcNow},
                new SparkDTO() { UId = 2, PId = 3, Status = (int)SparkStatus.APPROVED, CreatedDate = System.DateTime.UtcNow},
                new SparkDTO() { UId = 3, PId = 3, Status = (int)SparkStatus.APPROVED, CreatedDate = System.DateTime.UtcNow},
                new SparkDTO() { UId = 6, PId = 3, Status = (int)SparkStatus.DECLINED, CreatedDate = System.DateTime.UtcNow},
                new SparkDTO() { UId = 5, PId = 3, Status = (int)SparkStatus.PENDING, CreatedDate = System.DateTime.UtcNow},
            };

            var project = new ProjectDTO
            {
                Id = 3,
                Title = "Title",
                Description = "Description",
                Sparks = sparks,
                CreatedDate = System.DateTime.UtcNow
            };

            projectRepositoryMock.Setup(p => p.FindAsync(3)).ReturnsAsync(project);

            using (var logic = new ProjectLogic(projectRepositoryMock.Object, locationRepositoryMock.Object, skillLogicMock.Object, sparkLogicMock.Object, locationLogicMock.Object, categoryLogicMock.Object))
            {
                var results = await logic.GetApprovedSparksAsync(3);

                Assert.Equal(3, results.Count());
                Assert.Contains(sparks[0], results);
                Assert.Contains(sparks[1], results);
                Assert.Contains(sparks[2], results);
            }
        }

        #region IntegrationTests

        public static IEnumerable<object[]> GetCreateProjectTestParams()
        {
            List<(CreateProjectDTO user, int creatorId, ResponseLogic expected)> projectsToTest = new List<(CreateProjectDTO, int, ResponseLogic)>
            {
                (new CreateProjectDTO() { Title = "Project", Description = "WithNoLocationOrCateogory" }, 1, ResponseLogic.SUCCESS),
                (new CreateProjectDTO() { Title = "Project", Description = "WithNoLocationAndExistingCategory", Category = new CategoryDTO() { Id = 1, Name = "Cooking"} }, 1, ResponseLogic.SUCCESS),
                (new CreateProjectDTO() { Title = "Project", Description = "WithNoLocationAndNonExistingCategory", Category = new CategoryDTO() { Id = 0, Name = "Dancing"} }, 1, ResponseLogic.SUCCESS),
                (new CreateProjectDTO() { Title = "Project", Description = "WithNoCategoryAndNonExistingLocation", Location = new LocationDTO() { Id = 0, City = "Brisbane", Country = "Australia" } }, 1, ResponseLogic.SUCCESS),
                (new CreateProjectDTO() { Title = "Project", Description = "WithNoCategoryAndExistingLocation", Location = new LocationDTO() { Id = 1, City = "Sydney", Country = "Australia" } }, 1, ResponseLogic.SUCCESS),
                (new CreateProjectDTO() { Title = "Project", Description = "WithCategoryAndExistingLocation", Location = new LocationDTO() { Id = 1, City = "Sydney", Country = "Australia" }, Category = new CategoryDTO() { Id = 1, Name = "Cooking"} }, 1, ResponseLogic.SUCCESS)
            };

            foreach (var testParam in projectsToTest)
            {
                yield return new object[] { testParam.user, testParam.creatorId, testParam.expected };
            }
        }

        [Theory]
        [MemberData(nameof(GetCreateProjectTestParams))]
        public async void CreateAsync(CreateProjectDTO project, int userId, ResponseLogic expected)
        {
            var existingCategry = new Category() { Id = 1, Name = "Cooking" };
            var creatingUser = new User() { Id = 1, Firstname = "IAlready", Surname = "Exist", Mail = "already@example.com", AzureUId = "existingAuzreUId" };
            var existingLocation = new Location() { Id = 1, City = "Sydney", Country = "Australia" };

            context = setupContextForIntegrationTests();
            userRepository = new UserRepository(context);
            skillRepository = new SkillRepository(context);
            projectRepository = new ProjectRepository(context);
            locationRepository = new LocationRepository(context);
            categoryRepository = new CategoryRepository(context);

            var locationLogic = new LocationLogic(locationRepository, userRepository, projectRepository);
            var categoryLogic = new CategoryLogic(categoryRepository, projectRepository);

            context.Users.Add(creatingUser);
            context.Locations.Add(existingLocation);
            context.Categories.Add(existingCategry);
            context.SaveChanges();

            //SanityCheck
            Assert.Equal(1, await context.Users.CountAsync());
            Assert.Equal(creatingUser, await context.Users.FirstAsync());
            Assert.Equal(1, await context.Locations.CountAsync());
            Assert.Equal(existingLocation, await context.Locations.FirstAsync());
            Assert.Equal(1, await context.Categories.CountAsync());
            Assert.Equal(existingCategry, await context.Categories.FirstAsync());

            using (var logic = new ProjectLogic(projectRepository, locationRepository, skillLogicMock.Object, sparkLogicMock.Object, locationLogic,  categoryLogic))
            {
                var result = await logic.CreateAsync(project, userId);

                Assert.Equal(expected, result.outcome);
            }
        }

        private CrowdSparkContext setupContextForIntegrationTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<CrowdSparkContext>()
                .UseSqlite(connection);

            context = new CrowdSparkContext(builder.Options);
            context.Database.EnsureCreated();
            context.Database.BeginTransaction();

            return context;
        }

        #endregion

    }
}
