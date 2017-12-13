using System;
using System.Collections.Generic;
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
        private readonly Mock<IProjectRepository> projectRepositoryMock;

        public CategoryLogicTests()
        {
            categoryRepositoryMock = new Mock<ICategoryRepository>();
            projectRepositoryMock = new Mock<IProjectRepository>();
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

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
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

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(categoryToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToCreate.Name));
                categoryRepositoryMock.Verify(c => c.CreateAsync(It.IsAny<CategoryCreateDTO>()), Times.Never());
            }
        }

        [Fact]
        public async void CreateAsync_GivenNothingCreated_ReturnsERROR_CREATING()
        {
            var categoryToCreate = new CategoryCreateDTO
            {
                Name = "New Category"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToCreate.Name)).ReturnsAsync(default(Category));
            categoryRepositoryMock.Setup(c => c.CreateAsync(categoryToCreate)).ReturnsAsync(0);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(categoryToCreate);

                Assert.Equal(ResponseLogic.ERROR_CREATING, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToCreate.Name));
                categoryRepositoryMock.Verify(c => c.CreateAsync(categoryToCreate));
            }
        }

        [Fact]
        public async void FindAsync_GivenCategoriesExist_ReturnsEnumerableCategories()
        {
            var categoriesToReturn = new Category[]
            {
                new Category { Id = 1, Name = "This is the first" },
                new Category { Id = 2, Name = "This is the second" },
                new Category { Id = 3, Name = "This is the third" }
            };

            categoryRepositoryMock.Setup(c => c.FindWildcardAsync("this")).ReturnsAsync(categoriesToReturn);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.FindAsync("this");

                Assert.Equal(categoriesToReturn, results);
                categoryRepositoryMock.Verify(c => c.FindWildcardAsync("this"));
            }
        }

        [Fact]
        public async void FindExactAsync_GivenCategoryExists_ReturnsCategory()
        {
            var categoryToReturn = new Category { Id = 1, Name = "This is the first" };

            categoryRepositoryMock.Setup(c => c.FindAsync("This is the first")).ReturnsAsync(categoryToReturn);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.FindExactAsync("This is the first");

                Assert.Equal(categoryToReturn, result);
                categoryRepositoryMock.Verify(c => c.FindAsync("This is the first"));
            }
        }

        [Fact]
        public async void GetAsync_GivenCategoriesExist_ReturnsEnumerableCategories()
        {
            var categoriesToReturn = new Category[]
            {
                new Category { Id = 1, Name = "This is the first" },
                new Category { Id = 2, Name = "This is the second" },
                new Category { Id = 3, Name = "This is the third" }
            };

            categoryRepositoryMock.Setup(c => c.ReadAsync()).ReturnsAsync(categoriesToReturn);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.GetAsync();

                Assert.Equal(categoriesToReturn, results);
                categoryRepositoryMock.Verify(c => c.ReadAsync());
            }
        }

        [Fact]
        public async void GetAsync_GivenExistingCategoryId_ReturnsCategory()
        {
            var categoryToReturn = new Category { Id = 3, Name = "This is a Category" };

            categoryRepositoryMock.Setup(c => c.FindAsync(3)).ReturnsAsync(categoryToReturn);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.GetAsync(3);

                Assert.Equal(categoryToReturn, result);
                categoryRepositoryMock.Verify(c => c.FindAsync(3));
            }
        }

        #endregion

        #region IntegrationTests

        #endregion
    }
}