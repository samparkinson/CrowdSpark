using System;
using System.Linq;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Logic.Tests
{
    public class SkillLogicTests
    {
        Mock<ISkillRepository> skillRepositoryMock;
        SkillRepository skillRepository;
        CrowdSparkContext context;
        Mock<IUserRepository> userRepositoryMock;
        Mock<IProjectRepository> projectRepositoryMock;

        public SkillLogicTests()
        {
            skillRepositoryMock = new Mock<ISkillRepository>();
            projectRepositoryMock = new Mock<IProjectRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
        }

        public void Dispose()
        {
            DisposeContextIfExists();
        }

        void DisposeContextIfExists()
        {
            if (skillRepository != null)
            {
                context?.Database?.RollbackTransaction();
                skillRepository.Dispose();
            }
        }

        #region UnitTests

        [Fact]
        public async void CreateAsync_GivenValidSkill_ReturnsSUCCESS()
        {
            var skillToCreate = new SkillCreateDTO
            {
                Name = "Skill"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToCreate.Name)).ReturnsAsync(default(SkillDTO));
            skillRepositoryMock.Setup(c => c.CreateAsync(skillToCreate)).ReturnsAsync(1);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(skillToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToCreate.Name));
                skillRepositoryMock.Verify(c => c.CreateAsync(skillToCreate));
            }
        }

        [Fact]
        public async void CreateAsync_GivenSkillExists_ReturnsSUCCESS()
        {
            var skillToCreate = new SkillCreateDTO
            {
                Name = "Skill"
            };

            var existingSkill = new SkillDTO
            {
                Id = 1,
                Name = "Skill"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToCreate.Name)).ReturnsAsync(existingSkill);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(skillToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToCreate.Name));
                skillRepositoryMock.Verify(c => c.CreateAsync(It.IsAny<SkillCreateDTO>()), Times.Never());
            }
        }

        [Fact]
        public async void CreateAsync_GivenNothingCreated_ReturnsERROR_CREATING()
        {
            var skillToCreate = new SkillCreateDTO
            {
                Name = "Skill"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToCreate.Name)).ReturnsAsync(default(SkillDTO));
            skillRepositoryMock.Setup(c => c.CreateAsync(skillToCreate)).ReturnsAsync(0);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(skillToCreate);

                Assert.Equal(ResponseLogic.ERROR_CREATING, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToCreate.Name));
                skillRepositoryMock.Verify(c => c.CreateAsync(skillToCreate));
            }
        }

        #endregion

        #region IntegreationTests

        [Fact]
        public async void SearchAsync_GivenMatchingSkills_ReturnsSkills()
        {
            var existingSkills = new Skill[]
            {
                new Skill() { Id = 1, Name = "Cooking" },
                new Skill() { Id = 2, Name = "Cooking Bacon"},
                new Skill() { Id = 3, Name = "Burning things while trying to cook"},
                new Skill() { Id = 4, Name = "Yoddeling"}
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.AddRange(existingSkills);
            context.SaveChanges();

            //Sanity Check
            Assert.Equal(4, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.SearchAsync("cook");

                Assert.Equal(3, results.Count());
                Assert.Equal(existingSkills[0].Name, results.ToArray()[0].Name);
                Assert.Equal(existingSkills[1].Name, results.ToArray()[1].Name);
                Assert.Equal(existingSkills[2].Name, results.ToArray()[2].Name);
            }
        }

        [Fact]
        public async void SearchAsync_GivenNoMatchingSkills_ReturnsEmptyEnumerable()
        {
            var existingSkills = new Skill[]
            {
                new Skill() { Id = 1, Name = "Cooking" },
                new Skill() { Id = 2, Name = "Cooking Bacon"},
                new Skill() { Id = 3, Name = "Burning things while trying to cook"},
                new Skill() { Id = 4, Name = "Yoddeling"}
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.AddRange(existingSkills);
            context.SaveChanges();

            //Sanity Check
            Assert.Equal(4, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.FindExactAsync("cooking");

                Assert.Equal(existingSkills[0].Name, result.Name);
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
