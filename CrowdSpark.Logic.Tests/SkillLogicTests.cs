using System;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Models;
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
    }
}
