﻿using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class CategoryRepositoryTests
    {
        private readonly CrowdSparkContext context;       
     
        public CategoryRepositoryTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<CrowdSparkContext>()
                .UseSqlite(connection);

            context = new CrowdSparkContext(builder.Options);
            context.Database.EnsureCreated();

            context.Database.BeginTransaction();            
        }

        public void Dispose()
        {
            context.Database.RollbackTransaction();
            context.Dispose();
        }

        [Fact]
        public async void CreateAsync_CheckIfCategoryExists()
        {
            var cat = new CategoryCreateDTO
            {
                Name = "Programming"
            };

            using (var repository = new CategoryRepository(context))
            {
                var id = await repository.CreateAsync(cat);
                Assert.NotNull(await context.Categories.FindAsync(id));
            }
        }

        [Fact]
        public async void CreateAsync_GivenNoChangesSaved_ReturnsDbUpdateException()
        {
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.Categories.Add(It.IsAny<Category>()));
            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(0);

            using (var repo = new CategoryRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateAsync(new CategoryCreateDTO() { }));
            }
        }

        [Fact]
        public async void CreateAsync_GivenSaveThrowsException_ReturnsDbUpdateException()
        {
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.Categories.Add(It.IsAny<Category>()));
            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ThrowsAsync(new System.Data.DataException("Error", default(Exception)));

            using (var repo = new CategoryRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateAsync(new CategoryCreateDTO() { }));
            }
        }

        [Fact]
        public async void CreateManyCategories_GetThemBackSorted()
        {
            var list = new List<CategoryCreateDTO>();

            list.Add( new CategoryCreateDTO{
                Name = "Programming"
            });

            list.Add(new CategoryCreateDTO{
                Name = "Mechanical"
            });

            list.Add(new CategoryCreateDTO
            {
                Name = "Sport"
            });

            list.Add(new CategoryCreateDTO
            {
                Name = "Outlife"
            });

            using (var repository = new CategoryRepository(context))
            {
                var id_list = new HashSet<int>();
                for (int i = 0; i< list.Count; i++ ){
                    id_list.Add( await repository.CreateAsync(list[i]) );
                }

                Assert.Equal(id_list.Count, list.Count);

                // Check to see if we get categories sorted.
                var sorted_list = await repository.ReadAsync();
                var listToCheck = new List<CategoryDTO>(sorted_list);

                Assert.Equal(list.Count, sorted_list.Count);
                list.Sort((CategoryCreateDTO x, CategoryCreateDTO y) => x.Name.CompareTo(y.Name));

                for (int i = 0; i < list.Count; i++)
                {
                    Assert.Equal(list[i].Name, listToCheck[i].Name);    
                }
            }
        }

        [Fact]
        public async void FindAsync_TrueifExists()
        {
            var cat = new CategoryCreateDTO
            {
                Name = "Programming"
            };

            using (var repository = new CategoryRepository(context))
            {
                var id = await repository.CreateAsync(cat);
                Assert.NotNull(await repository.FindAsync("Programming"));
            }
        }

        [Fact]
        public async void FindAsync_Trueifcaseincorrect()
        {
            var cat = new CategoryCreateDTO
            {
                Name = "Programming"
            };

            using (var repository = new CategoryRepository(context))
            {
                var id = await repository.CreateAsync(cat);
                Assert.NotNull(await repository.FindAsync("programming"));
            }
        }

        [Fact]
        public async void FindWildcardAsync_EmptyCollection()
        {
            var list = new List<CategoryCreateDTO>();

            list.Add(new CategoryCreateDTO
            {
                Name = "Programming"
            });

            list.Add(new CategoryCreateDTO
            {
                Name = "Mechanical"
            });

            list.Add(new CategoryCreateDTO
            {
                Name = "Sport"
            });

            list.Add(new CategoryCreateDTO
            {
                Name = "Outlife"
            });

            using (var repository = new CategoryRepository(context))
            {
                var id_list = new HashSet<int>();
                for (int i = 0; i < list.Count; i++)
                {
                    id_list.Add(await repository.CreateAsync(list[i]));
                }

                Assert.Equal(id_list.Count, list.Count);

                var sorted_list = await repository.FindWildcardAsync("Arduino");
                var listToCheck = new List<CategoryDTO>(sorted_list);

                Assert.Equal(0, listToCheck.Count);

                sorted_list = await repository.FindWildcardAsync("e");
                listToCheck = new List<CategoryDTO>(sorted_list);
                Assert.True(listToCheck.Count > 0);
            }
        }

        [Fact]
        public async void UpdateAsync_GivenCorrectId_ReturnsTrue()
        {
            var cat = new CategoryCreateDTO
            {
                Name = "Programming"
            };

            var catup = new CategoryDTO
            {
                Id = 1,
                Name = "Sports"
            };

            using (var repository = new CategoryRepository(context))
            {
                var id = await repository.CreateAsync(cat);
                Assert.True(await repository.UpdateAsync(catup));
            }
        }

        [Fact]
        public async void UpdateAsync_GivenCorrectId_ReturnsFalse()
        {
            var cat = new CategoryCreateDTO
            {
                Name = "Programming"
            };

            var catup = new CategoryDTO
            {
                Id = 2,
                Name = "Sports"
            };

            using (var repository = new CategoryRepository(context))
            {
                var id = await repository.CreateAsync(cat);
                Assert.False(await repository.UpdateAsync(catup));
            }
        }

        [Fact]
        public async void DeleteAsync_FalseIfNotExists()
        {
            var cat = new CategoryCreateDTO
            {
                Name = "Programming"
            };

            using (var repository = new CategoryRepository(context))
            {
                var id = await repository.CreateAsync(cat);
                Assert.False(await repository.DeleteAsync(id+1));
            }
        }

        [Fact]
        public async void DeleteAsync_TrueifExists_ActuallyDeletes()
        {
            var cat = new CategoryCreateDTO
            {
                Name = "Programming"
            };

            using (var repository = new CategoryRepository(context))
            {
                var id = await repository.CreateAsync(cat);
                Assert.True(await repository.DeleteAsync(id));
                Assert.Equal(0, await context.Categories.CountAsync());
            }
        }
    }
}
