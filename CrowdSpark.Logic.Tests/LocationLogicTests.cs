using System;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Models;
using Moq;
using Xunit;

namespace CrowdSpark.Logic.Tests
{
    public class LocationLogicTests
    {
        private readonly Mock<ILocationRepository> locationRepositoryMock;
        private LocationRepository locationRepository;
        private CrowdSparkContext context;
        private readonly Mock<IProjectRepository> projectRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;

        public LocationLogicTests()
        {
            locationRepositoryMock = new Mock<ILocationRepository>();
            projectRepositoryMock = new Mock<IProjectRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
        }

        public void Dispose()
        {
            DisposeContextIfExists();
        }

        void DisposeContextIfExists()
        {
            if (locationRepository != null)
            {
                context.Database.RollbackTransaction();
                locationRepository.Dispose();
            }
        }

        #region UnitTests

        [Fact]
        public async void CreateAsync_GivenValidLocation_ReturnsSUCCESS()
        {
            var locationToCreate = new LocationDTO
            {
                City = "Sydney",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToCreate.City, locationToCreate.Country)).ReturnsAsync(default(Location));
            locationRepositoryMock.Setup(c => c.CreateAsync(locationToCreate)).ReturnsAsync(1);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object,  projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(locationToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToCreate.City, locationToCreate.Country));
                locationRepositoryMock.Verify(c => c.CreateAsync(locationToCreate));
            }
        }

        [Fact]
        public async void CreateAsync_GivenLocationyExists_ReturnsSUCCESS()
        {
            var locationToCreate = new LocationDTO
            {
                City = "Sydney",
                Country = "Australia"
            };

            var existingLocation = new Location
            {
                Id = 1,
                City = "Sydney",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToCreate.City, locationToCreate.Country)).ReturnsAsync(existingLocation);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(locationToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToCreate.City, locationToCreate.Country));
                locationRepositoryMock.Verify(c => c.CreateAsync(It.IsAny<LocationDTO>()), Times.Never());
            }
        }

        [Fact]
        public async void CreateAsync_GivenNothingCreated_ReturnsERROR_CREATING()
        {
            var locationToCreate = new LocationDTO
            {
                City = "Sydney",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToCreate.City, locationToCreate.Country)).ReturnsAsync(default(Location));
            locationRepositoryMock.Setup(c => c.CreateAsync(locationToCreate)).ReturnsAsync(0);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(locationToCreate);

                Assert.Equal(ResponseLogic.ERROR_CREATING, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToCreate.City, locationToCreate.Country));
                locationRepositoryMock.Verify(c => c.CreateAsync(locationToCreate));
            }
        }

        [Fact]
        public async void GetAsync_GivenLocationsExist_ReturnsEnumerableLocations()
        {
            var locationsToReturn = new Location[]
            {
                new Location { Id = 1, City = "Sydney", Country = "Australia" },
                new Location { Id = 2, City = "Melbourne", Country = "Australia" },
                new Location { Id = 3, City = "Brisbane", Country = "Australia" }
            };

            locationRepositoryMock.Setup(c => c.ReadAsync()).ReturnsAsync(locationsToReturn);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.GetAsync();

                Assert.Equal(locationsToReturn, results);
                locationRepositoryMock.Verify(c => c.ReadAsync());
            }
        }

        [Fact]
        public async void GetAsync_GivenExistingLocationId_ReturnsLocation()
        {
            var locationToReturn = new Location { Id = 3, City = "Brisbane", Country = "Australia" };

            locationRepositoryMock.Setup(c => c.FindAsync(3)).ReturnsAsync(locationToReturn);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.GetAsync(3);

                Assert.Equal(locationToReturn, result);
                locationRepositoryMock.Verify(c => c.FindAsync(3));
            }
        }

        [Fact]
        public async void FindAsyncWithCityAndCountry_GivenLocationsExist_ReturnsEnumerableLocations()
        {
            var locationsToReturn = new Location[]
            {
                new Location { Id = 1, City = "Sydney", Country = "Australia" },
                new Location { Id = 2, City = "Melbourne", Country = "Australia" },
                new Location { Id = 3, City = "Brisbane", Country = "Australia" }
            };

            locationRepositoryMock.Setup(c => c.FindWildcardAsync("e", "Australia")).ReturnsAsync(locationsToReturn);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.FindAsync("e", "Australia");

                Assert.Equal(locationsToReturn, results);
                locationRepositoryMock.Verify(c => c.FindWildcardAsync("e", "Australia"));
            }
        }

        [Fact]
        public async void FindAsyncWithCity_GivenLocationsExist_ReturnsEnumerableLocations()
        {
            var locationsToReturn = new Location[]
            {
                new Location { Id = 1, City = "Sydney", Country = "Australia" },
                new Location { Id = 2, City = "Melbourne", Country = "Australia" },
                new Location { Id = 3, City = "Brisbane", Country = "Australia" }
            };

            locationRepositoryMock.Setup(c => c.FindWildcardAsync("e")).ReturnsAsync(locationsToReturn);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.FindAsync("e");

                Assert.Equal(locationsToReturn, results);
                locationRepositoryMock.Verify(c => c.FindWildcardAsync("e"));
            }
        }


        #endregion
    }
}
