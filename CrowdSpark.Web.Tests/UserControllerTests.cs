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

        //[Fact]
        //public async Task Delete_User_Notfound_ReturnsNOTFound()
        //{
        //    TODO, Need to mock out the thing that gets user ID in this case then call this.Commenting out until this is done

        //   var userLogic = new Mock<IUserLogic>();

        //    var controller = new UsersController(userLogic.Object);

        //    userLogic.Setup(u => u.DeleteAsync(1)).ReturnsAsync(ResponseLogic.NOT_FOUND);

        //    var response = await controller.Delete();

        //    Assert.IsType<NotFoundResult>(response);
        //}
    }
}
