using CrowdSpark.Common;
using CrowdSpark.Logic;
using CrowdSpark.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CrowdSpark.Web.Tests
{
    public class ProjectsControllerTests
    {
        [Fact]
        public async Task Get_given_no_existing_id_returns_NotFound()
        {
            var repository = new Mock<IProjectLogic>();
            repository.Setup(r => r.GetAsync(42)).ReturnsAsync(default(ProjectSummaryDTO));

            var controller = new ProjectsController(repository.Object);

            var response = await controller.Get(42);

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Get_given_existing_id_returns_OK_with_project()
        {
            var proj = new ProjectSummaryDTO { Id = 1 };
            var repository = new Mock<IProjectLogic>();
            repository.Setup(r => r.GetAsync(1)).ReturnsAsync(proj);

            var controller = new ProjectsController(repository.Object);

            var response = await controller.Get(1) as OkObjectResult;

            Assert.Equal(proj, response.Value);
        }

        [Fact]
        public async Task Post_given_project_returns_createdAt()
        {
            var project = new ProjectDTO { Id = 1 };

            var repository = new Mock<IProjectLogic>();

            var controller = new ProjectsController(repository.Object);

            var response = await controller.Post(project);

            Assert.IsType<CreatedAtActionResult>(response);
        }

        [Fact]
        public async Task Put_given_mismatchedids_returnsBadRequest()
        {
            var project = new ProjectSummaryDTO { Id = 1 };

            var repository = new Mock<IProjectLogic>();

            var controller = new ProjectsController(repository.Object);

            var response = await controller.Put(2, project);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Put_given_matchingids_returnsSUCCESS()
        {
            var project = new ProjectSummaryDTO { Id = 1 };

            var repository = new Mock<IProjectLogic>();

            var controller = new ProjectsController(repository.Object);
            repository.Setup(r => r.UpdateAsync(project)).ReturnsAsync(ResponseLogic.SUCCESS);

            var response = await controller.Put(1, project);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Put_given_matchingids_butnotvalidproject_returnsNotFound()
        {
            var project = new ProjectSummaryDTO { Id = 1 };

            var repository = new Mock<IProjectLogic>();

            var controller = new ProjectsController(repository.Object);
            repository.Setup(r => r.UpdateAsync(project)).ReturnsAsync(ResponseLogic.NOT_FOUND);

            var response = await controller.Put(1, project);

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_validID_ReturnsOK()
        {
            var repository = new Mock<IProjectLogic>();

            var controller = new ProjectsController(repository.Object);
            repository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(ResponseLogic.SUCCESS);

            var response = await controller.Delete(1);

            Assert.IsType<OkResult>(response);
        }
    }
}
