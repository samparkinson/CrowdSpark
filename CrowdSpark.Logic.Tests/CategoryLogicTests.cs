using System;
using System.Collections.Generic;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Logic;
using CrowdSpark.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Logic.Tests
{
    public class CategoryLogicTests
    {
        private readonly Mock<ICategoryRepository> categoryRepositoryMock;
        private CategoryRepository categoryRepository;
        private CrowdSparkContext context;
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
            if (categoryRepository != null)
            {
                context.Database.RollbackTransaction();
                categoryRepository.Dispose();
            }
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

        [Fact]
        public async void UpdateAsync_GivenCategoryExists_ReturnsSuccess()
        {
            var categoryToUpdate = new Category
            {
                Id = 1,
                Name = "Category"
            };

            var categoryToUpdateWithChanges = new Category
            {
                Id = 1,
                Name = "Category123"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToUpdateWithChanges.Id)).ReturnsAsync(categoryToUpdate);
            categoryRepositoryMock.Setup(c => c.UpdateAsync(categoryToUpdateWithChanges)).ReturnsAsync(true);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(categoryToUpdateWithChanges);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToUpdateWithChanges.Id));
                categoryRepositoryMock.Verify(c => c.UpdateAsync(categoryToUpdateWithChanges));
            }
        }

        [Fact]
        public async void UpdateAsync_GivenCategoryDoesNotExist_ReturnsNOT_FOUND()
        {
            var categoryToUpdateWithChanges = new Category
            {
                Id = 1,
                Name = "Category123"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToUpdateWithChanges.Id)).ReturnsAsync(default(Category));

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(categoryToUpdateWithChanges);

                Assert.Equal(ResponseLogic.NOT_FOUND, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToUpdateWithChanges.Id));
                categoryRepositoryMock.Verify(c => c.UpdateAsync(It.IsAny<Category>()), Times.Never());
            }
        }

        [Fact]
        public async void UpdateAsync_GivenErrorUpdating_ReturnsERROR_UPDATING()
        {
            var categoryToUpdate = new Category
            {
                Id = 1,
                Name = "Category"
            };

            var categoryToUpdateWithChanges = new Category
            {
                Id = 1,
                Name = "Category123"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToUpdateWithChanges.Id)).ReturnsAsync(categoryToUpdate);
            categoryRepositoryMock.Setup(c => c.UpdateAsync(categoryToUpdateWithChanges)).ReturnsAsync(false);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(categoryToUpdateWithChanges);

                Assert.Equal(ResponseLogic.ERROR_UPDATING, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToUpdateWithChanges.Id));
                categoryRepositoryMock.Verify(c => c.UpdateAsync(categoryToUpdateWithChanges));
            }
        }

        [Fact]
        public async void RemoveAsync_GivenCategoryExistsAndInNoProjects_ReturnsSuccess()
        {
            var categoryToDelete = new Category
            {
                Id = 1,
                Name = "Category"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToDelete.Id)).ReturnsAsync(categoryToDelete);
            categoryRepositoryMock.Setup(c => c.DeleteAsync(categoryToDelete.Id)).ReturnsAsync(true);
            projectRepositoryMock.Setup(p => p.ReadAsync()).ReturnsAsync(new ProjectSummaryDTO[] { });

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveAsync(categoryToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToDelete.Id));
                categoryRepositoryMock.Verify(c => c.DeleteAsync(categoryToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveAsync_GivenCategoryExistsAndInOneProject_ReturnsSuccess()
        {
            var categoryToDelete = new Category
            {
                Id = 1,
                Name = "Category"
            };

            var projectsArray = new ProjectSummaryDTO[]
            {
                new ProjectSummaryDTO { Title = "Project1", Category = categoryToDelete }
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToDelete.Id)).ReturnsAsync(categoryToDelete);
            categoryRepositoryMock.Setup(c => c.DeleteAsync(categoryToDelete.Id)).ReturnsAsync(true);
            projectRepositoryMock.Setup(p => p.ReadAsync()).ReturnsAsync(projectsArray);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveAsync(categoryToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToDelete.Id));
                categoryRepositoryMock.Verify(c => c.DeleteAsync(categoryToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveAsync_GivenCategoryExistsAndMoreThanOneProject_ReturnsSuccess()
        {
            var categoryToDelete = new Category
            {
                Id = 1,
                Name = "Category"
            };

            var projectsArray = new ProjectSummaryDTO[]
            {
                new ProjectSummaryDTO { Title = "Project1", Category = categoryToDelete },
                new ProjectSummaryDTO { Title = "Project2", Category = categoryToDelete}
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToDelete.Id)).ReturnsAsync(categoryToDelete);
            projectRepositoryMock.Setup(p => p.ReadAsync()).ReturnsAsync(projectsArray);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveAsync(categoryToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToDelete.Id));
                categoryRepositoryMock.Verify(c => c.DeleteAsync(It.IsAny<int>()), Times.Never());
                projectRepositoryMock.Verify(p => p.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveAsync_GivenDatabaseError_ReturnsERROR_DELETING()
        {
            var categoryToDelete = new Category
            {
                Id = 1,
                Name = "Category"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToDelete.Id)).ReturnsAsync(categoryToDelete);
            categoryRepositoryMock.Setup(c => c.DeleteAsync(categoryToDelete.Id)).ReturnsAsync(false);
            projectRepositoryMock.Setup(p => p.ReadAsync()).ReturnsAsync(new ProjectSummaryDTO[] { });

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveAsync(categoryToDelete);

                Assert.Equal(ResponseLogic.ERROR_DELETING, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToDelete.Id));
                categoryRepositoryMock.Verify(c => c.DeleteAsync(categoryToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveAsync_GivenCategoryDoesNotExist_ReturnsNOT_FOUND()
        {
            var categoryToDelete = new Category
            {
                Id = 1,
                Name = "Category"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToDelete.Id)).ReturnsAsync(default(Category));

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveAsync(categoryToDelete);

                Assert.Equal(ResponseLogic.NOT_FOUND, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToDelete.Id));
                categoryRepositoryMock.Verify(c => c.DeleteAsync(It.IsAny<int>()), Times.Never());
                projectRepositoryMock.Verify(p => p.ReadAsync(), Times.Never());
            }
        }

        [Fact]
        public async void DeleteAsync_GivenDatabaseError_ReturnsERROR_DELETING()
        {
            var categoryToDelete = new Category
            {
                Id = 1,
                Name = "Category"
            };

            categoryRepositoryMock.Setup(c => c.FindAsync(categoryToDelete.Id)).ReturnsAsync(categoryToDelete);
            categoryRepositoryMock.Setup(c => c.DeleteAsync(categoryToDelete.Id)).ReturnsAsync(false);

            using (var logic = new CategoryLogic(categoryRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.DeleteAsync(categoryToDelete.Id);

                Assert.Equal(ResponseLogic.ERROR_DELETING, response);
                categoryRepositoryMock.Verify(c => c.FindAsync(categoryToDelete.Id));
                categoryRepositoryMock.Verify(c => c.DeleteAsync(categoryToDelete.Id));
            }
        }

        #endregion

        #region IntegrationTests

        [Fact]
        public async void DeleteAsync_GivenCategoryExistsAndInNoProjects_ReturnsSuccess()
        {
            var categoryToDelete = new Category
            {
                Id = 1,
                Name = "Category"
            };

            categoryRepository = new CategoryRepository(setupContextForIntegrationTests());

            context.Categories.Add(categoryToDelete);
            context.SaveChanges();

            //Sanity Check
            Assert.NotNull(context.Categories.Find(categoryToDelete.Id));
            Assert.Empty(await context.Projects.ToArrayAsync());

            using (var logic = new CategoryLogic(categoryRepository, projectRepositoryMock.Object))
            {
                var response = await logic.DeleteAsync(categoryToDelete.Id);

                Assert.Equal(ResponseLogic.SUCCESS, response);
            }
        }

        [Fact]
        public async void DeleteAsync_GivenCategoryExistsAndInProjects_ReturnsSuccess()
        {
            var categoryToDelete = new Category
            {
                Id = 1,
                Name = "Category"
            };

            var projects = new Project[]
            {
                new Project{ Title = "Project1", Category = categoryToDelete, CreatedDate = DateTime.Now, Description = "abcd"},
                new Project{ Title = "Project2", Category = categoryToDelete, CreatedDate = DateTime.Now, Description = "abcd"}
            };

            categoryRepository = new CategoryRepository(setupContextForIntegrationTests());

            context.Categories.Add(categoryToDelete);
            context.Projects.AddRange(projects);
            context.SaveChanges();

            //Sanity Check
            Assert.NotNull(context.Categories.Find(categoryToDelete.Id));
            Assert.Equal(2, (await context.Projects.ToArrayAsync()).Length);

            using (var logic = new CategoryLogic(categoryRepository, projectRepositoryMock.Object))
            {
                var response = await logic.DeleteAsync(categoryToDelete.Id);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                foreach (var project in await context.Projects.ToArrayAsync())
                {
                    Assert.Null(project.Category);
                }
            }
        }

        private CrowdSparkContext setupContextForIntegrationTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<CrowdSparkContext>()
                .UseSqlite(connection);

            context = new CrowdSparkContext(builder.Options);
            context.Database.EnsureCreated();
            context.Database.BeginTransaction();

            return context;
        }

        #endregion
    }
}