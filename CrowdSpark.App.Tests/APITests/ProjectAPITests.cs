using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CrowdSpark.App.Models;

namespace CrowdSpark.App.Tests.APITests
{
    [TestClass]
    public class ProjectAPITests
    {
        Mock<IProjectAPI> projectAPI;
        public ProjectAPITests()
        {
            projectAPI = new Mock<IProjectAPI>();
        }

        [TestMethod]
        public void Test_returning_shit()
        {
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void Test_returning_crap()
        {
            Assert.AreEqual(1, 1);
        }
    }
}
