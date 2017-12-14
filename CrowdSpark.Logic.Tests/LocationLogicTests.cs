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

        [Fact]
        public async void FindExactAsync_GivenLocationExists_ReturnsLocation()
        {
            var locationToReturn = new Location { Id = 1, City = "Sydney", Country = "Australia" };

            locationRepositoryMock.Setup(c => c.FindAsync("Sydney", "Australia")).ReturnsAsync(locationToReturn);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.FindExactAsync("Sydney", "Australia");

                Assert.Equal(locationToReturn, result);
                locationRepositoryMock.Verify(c => c.FindAsync("Sydney", "Australia"));
            }
        }

        [Fact]
        public async void UpdateAsync_GivenLocationExists_ReturnsSuccess()
        {
            var locationToUpdate = new Location
            {
                Id = 1,
                City = "Sydne",
                Country = "Australia"
            };

            var locationToUpdateWithChanges = new Location
            {
                Id = 1,
                City = "Sydney",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToUpdateWithChanges.Id)).ReturnsAsync(locationToUpdate);
            locationRepositoryMock.Setup(c => c.UpdateAsync(locationToUpdateWithChanges)).ReturnsAsync(true);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(locationToUpdateWithChanges);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToUpdateWithChanges.Id));
                locationRepositoryMock.Verify(c => c.UpdateAsync(locationToUpdateWithChanges));
            }
        }

        [Fact]
        public async void UpdateAsync_GivenLocationDoesNotExist_ReturnsNOT_FOUND()
        {
            var locationToUpdateWithChanges = new Location
            {
                Id = 1,
                City = "Sydney",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToUpdateWithChanges.Id)).ReturnsAsync(default(Location));

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(locationToUpdateWithChanges);

                Assert.Equal(ResponseLogic.NOT_FOUND, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToUpdateWithChanges.Id));
                locationRepositoryMock.Verify(c => c.UpdateAsync(It.IsAny<Location>()), Times.Never());
            }
        }

        [Fact]
        public async void UpdateAsync_GivenErrorUpdating_ReturnsERROR_UPDATING()
        {
            var locationToUpdate = new Location
            {
                Id = 1,
                City = "Sydne",
                Country = "Australia"
            };

            var locationToUpdateWithChanges = new Location
            {
                Id = 1,
                City = "Sydney",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToUpdateWithChanges.Id)).ReturnsAsync(locationToUpdate);
            locationRepositoryMock.Setup(c => c.UpdateAsync(locationToUpdateWithChanges)).ReturnsAsync(false);

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(locationToUpdateWithChanges);

                Assert.Equal(ResponseLogic.ERROR_UPDATING, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToUpdateWithChanges.Id));
                locationRepositoryMock.Verify(c => c.UpdateAsync(locationToUpdateWithChanges));
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenLocationExistsAndInNoProjectsOrUsers_ReturnsSuccess()
        {
            var locationToDelete = new Location
            {
                Id = 1,
                City = "Sydne",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToDelete.Id)).ReturnsAsync(locationToDelete);
            locationRepositoryMock.Setup(c => c.DeleteAsync(locationToDelete.Id)).ReturnsAsync(true);
            projectRepositoryMock.Setup(p => p.ReadAsync()).ReturnsAsync(new ProjectSummaryDTO[] { });
            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(new UserDTO[] { });

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(locationToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToDelete.Id));
                locationRepositoryMock.Verify(c => c.DeleteAsync(locationToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadAsync());
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenLocationExistsAndInOneProjectOrUser_ReturnsSuccess()
        {
            var locationToDelete = new Location
            {
                Id = 1,
                City = "Sydne",
                Country = "Australia"
            };

            var projectsArray = new ProjectSummaryDTO[]
            {
                new ProjectSummaryDTO { Title = "Project1", LocationId = locationToDelete.Id }
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToDelete.Id)).ReturnsAsync(locationToDelete);
            locationRepositoryMock.Setup(c => c.DeleteAsync(locationToDelete.Id)).ReturnsAsync(true);
            projectRepositoryMock.Setup(p => p.ReadAsync()).ReturnsAsync(projectsArray);
            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(new UserDTO[] { });

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(locationToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToDelete.Id));
                locationRepositoryMock.Verify(c => c.DeleteAsync(locationToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadAsync());
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenLocationExistsInMoreThanOneProjectAndUser_ReturnsSuccess()
        {
            var locationToDelete = new Location
            {
                Id = 1,
                City = "Sydne",
                Country = "Australia"
            };

            var projectsArray = new ProjectSummaryDTO[]
            {
                new ProjectSummaryDTO { Title = "Project1", LocationId = locationToDelete.Id },
                new ProjectSummaryDTO { Title = "Project2", LocationId = locationToDelete.Id }
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToDelete.Id)).ReturnsAsync(locationToDelete);
            projectRepositoryMock.Setup(p => p.ReadAsync()).ReturnsAsync(projectsArray);
            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(new UserDTO[] { });

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(locationToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToDelete.Id));
                locationRepositoryMock.Verify(c => c.DeleteAsync(It.IsAny<int>()), Times.Never());
                projectRepositoryMock.Verify(p => p.ReadAsync());
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenDatabaseError_ReturnsERROR_DELETING()
        {
            var locationToDelete = new Location
            {
                Id = 1,
                City = "Sydne",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(c => c.FindAsync(locationToDelete.Id)).ReturnsAsync(locationToDelete);
            locationRepositoryMock.Setup(c => c.DeleteAsync(locationToDelete.Id)).ReturnsAsync(false);
            projectRepositoryMock.Setup(p => p.ReadAsync()).ReturnsAsync(new ProjectSummaryDTO[] { });
            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(new UserDTO[] { });

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(locationToDelete);

                Assert.Equal(ResponseLogic.ERROR_DELETING, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToDelete.Id));
                locationRepositoryMock.Verify(c => c.DeleteAsync(locationToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadAsync());
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenLocationDoesNotExist_ReturnsNOT_FOUND()
        {
            var locationToDelete = new Location
            {
                Id = 1,
                City = "Sydne",
                Country = "Australia"
            };

            locationRepositoryMock.Setup(l => l.FindAsync(locationToDelete.Id)).ReturnsAsync(default(Location));

            using (var logic = new LocationLogic(locationRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(locationToDelete);

                Assert.Equal(ResponseLogic.NOT_FOUND, response);
                locationRepositoryMock.Verify(c => c.FindAsync(locationToDelete.Id));
                locationRepositoryMock.Verify(c => c.DeleteAsync(It.IsAny<int>()), Times.Never());
                projectRepositoryMock.Verify(p => p.ReadAsync(), Times.Never());
                userRepositoryMock.Verify(u => u.ReadAsync(), Times.Never());
            }
        }

        #endregion
    }
}
