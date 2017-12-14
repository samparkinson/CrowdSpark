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
            var loc = new LocationDTO
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
            var loc = new LocationDTO
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
            var cop = new LocationDTO
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            var ber = new LocationDTO
            {
                City = "Berlin",
                Country = "Germany"
            };

            var war = new LocationDTO
            {
                City = "Warszawa",
                Country = "Poland"
            };

            var krak = new LocationDTO
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
                var rese = new List<Location>(await repository.FindWildcardAsync("e"));
                Assert.Equal(2, rese.Count);

                // We dont want to optimize for alphabet.
                Assert.Equal("Copenhagen", rese[0].City);
                Assert.Equal("Denmark", rese[0].Country);
                Assert.Equal("Berlin", rese[1].City);
                Assert.Equal("Germany", rese[1].Country);

                var empty = new List<Location>(await repository.FindWildcardAsync("e", "Sweden"));
                Assert.Equal(0, rese.Count);
            }
        }

        [Fact]
        public async void ReadOrderedAsync_GivenLocations_ReturnsOrderedArray()
        {
            var cop = new LocationDTO
            {
                City = "Copenhagen",
                Country = "Denmark"
            };

            var ber = new LocationDTO
            {
                City = "Berlin",
                Country = "Germany"
            };

            var war = new LocationDTO
            {
                City = "Warszawa",
                Country = "Poland"
            };

            var krak = new LocationDTO
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

                var rese = new List<Location>( await repository.ReadOrderedAsync());
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
