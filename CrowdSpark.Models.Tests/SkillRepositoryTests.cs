using System;
using CrowdSpark.Entitites;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class SkillRepositoryTests
    {
        private readonly CrowdSparkContext context;

        public SkillRepositoryTests()
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
        public async void CreateAsync_GivenValidSkill_ReturnsNewSkillID()
        {
            var skill = new Skill
            {
                Id = -1,
                Name = "Cooking"
            };
            //var contextMock = new Mock<ICrowdSparkContext>();

            //contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(1);
            //contextMock.Setup(c => c.Users.Add(It.IsAny<User>())).Returns(1);

            using (var repository = new SkillRepository(context))
            {
                var id = await repository.CreateAsync(skill);
                var foundSkill = await context.Skills.FirstAsync();
                Assert.Equal(foundSkill.Id, id);
            }
        }

        public async void CreateAsync_GivenTwoValidSkills_ReturnsTwoNewSkillIDs()
        {
            var skill1 = new Skill
            {
                Id = -1,
                Name = "Cooking"
            };
            var skill2 = new Skill
            {
                Id = -1,
                Name = "Dancing"
            };
            //var contextMock = new Mock<ICrowdSparkContext>();

            //contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(1);
            //contextMock.Setup(c => c.Users.Add(It.IsAny<User>())).Returns(1);

            using (var repository = new SkillRepository(context))
            {
                var id1 = await repository.CreateAsync(skill1);
                var id2 = await repository.CreateAsync(skill2);

                var foundSkill1 = await context.Skills.FirstAsync();
                var foundSkill2 = (await context.Skills.ToArrayAsync())[1];

                Assert.Equal(foundSkill1.Id, id1);
                Assert.Equal(foundSkill2.Id, id2);
            }
        }
    }
}
