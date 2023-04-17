using AutoMapper;
using LocationCapture.BL;
using LocationCapture.Models;
using LocationCapture.WebAPI.Controllers;
using LocationCapture.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.WebAPI.UnitTests
{
    public class LocationsControllerUnitTests
    {
        private void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Location, LocationDto>();
                cfg.CreateMap<LocationForCreationDto, Location>();
                cfg.CreateMap<LocationForUpdateDto, Location>();
            });
        }

        [Fact]
        public async void GetAllLocations_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var locations = new List<Location>
            {
                new Location
                {
                    Id = 1,
                    Name = "Warsaw"
                },
                new Location
                {
                    Id = 2,
                    Name = "Lisbon"
                }
            };
            var locationDataService = Substitute.For<ILocationDataService>();
            locationDataService.GetAllLocationsAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<Location>>();
                    tcs.SetResult(locations);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationsController(locationDataService, Substitute.For<ILogger<LocationsController>>());
            var result = await sit.GetAllLocations();

            // Assert
            var dtos = WebApiTestsHelper.ExtractGenericCollectionFromActionResult<OkObjectResult, IEnumerable<LocationDto>>(result)
                .ToList();
            for (var i = 0; i < dtos.Count; i++)
            {
                Assert.Equal(locations[i].Id, dtos[i].Id);
                Assert.Equal(locations[i].Name, dtos[i].Name);
            }

            WebApiTestsHelper.Unlock();
        }

        [Fact]
        public async void AddLocation_ShouldReturnBadRequest()
        {
            // Arrange
            var locationForCreationDto = new LocationForCreationDto();

            // Act
            var sit = new LocationsController(null, Substitute.For<ILogger<LocationsController>>());
            sit.ModelState.AddModelError("PropName", "ModelError");
            var result = await sit.AddLocation(locationForCreationDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void AddLocation_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var locationForCreationDto = new LocationForCreationDto
            {
                Name = "Warsaw"
            };
            var locationDataService = Substitute.For<ILocationDataService>();
            locationDataService.AddLocationAsync(Arg.Any<Location>())
                .Returns(_ =>
                {
                    var locationToAdd = _.Arg<Location>();
                    locationToAdd.Id = 55;
                    var tcs = new TaskCompletionSource<Location>();
                    tcs.SetResult(locationToAdd);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationsController(locationDataService, Substitute.For<ILogger<LocationsController>>());
            var result = await sit.AddLocation(locationForCreationDto);

            // Assert
            var addedLocation = WebApiTestsHelper.ExtractObjectFromActionResult<OkObjectResult, LocationDto>(result);
            Assert.Equal(locationForCreationDto.Name, addedLocation.Name);
            Assert.Equal(55, addedLocation.Id);

            WebApiTestsHelper.Unlock();
        }

        [Fact]
        public async void UpdateLocation_ShouldReturnBadRequest_IdMismatch()
        {
            // Arrange
            var locationForUpdateDto = new LocationForUpdateDto {Id = 5};

            // Act
            var sit = new LocationsController(null, Substitute.For<ILogger<LocationsController>>());
            var result = await sit.UpdateLocation(13, locationForUpdateDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void UpdateLocation_ShouldReturnBadRequest_ModelError()
        {
            // Arrange
            var locationForUpdateDto = new LocationForUpdateDto { Id = 1 };

            // Act
            var sit = new LocationsController(null, Substitute.For<ILogger<LocationsController>>());
            sit.ModelState.AddModelError("PropName", "ModelError");
            var result = await sit.UpdateLocation(1, locationForUpdateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void UpdateLocation_ShouldReturnNotFound()
        {
            // Arrange
            var locationForUpdateDto = new LocationForUpdateDto { Id = 1 };
            var locationDataService = Substitute.For<ILocationDataService>();
            locationDataService.GetAllLocationsAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<Location>>();
                    tcs.SetResult(Enumerable.Empty<Location>());
                    return tcs.Task;
                });

            // Act
            var sit = new LocationsController(locationDataService, Substitute.For<ILogger<LocationsController>>());
            var result = await sit.UpdateLocation(1, locationForUpdateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void UpdateLocation_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var locationForUpdateDto = new LocationForUpdateDto
            {
                Id = 5,
                Name = "Warsaw_Updated"
            };
            var locations = new List<Location>
            {
                new Location
                {
                    Id = 5,
                    Name = "Warsaw"
                }
            };
            var locationDataService = Substitute.For<ILocationDataService>();
            locationDataService.GetAllLocationsAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<Location>>();
                    tcs.SetResult(locations);
                    return tcs.Task;
                });
            locationDataService.RenameLocationAsync(Arg.Any<int>(), Arg.Any<string>())
                .Returns(_ =>
                {
                    var locationId = _.Arg<int>();
                    var newName = _.Arg<string>();
                    var locationToUpdate = locations.First(loc => loc.Id == locationId);
                    locationToUpdate.Name = newName;

                    var tcs = new TaskCompletionSource<Location>();
                    tcs.SetResult(locationToUpdate);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationsController(locationDataService, Substitute.For<ILogger<LocationsController>>());
            var result = await sit.UpdateLocation(5, locationForUpdateDto);

            // Assert
            var updatedLocation = WebApiTestsHelper.ExtractObjectFromActionResult<OkObjectResult, LocationDto>(result);
            Assert.Equal(locationForUpdateDto.Name, updatedLocation.Name);

            WebApiTestsHelper.Unlock();
        }

        [Fact]
        public async void DeleteLocation_ShouldReturnNotFound()
        {
            // Arrange
            var locationDataService = Substitute.For<ILocationDataService>();
            locationDataService.GetAllLocationsAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<Location>>();
                    tcs.SetResult(Enumerable.Empty<Location>());
                    return tcs.Task;
                });

            // Act
            var sit = new LocationsController(locationDataService, Substitute.For<ILogger<LocationsController>>());
            var result = await sit.DeleteLocation(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void DeleteLocation_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var locations = new List<Location>
            {
                new Location
                {
                    Id = 5,
                    Name = "Warsaw"
                }
            };
            var locationDataService = Substitute.For<ILocationDataService>();
            locationDataService.GetAllLocationsAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<Location>>();
                    tcs.SetResult(locations);
                    return tcs.Task;
                });
            locationDataService.RemoveLocationAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var locationId = _.Arg<int>();
                    var locationToDelete = locations.First(loc => loc.Id == locationId);

                    var tcs = new TaskCompletionSource<Location>();
                    tcs.SetResult(locationToDelete);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationsController(locationDataService, Substitute.For<ILogger<LocationsController>>());
            var result = await sit.DeleteLocation(5);

            // Assert
            var deletedLocation = WebApiTestsHelper.ExtractObjectFromActionResult<OkObjectResult, LocationDto>(result);
            Assert.Equal(5, deletedLocation.Id);

            WebApiTestsHelper.Unlock();
        }
    }
}
