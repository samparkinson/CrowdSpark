using System;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class UserRepositoryTests
    {
        public UserRepositoryTests()
        {
            // Setup Seed Here
            // If using real DB, begin transaction
        }

        public void Dispose()
        {
            // If using real DB rollback transaction
        }

        [Fact]
        public void CreateAsync_GivenValidUser_ReturnsNewUserID()
        {
            // MOCK DB
        }

        [Fact]
        public void CreateAsync_GivenDatabaseSaveError_ReturnsDbUpdateException()
        {
            // MockDB
        }

        [Fact]
        public void CreateAsync_GivenValidUser_SuccesfullyCreatesDBRecord()
        {
            // Create in memory or SQLlite DB
        }
    }
}
