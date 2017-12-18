using System;
using System.Collections.Generic;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Logic.Tests
{
    public class UserLogicTests
    {
        Mock<IUserRepository> userRepositoryMock;
        Mock<ISkillLogic> skillLogicMock;
        Mock<ISparkLogic> sparkLogicMock;
        Mock<ILocationLogic> locationLogicMock;
        UserRepository userRepository;
        SkillRepository skillRepository;
        ProjectRepository projectRepository;
        LocationRepository locationRepository;
        CrowdSparkContext context;

        public UserLogicTests()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            skillLogicMock = new Mock<ISkillLogic>();
            sparkLogicMock = new Mock<ISparkLogic>();
            locationLogicMock = new Mock<ILocationLogic>();
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
            }
        }

        #region UnitTests

        [Fact]
        public async void GetAsync_GivenUsersExist_ReturnsEnumerableLocations()
        {
            var usersToReturn = new List<UserDTO>()
            {
                new UserDTO { Id = 1, Firstname = "Bob", Surname = "Smith", Mail = "test@example.com" },
                new UserDTO { Id = 2, Firstname = "John", Surname = "Smith", Mail = "test@example.com" },
                new UserDTO { Id = 3, Firstname = "Gary", Surname = "Smith", Mail = "test@example.com" }
            };

            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(usersToReturn);

            using (var logic = new UserLogic(userRepositoryMock.Object, skillLogicMock.Object, sparkLogicMock.Object, locationLogicMock.Object))
            {
                var results = await logic.GetAsync();

                Assert.Equal(usersToReturn, results);
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void GetAsyncWithId_GivenExistingUserId_ReturnsUser()
        {
            var userToReturn = new UserDTO { Id = 2, Firstname = "John", Surname = "Smith", Mail = "test@example.com" };

            userRepositoryMock.Setup(u => u.FindAsync(2)).ReturnsAsync(userToReturn);

            using (var logic = new UserLogic(userRepositoryMock.Object, skillLogicMock.Object, sparkLogicMock.Object, locationLogicMock.Object))
            {
                var result = await logic.GetAsync(2);

                Assert.Equal(userToReturn, result);
                userRepositoryMock.Verify(u => u.FindAsync(2));
            }
        }

        [Fact]
        public async void GetIdFromAzureUIdAsync_GivenExistingUser_ReturnsUserId()
        {
            userRepositoryMock.Setup(u => u.GetIdAsync("foo")).ReturnsAsync(1);

            using (var logic = new UserLogic(userRepositoryMock.Object, skillLogicMock.Object, sparkLogicMock.Object, locationLogicMock.Object))
            {
                var result = await logic.GetIdFromAzureUIdAsync("foo");

                Assert.Equal(1, result);
                userRepositoryMock.Verify(u => u.GetIdAsync("foo"));
            }
        }

        #endregion

        #region IntegrationTests

        public static IEnumerable<object[]> GetCreateUserTestParams()
        {
            List<(UserCreateDTO user, String auzreUId, ResponseLogic expected)> usersToTest = new List<(UserCreateDTO, String, ResponseLogic)>
            {
                (new UserCreateDTO() { Firstname = "IAlready", Surname = "Exist", Mail = "already@example.com"}, "existingAuzreUId", ResponseLogic.ALREADY_EXISTS),
                (new UserCreateDTO() { Firstname = "Simple", Surname = "User", Mail = "simple@example.com"}, "UniqueAzureUId", ResponseLogic.SUCCESS),
                (new UserCreateDTO() { Firstname = "UserWith", Surname = "NewLocation", Mail = "withnewlocation@example.comn", Location = new LocationDTO() { Id = 0, City = "Brisbane", Country = "Australia" } }, "UniqueAzureUId", ResponseLogic.SUCCESS),
                (new UserCreateDTO() { Firstname = "UserWith", Surname = "NewLocation", Mail = "withexistinglocation@example.comn", Location = new LocationDTO() { Id = 1, City = "Sydney", Country = "Australia" } }, "UniqueAzureUId", ResponseLogic.SUCCESS),
                (new UserCreateDTO() { Firstname = "UserWith", Surname = "NewLocation", Mail = "withexistinglocation@example.comn", Location = new LocationDTO() { Id = 0, City = "Sydney", Country = "Australia" } }, "UniqueAzureUId", ResponseLogic.SUCCESS)
            };

            foreach (var testParam in usersToTest)
            {
                yield return new object[] { testParam.user, testParam.auzreUId, testParam.expected };
            }
        }

        [Theory]
        [MemberData(nameof(GetCreateUserTestParams))]
        public async void CreateAsync(UserCreateDTO user, String azureUId, ResponseLogic expected)
        {
            var existingUser = new User() { Id = 1, Firstname = "IAlready", Surname = "Exist", Mail = "already@example.com", AzureUId = "existingAuzreUId" };
            var existingLocation = new Location() { Id = 1, City = "Sydney", Country = "Australia" };

            userRepository = new UserRepository(setupContextForIntegrationTests());
            skillRepository = new SkillRepository(context);
            projectRepository = new ProjectRepository(context);
            locationRepository = new LocationRepository(context);

            var locationLogic = new LocationLogic(locationRepository, userRepository, projectRepository);

            context.Users.Add(existingUser);
            context.Locations.Add(existingLocation);
            context.SaveChanges();

            //SanityCheck
            Assert.Equal(1, await context.Users.CountAsync());
            Assert.Equal(existingUser, await context.Users.FirstAsync());
            Assert.Equal(1, await context.Locations.CountAsync());
            Assert.Equal(existingLocation, await context.Locations.FirstAsync());

            using (var logic = new UserLogic(userRepository, skillLogicMock.Object, sparkLogicMock.Object, locationLogic))
            {
                var result = await logic.CreateAsync(user, azureUId);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public async void AddSkillAsync_GivenSkillAndUserExist_ReturnsSUCCESS()
        {
            var existingUser = new User() { Id = 1, Firstname = "IAlready", Surname = "Exist", Mail = "already@example.com", AzureUId = "existingAuzreUId" };
            var existingSkill = new Skill() { Id = 1, Name = "Dancing" };

            userRepository = new UserRepository(setupContextForIntegrationTests());
            skillRepository = new SkillRepository(context);
            projectRepository = new ProjectRepository(context);
            //locationRepository = new LocationRepository(context);

            //var locationLogic = new LocationLogic(locationRepository, userRepository, projectRepository);
            var skillLogic = new SkillLogic(skillRepository, userRepository, projectRepository);

            context.Users.Add(existingUser);
            context.Skills.Add(existingSkill);
            context.SaveChanges();
            context.Entry(existingSkill).State = EntityState.Detached;

            //SanityCheck
            Assert.Equal(1, await context.Users.CountAsync());
            Assert.Equal(existingUser, await context.Users.FirstAsync());
            Assert.Equal(1, await context.Skills.AsNoTracking().CountAsync());
            // Assert.Equal(existingSkill, await context.Skills.AsNoTracking().FirstAsync());

            using (var logic = new UserLogic(userRepository, skillLogic, sparkLogicMock.Object, locationLogicMock.Object))
            {
                var result = await logic.AddSkillAsync(1, new SkillDTO() { Id = 1, Name = "Dancing" });

                Assert.Equal(ResponseLogic.SUCCESS, result);
            }
        }

        [Fact]
        public async void AddSkillAsyncWithId_GivenSkillAndUserExist_ReturnsSUCCESS()
        {
            var existingUser = new User() { Id = 1, Firstname = "IAlready", Surname = "Exist", Mail = "already@example.com", AzureUId = "existingAuzreUId" };
            var existingSkill = new Skill() { Id = 1, Name = "Dancing" };

            userRepository = new UserRepository(setupContextForIntegrationTests());
            skillRepository = new SkillRepository(context);
            projectRepository = new ProjectRepository(context);
            //locationRepository = new LocationRepository(context);

            //var locationLogic = new LocationLogic(locationRepository, userRepository, projectRepository);
            var skillLogic = new SkillLogic(skillRepository, userRepository, projectRepository);

            context.Users.Add(existingUser);
            context.Skills.Add(existingSkill);
            context.SaveChanges();
            context.Entry(existingSkill).State = EntityState.Detached;

            //SanityCheck
            Assert.Equal(1, await context.Users.CountAsync());
            Assert.Equal(existingUser, await context.Users.FirstAsync());
            Assert.Equal(1, await context.Skills.AsNoTracking().CountAsync());
            // Assert.Equal(existingSkill, await context.Skills.AsNoTracking().FirstAsync());

            using (var logic = new UserLogic(userRepository, skillLogic, sparkLogicMock.Object, locationLogicMock.Object))
            {
                var result = await logic.AddSkillAsync(1, 1);

                Assert.Equal(ResponseLogic.SUCCESS, result);
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
