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
    public class UsersControllerTests
    {

        [Fact]
        public async Task Delete_User_Notfound_ReturnsNOTFound()
        {
            var repository = new Mock<IUserLogic>();

            var controller = new UsersController(repository.Object);
            repository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(ResponseLogic.NOT_FOUND);

            var response = await controller.Delete();

            Assert.IsType<NotFoundResult>(response);
        }
    }
}
