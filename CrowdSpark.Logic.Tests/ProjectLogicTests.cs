using System;
using System.Linq;
using CrowdSpark.Logic;
using CrowdSpark.Models;
using CrowdSpark.Common;
using Moq;
using Xunit;

namespace CrowdSpark.Logic.Tests
{
    public class ProjectLogicTests
    {
        Mock<IProjectRepository> projectRepositoryMock;
        Mock<ILocationRepository> locationRepositoryMock;
        Mock<ISkillLogic> skillLogicMock;
        Mock<ISparkLogic> sparkLogicMock;
        Mock<ILocationLogic> locationLogicMock;

        public ProjectLogicTests()
        {
            projectRepositoryMock = new Mock<IProjectRepository>();
            locationRepositoryMock = new Mock<ILocationRepository>();
            skillLogicMock = new Mock<ISkillLogic>();
            sparkLogicMock = new Mock<ISparkLogic>();
            locationLogicMock = new Mock<ILocationLogic>();
        }

        public void Dispose()
        {
            
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

            using (var logic = new ProjectLogic(projectRepositoryMock.Object, locationRepositoryMock.Object, skillLogicMock.Object, sparkLogicMock.Object, locationLogicMock.Object))
            {
                var results = await logic.GetApprovedSparksAsync(3);

                Assert.Equal(3, results.Count());
                Assert.Contains(sparks[0], results);
                Assert.Contains(sparks[1], results);
                Assert.Contains(sparks[2], results);
            }
        }
    }
}
