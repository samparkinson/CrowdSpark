using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class LocationRepositoryTests
    {
        private readonly CrowdSparkContext context;       

        public LocationRepositoryTests()
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
        public async void CreateAsync_GivenValidLocation_ReturnsNewLocationID()
        {
            var loc = new LocationCreateDTO
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            using (var repository = new LocationRepository(context))
            {
                var id = await repository.CreateAsync(loc);
                Assert.Equal((await context.Locations.FirstAsync()).Id, id);
            }
        }

        [Fact]
        public async void CreateAsync_GivenValidLocationDeletesIt_ReturnsSuccess()
        {
            var loc = new LocationCreateDTO
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            using (var repository = new LocationRepository(context))
            {
                var id = await repository.CreateAsync(loc);
                Assert.Equal((await context.Locations.FirstAsync()).Id, id);
                Assert.True(await repository.DeleteAsync(id));
            }
        }

        [Fact]
        public async void CreateAsync_GivenNoChangesSaved_ReturnsDbUpdateException()
        {
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.Locations.Add(It.IsAny<Location>()));
            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(0);

            using (var repo = new LocationRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateAsync(new LocationCreateDTO() { }));
            }
        }

        [Fact]
        public async void CreateAsync_GivenSaveThrowsException_ReturnsDbUpdateException()
        {
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.Locations.Add(It.IsAny<Location>()));
            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ThrowsAsync(new System.Data.DataException("Error", default(Exception)));

            using (var repo = new LocationRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateAsync(new LocationCreateDTO() { }));
            }
        }

        [Fact]
        public async void ReadAsync_GivenNoLocations_ReturnsEmptyEnumerable()
        {
            //SanityCheck
            Assert.Empty(await context.Locations.ToArrayAsync());

            using (var repo = new LocationRepository(context))
            {
                var result = await repo.ReadAsync();

                Assert.Empty(result);
            }
        }

        [Fact]
        public async void ReadAsync_GivenLocationsExist_ReturnsLocations()
        {
            var locations = new Location[]
            {
                new Location() {Id = 1, City = "Sydney", Country = "Australia" },
                new Location() {Id = 2, City = "Brisbane", Country = "Australia" }
            };

            context.Locations.AddRange(locations);
            context.SaveChanges();

            //SanityCheck
            Assert.Equal(2, await context.Locations.CountAsync());

            using (var repo = new LocationRepository(context))
            {
                var results = await repo.ReadAsync();

                Assert.Equal(2, results.Count);
            }
        }

        [Fact]
        public async void UpdateAsync_GivenBadIDReturnFalse()
        {
            var loc = new LocationCreateDTO
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            var locupdate = new LocationDTO
            {
                Id = 2,
                City = "Copenhagen",
                Country = "Denmark"
            };

            using (var repository = new LocationRepository(context))
            {
                var id = await repository.CreateAsync(loc);
                
                Assert.False(await repository.UpdateAsync(locupdate));
            }
        }

        [Fact]
        public async void UpdateAsync_GivenGoodIDReturnTrue()
        {
            var loc = new LocationCreateDTO
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            var locupdate = new LocationDTO
            {
                Id = 1,
                City = "Copenhagen",
                Country = "Denmark"
            };

            using (var repository = new LocationRepository(context))
            {
                var id = await repository.CreateAsync(loc);

                Assert.True(await repository.UpdateAsync(locupdate));
            }
        }

        [Fact]
        public async void DeleteAsync_GivenInvalidLocationId_ReturnsFailed()
        {
            using (var repository = new LocationRepository(context))
            {
                Assert.False(await repository.DeleteAsync(-321));
            }
        }

        [Fact]
        public async void FindWildCardAsyncAsync_GivenStrings_ReturnsLocationsOrNull()
        {
            var cop = new LocationCreateDTO
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            var ber = new LocationCreateDTO
            {
                City = "Berlin",
                Country = "Germany"
            };

            var war = new LocationCreateDTO
            {
                City = "Warszawa",
                Country = "Poland"
            };

            var krak = new LocationCreateDTO
            {
                City = "Krakow",
                Country = "Poland"
            };

            using (var repository = new LocationRepository(context))
            {
                await repository.CreateAsync(cop);
                await repository.CreateAsync(ber);
                await repository.CreateAsync(war);
                await repository.CreateAsync(krak);

                var res = await repository.FindWildcardAsync("z");
                foreach (var item in res)
                {
                    Assert.Equal(war.City, item.City);
                }
                var rese = new List<LocationDTO>(await repository.FindWildcardAsync("e"));
                Assert.Equal(2, rese.Count);

                // We dont want to optimize for alphabet.
                Assert.Equal("Copenhagen", rese[0].City);
                Assert.Equal("Denmark", rese[0].Country);
                Assert.Equal("Berlin", rese[1].City);
                Assert.Equal("Germany", rese[1].Country);

                var expectedEmpty = new List<LocationDTO>(await repository.FindWildcardAsync("e", "Sweden"));
                Assert.Equal(0, expectedEmpty.Count);
            }
        }

        [Fact]
        public async void ReadOrderedAsync_GivenLocations_ReturnsOrderedArray()
        {
            var cop = new LocationCreateDTO
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            var ber = new LocationCreateDTO
            {
                City = "Berlin",
                Country = "Germany"
            };

            var war = new LocationCreateDTO
            {
                City = "Warszawa",
                Country = "Poland"
            };

            var krak = new LocationCreateDTO
            {
                City = "Krakow",
                Country = "Poland"
            };

            using (var repository = new LocationRepository(context))
            {
                await repository.CreateAsync(war);
                await repository.CreateAsync(krak);
                await repository.CreateAsync(cop);
                await repository.CreateAsync(ber);

                var rese = new List<LocationDTO>( await repository.ReadOrderedAsync());
                Assert.Equal(4, rese.Count);

                // We dont want to optimize for alphabet.
                Assert.Equal("Copenhagen", rese[0].City);
                Assert.Equal("Berlin", rese[1].City);
                Assert.Equal("Krakow", rese[2].City);
                Assert.Equal("Warszawa", rese[3].City);

            }
        }
    }
}
