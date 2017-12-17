using CrowdSpark.Common;
using CrowdSpark.Logic;
using CrowdSpark.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace CrowdSpark.Web.Tests
{
    public class ProjectsControllerTests
    {
        Mock<IProjectLogic> projectLogic;
        Mock<IUserLogic> userLogic;
        Mock<ISparkLogic> sparkLogic;
        public ProjectsControllerTests()
        {
            projectLogic = new Mock<IProjectLogic>();
            userLogic = new Mock<IUserLogic>();
            sparkLogic = new Mock<ISparkLogic>();
        }

        [Fact]
        public async Task Get_given_no_existing_id_returns_NotFound()
        {
            projectLogic.Setup(r => r.GetAsync(42)).ReturnsAsync(default(ProjectSummaryDTO));

            var controller = new ProjectsController(projectLogic.Object, userLogic.Object, sparkLogic.Object);

            var response = await controller.Get(42);

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Get_given_existing_id_returns_OK_with_project()
        {
            var proj = new ProjectDTO { Id = 1 };

            projectLogic.Setup(r => r.GetDetailedAsync(1)).ReturnsAsync(proj);

            var controller = new ProjectsController(projectLogic.Object, userLogic.Object, sparkLogic.Object);

            var response = await controller.Get(1) as OkObjectResult;

            Assert.Equal(proj, response.Value);
        }

        [Fact]
        public async Task Post_given_project_returns_createdAt()
        {
            var project = new CreateProjectDTO { Title = "Title", Description = "Description" };

            userLogic.Setup(u => u.GetIdFromAzureUIdAsync("userid")).ReturnsAsync(1);
            projectLogic.Setup(p => p.CreateAsync(project, 1)).ReturnsAsync((ResponseLogic.SUCCESS, 1));

            var controller = new ProjectsController(projectLogic.Object, userLogic.Object, sparkLogic.Object)
            {
                GetUserId = () => "userid"
            };

            var response = await controller.Post(project);

            Assert.IsType<CreatedAtActionResult>(response);
        }

        [Fact]
        public async Task Put_given_mismatchedids_returnsBadRequest()
        {
            var project = new ProjectDTO { Id = 1, Title = "Foo", Description = "Bar", Creator = new UserDTO { Id = 1, Firstname = "Bob" } };

            userLogic.Setup(u => u.GetIdFromAzureUIdAsync("userid")).ReturnsAsync(2);

            var controller = new ProjectsController(projectLogic.Object, userLogic.Object, sparkLogic.Object)
            {
                GetUserId = () => "userid"
            };

            var response = await controller.Put(1, project);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Put_given_matchingids_returnsSUCCESS()
        {
            var project = new ProjectDTO { Id = 1, Title = "Foo", Description = "Bar", Creator = new UserDTO { Id = 1, Firstname = "Bob" } };

            projectLogic.Setup(r => r.UpdateAsync(project, 1)).ReturnsAsync(ResponseLogic.SUCCESS);
            userLogic.Setup(u => u.GetIdFromAzureUIdAsync("userid")).ReturnsAsync(1);

            var controller = new ProjectsController(projectLogic.Object, userLogic.Object, sparkLogic.Object)
            {
                GetUserId = () => "userid"
            };

            var response = await controller.Put(1, project);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Put_given_matchingids_butnotvalidproject_returnsNotFound()
        {
            var project = new ProjectDTO { Id = 1, Title = "Foo", Description = "Bar", Creator = new UserDTO { Id = 1, Firstname = "Bob", Surname = "Smith", Mail = "test@example.com" }, CreatedDate = System.DateTime.UtcNow };

            projectLogic.Setup(r => r.UpdateAsync(project, 1)).ReturnsAsync(ResponseLogic.NOT_FOUND);
            userLogic.Setup(u => u.GetIdFromAzureUIdAsync("userid")).ReturnsAsync(1);

            var controller = new ProjectsController(projectLogic.Object, userLogic.Object, sparkLogic.Object)
            {
                GetUserId = () => "userid"
            };

            var response = await controller.Put(1, project);

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_validID_ReturnsOK()
        {
            projectLogic.Setup(r => r.DeleteAsync(1, 1)).ReturnsAsync(ResponseLogic.SUCCESS);
            userLogic.Setup(u => u.GetIdFromAzureUIdAsync("userid")).ReturnsAsync(1);

            var controller = new ProjectsController(projectLogic.Object, userLogic.Object, sparkLogic.Object)
            {
                GetUserId = () => "userid"
            };

            var response = await controller.Delete(1);

            Assert.IsType<OkResult>(response);
        }
    }
}
