using System;
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

            //SEED IN HERE IF YOU WANT

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
        /* Move to Logic test
        [Fact]
        public async void CreateAsyncTwice_ReturnsSameID()
        {
            var cat = new CategoryCreateDTO
            {
                Name = "Programming"
            };

            using (var repository = new CategoryRepository(context))
            {
                var id = await repository.CreateAsync(cat);
                var id2 = await repository.CreateAsync(cat);
                Assert.Equal(id, id2);
            }
        }
        */

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
                var listToCheck = new List<Category>(sorted_list);

                Assert.Equal(list.Count, sorted_list.Count);
                list.Sort((CategoryCreateDTO x, CategoryCreateDTO y) => x.Name.CompareTo(y.Name));

                for (int i = 0; i < list.Count; i++)
                {
                    Assert.Equal(list[i].Name, listToCheck[i].Name);    
                }
            }
        }
    }
}
