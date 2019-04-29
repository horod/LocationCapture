using AutoMapper;
using LocationCapture.BL;
using LocationCapture.Models;
using LocationCapture.WebAPI.Controllers;
using LocationCapture.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.WebAPI.UnitTests
{
    public class LocationSnapshotsControllerUnitTests
    {
        private void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<LocationSnapshot, LocationSnapshotDto>();
                cfg.CreateMap<LocationSnapshotForCreationDto, LocationSnapshot>();
                cfg.CreateMap<LocationSnapshotForUpdateDto, LocationSnapshot>();
            });
        }

        [Fact]
        public async void GetSnapshotsByLocationId_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot
                {
                    Id = 1,
                    LocationId = 1,
                    Longitude = 56,
                    Latitude = 49,
                    Altitude = 1024,
                    PictureFileName = "SomePic.jpg",
                    DateCreated = new DateTime(2018, 9, 1)
                }
            };
            var snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            snapshotDataService.GetSnapshotsByLocationIdAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(snapshots);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationSnapshotsController(snapshotDataService, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.GetSnapshotsByLocationId(1);

            // Assert
            var dtos = WebApiTestsHelper.ExtractGenericCollectionFromActionResult<OkObjectResult, IEnumerable<LocationSnapshotDto>>(result)
                .ToList();
            Assert.Equal(snapshots[0].Id, dtos[0].Id);
            Assert.Equal(snapshots[0].LocationId, dtos[0].LocationId);
            Assert.Equal(snapshots[0].Longitude, dtos[0].Longitude);
            Assert.Equal(snapshots[0].Latitude, dtos[0].Latitude);
            Assert.Equal(snapshots[0].Altitude, dtos[0].Altitude);
            Assert.Equal(snapshots[0].PictureFileName, dtos[0].PictureFileName);
            Assert.Equal(snapshots[0].DateCreated, dtos[0].DateCreated);

            WebApiTestsHelper.Unlock();
        }

        [Fact]
        public async void GetSnapshotsByIds_ShouldReturnBadRequest()
        {
            // Act
            var sit = new LocationSnapshotsController(null, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.GetSnapshotsByIds(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void GetSnapshotsByIds_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot
                {
                    Id = 1,
                    LocationId = 1,
                    Longitude = 56,
                    Latitude = 49,
                    Altitude = 1024,
                    PictureFileName = "SomePic.jpg",
                    DateCreated = new DateTime(2018, 9, 1)
                }
            };
            var snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            snapshotDataService.GetSnapshotsByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(snapshots);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationSnapshotsController(snapshotDataService, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.GetSnapshotsByIds(new[] { 1, 2, 3 });

            // Assert
            var dtos = WebApiTestsHelper.ExtractGenericCollectionFromActionResult<OkObjectResult, IEnumerable<LocationSnapshotDto>>(result)
                .ToList();
            Assert.Equal(snapshots[0].Id, dtos[0].Id);
            Assert.Equal(snapshots[0].LocationId, dtos[0].LocationId);
            Assert.Equal(snapshots[0].Longitude, dtos[0].Longitude);
            Assert.Equal(snapshots[0].Latitude, dtos[0].Latitude);
            Assert.Equal(snapshots[0].Altitude, dtos[0].Altitude);
            Assert.Equal(snapshots[0].PictureFileName, dtos[0].PictureFileName);
            Assert.Equal(snapshots[0].DateCreated, dtos[0].DateCreated);

            WebApiTestsHelper.Unlock();
        }

        [Fact]
        public async void AddSnapshot_ShouldReturnBadRequest()
        {
            // Arrange
            var snapshotForCreationDto = new LocationSnapshotForCreationDto();

            // Act
            var sit = new LocationSnapshotsController(null, Substitute.For<ILogger<LocationSnapshotsController>>());
            sit.ModelState.AddModelError("PropName", "ModelError");
            var result = await sit.AddSnapshot(snapshotForCreationDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void AddSnapshot_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var snapshotForCreationDto = new LocationSnapshotForCreationDto
            {
                LocationId = 1,
                Longitude = 56,
                Latitude = 49,
                Altitude = 1024,
                PictureFileName = "SomePic.jpg",
                DateCreated = new DateTime(2018, 9, 1)
            };
            var snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            snapshotDataService.AddSnapshotAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ =>
                {
                    var snapshotToAdd = _.Arg<LocationSnapshot>();
                    snapshotToAdd.Id = 55;
                    var tcs = new TaskCompletionSource<LocationSnapshot>();
                    tcs.SetResult(snapshotToAdd);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationSnapshotsController(snapshotDataService, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.AddSnapshot(snapshotForCreationDto);

            // Assert
            var addedSnapshot = WebApiTestsHelper.ExtractObjectFromActionResult<OkObjectResult, LocationSnapshotDto>(result);
            Assert.Equal(55, addedSnapshot.Id);
            Assert.Equal(snapshotForCreationDto.LocationId, addedSnapshot.LocationId);
            Assert.Equal(snapshotForCreationDto.Longitude, addedSnapshot.Longitude);
            Assert.Equal(snapshotForCreationDto.Latitude, addedSnapshot.Latitude);
            Assert.Equal(snapshotForCreationDto.Altitude, addedSnapshot.Altitude);
            Assert.Equal(snapshotForCreationDto.PictureFileName, addedSnapshot.PictureFileName);
            Assert.Equal(snapshotForCreationDto.DateCreated, addedSnapshot.DateCreated);

            WebApiTestsHelper.Unlock();
        }

        [Fact]
        public async void UpdateSnapshot_ShouldReturnBadRequest_IdMismatch()
        {
            // Arrange
            var snapshotForUpdateDto = new LocationSnapshotForUpdateDto { Id = 5 };

            // Act
            var sit = new LocationSnapshotsController(null, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.UpdateSnapshot(13, snapshotForUpdateDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void UpdateSnapshot_ShouldReturnBadRequest_ModelError()
        {
            // Arrange
            var snapshotForUpdateDto = new LocationSnapshotForUpdateDto { Id = 1 };

            // Act
            var sit = new LocationSnapshotsController(null, Substitute.For<ILogger<LocationSnapshotsController>>());
            sit.ModelState.AddModelError("PropName", "ModelError");
            var result = await sit.UpdateSnapshot(1, snapshotForUpdateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void UpdateSnapshot_ShouldReturnNotFound()
        {
            // Arrange
            var snapshotForUpdateDto = new LocationSnapshotForUpdateDto { Id = 1 };
            var snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            snapshotDataService.GetSnapshotsByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(Enumerable.Empty<LocationSnapshot>());
                    return tcs.Task;
                });

            // Act
            var sit = new LocationSnapshotsController(snapshotDataService, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.UpdateSnapshot(1, snapshotForUpdateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void UpdateSnapshot_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var snapshotForUpdateDto = new LocationSnapshotForUpdateDto
            {
                Id = 5,
                Longitude = 4,
                Latitude = 5,
                Altitude = 6
            };
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot
                {
                    Id = 5,
                    Longitude = 1,
                    Latitude = 2,
                    Altitude = 3
                }
            };
            var snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            snapshotDataService.GetSnapshotsByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(snapshots);
                    return tcs.Task;
                });
            snapshotDataService.UpdateSnapshotAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ =>
                {
                    var snapshot = _.Arg<LocationSnapshot>();
                    var snapshotToUpdate = snapshots.First(loc => loc.Id == snapshot.Id);
                    snapshotToUpdate.Longitude = snapshot.Longitude;
                    snapshotToUpdate.Latitude = snapshot.Latitude;
                    snapshotToUpdate.Altitude = snapshot.Altitude;

                    var tcs = new TaskCompletionSource<LocationSnapshot>();
                    tcs.SetResult(snapshotToUpdate);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationSnapshotsController(snapshotDataService, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.UpdateSnapshot(5, snapshotForUpdateDto);

            // Assert
            var updatedLocationSnapshot = WebApiTestsHelper.ExtractObjectFromActionResult<OkObjectResult, LocationSnapshotDto>(result);
            Assert.Equal(snapshotForUpdateDto.Longitude, updatedLocationSnapshot.Longitude);
            Assert.Equal(snapshotForUpdateDto.Latitude, updatedLocationSnapshot.Latitude);
            Assert.Equal(snapshotForUpdateDto.Altitude, updatedLocationSnapshot.Altitude);

            WebApiTestsHelper.Unlock();
        }

        [Fact]
        public async void DeleteLocationSnapshot_ShouldReturnNotFound()
        {
            // Arrange
            var snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            snapshotDataService.GetSnapshotsByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(Enumerable.Empty<LocationSnapshot>());
                    return tcs.Task;
                });

            // Act
            var sit = new LocationSnapshotsController(snapshotDataService, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.DeleteSnapshot(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void DeleteLocationSnapshot_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot
                {
                    Id = 5,
                    Longitude = 1,
                    Latitude = 2,
                    Altitude = 3
                }
            };
            var snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            snapshotDataService.GetSnapshotsByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(snapshots);
                    return tcs.Task;
                });
            snapshotDataService.RemoveSnapshotAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var snapshotId = _.Arg<int>();
                    var snapshotToDelete = snapshots.First(loc => loc.Id == snapshotId);

                    var tcs = new TaskCompletionSource<LocationSnapshot>();
                    tcs.SetResult(snapshotToDelete);
                    return tcs.Task;
                });

            // Act
            var sit = new LocationSnapshotsController(snapshotDataService, Substitute.For<ILogger<LocationSnapshotsController>>());
            var result = await sit.DeleteSnapshot(5);

            // Assert
            var deletedLocationSnapshot = WebApiTestsHelper.ExtractObjectFromActionResult<OkObjectResult, LocationSnapshotDto>(result);
            Assert.Equal(5, deletedLocationSnapshot.Id);

            WebApiTestsHelper.Unlock();
        }
    }
}
