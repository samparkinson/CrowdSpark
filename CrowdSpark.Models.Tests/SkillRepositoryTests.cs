using System;
using CrowdSpark.Common;
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
        public async void CreateAsync_GivenValidSkill_ReturnsNewSkillID()
        {
            var skill = new SkillCreateDTO
            {
                Name = "Cooking"
            };

            using (var repository = new SkillRepository(context))
            {
                var id = await repository.CreateAsync(skill);
                var foundSkill = await context.Skills.FirstAsync();
                Assert.Equal(foundSkill.Id, id);
            }
        }

        [Fact]
        public async void CreateAsync_GivenTwoValidSkills_ReturnsTwoNewSkillIDs()
        {
            var skill1 = new SkillCreateDTO
            {
                Name = "Cooking"
            };
            var skill2 = new SkillCreateDTO
            {
                Name = "Dancing"
            };

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
