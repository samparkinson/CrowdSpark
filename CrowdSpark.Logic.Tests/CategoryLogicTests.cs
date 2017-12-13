using System;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Logic;
using CrowdSpark.Models;
using Moq;
using Xunit;

namespace CrowdSpark.Logic.Tests
{
    public class CategoryLogicTests
    {
        private readonly Mock<ICategoryRepository> categoryRepositoryMock;
        private readonly CategoryRepository categoryRepository;
        private readonly Mock<IProjectRepository> _projectRepository;

        public CategoryLogicTests()
        {
            categoryRepositoryMock = new Mock<ICategoryRepository>();
            _projectRepository = new Mock<IProjectRepository>();
        }

        public void Dispose()
        {
            DisposeContextIfExists();
        }

        void DisposeContextIfExists()
        {
            if (categoryRepository != null) categoryRepository.Dispose();
        }

        #region UnitTests

        [Fact]
        public async void CreateAsync_GivenValidCategory_ReturnsSUCCESS()
        {
            var categoryToCreate = new CategoryCreateDTO
            {
                Name = "New Category"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToCreate.Name)).ReturnsAsync(default(Category));
            categoryRepositoryMock.Setup(c => c.CreateAsync(categoryToCreate)).ReturnsAsync(1);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, _projectRepository.Object))
            {
                var response = await logic.CreateAsync(categoryToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToCreate.Name));
                categoryRepositoryMock.Verify(c => c.CreateAsync(categoryToCreate));
            }
        }

        [Fact]
        public async void CreateAsync_GivenCategoryExists_ReturnsSUCCESS()
        {
            var categoryToCreate = new CategoryCreateDTO
            {
                Name = "New Category"
            };

            var existingCategory = new Category
            {
                Id = 1,
                Name = "New Category"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToCreate.Name)).ReturnsAsync(existingCategory);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, _projectRepository.Object))
            {
                var response = await logic.CreateAsync(categoryToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToCreate.Name));
                categoryRepositoryMock.Verify(c => c.CreateAsync(It.IsAny<CategoryCreateDTO>()), Times.Never());
            }
        }

        #endregion

        #region IntegrationTests

        #endregion
    }
}
