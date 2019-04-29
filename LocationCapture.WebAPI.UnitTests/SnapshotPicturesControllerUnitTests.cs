using LocationCapture.BL;
using LocationCapture.Models;
using LocationCapture.WebAPI.Controllers;
using LocationCapture.WebAPI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.WebAPI.UnitTests
{
    public class SnapshotPicturesControllerUnitTests
    {
        private List<LocationSnapshot> _snapshots;
        private byte[] _miniatureData;
        private byte[] _pictureData;
        private ILocationSnapshotDataService _snapshotDataService;
        private IFileSystemService _fileSystemService;
        private IImageService _imageService;

        [Fact]
        public async void GetMiniatures_ShouldReturnOk()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateController();
            var result = await sit.GetMiniatures(new List<int> { 1, 2, 3 });

            // Assert
            var dtos = WebApiTestsHelper.ExtractGenericCollectionFromActionResult<OkObjectResult, ArrayList>(result);                
            Assert.Equal(3, dtos.Count);
            await _imageService.Received().GetThumbnailAsync(@"Snapshot_Pictures\Barcelona1.jpg");
            await _imageService.Received().GetThumbnailAsync(@"Snapshot_Pictures\Prague1.jpg");
            await _imageService.Received().GetThumbnailAsync(@"Snapshot_Pictures\Frankfurt1.jpg");
        }

        [Fact]
        public async void GetPicture_ShouldReturnOk()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateController();
            var result = await sit.GetPicture("Barcelona1.jpg");

            // Assert
            var encodedPicture = WebApiTestsHelper.ExtractObjectFromActionResult<OkObjectResult, string>(result);
            Assert.Equal(Convert.ToBase64String(_pictureData), encodedPicture);
            await _fileSystemService.Received().ReadAllBytesAsync(@"Snapshot_Pictures\Barcelona1.jpg");
        }

        [Fact]
        public async void GetMiniature_ShouldReturnOk()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateController();
            var result = await sit.GetPicture("Frankfurt1.jpg", true);

            // Assert
            var encodedMiniature = WebApiTestsHelper.ExtractObjectFromActionResult<OkObjectResult, string>(result);
            Assert.Equal(Convert.ToBase64String(_miniatureData), encodedMiniature);
            await _imageService.Received().GetThumbnailAsync(@"Snapshot_Pictures\Frankfurt1.jpg");
        }

        [Fact]
        public async void AddPicture_ShouldReturnOk()
        {
            // Arrange
            SetUp();
            _fileSystemService.FileExisits(Arg.Any<string>())
                .Returns(false);
            dynamic payload = new ExpandoObject();
            payload.pictureFile = Convert.ToBase64String(_pictureData);
            payload.pictureFileName = "Barcelona1.jpg";
            byte[] addedPicBytes = null;
            string addedPicPath = null;
            _fileSystemService.WriteAllBytesAsync(Arg.Any<string>(), Arg.Any<byte[]>())
                .Returns(_ =>
            {
                addedPicBytes = _.Arg<byte[]>();
                addedPicPath = _.Arg<string>();
                return Task.CompletedTask;
            });

            // Act
            var sit = CreateController();
            var result = await sit.AddPicture(payload);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Equal(@"Snapshot_Pictures\Barcelona1.jpg", addedPicPath);
            Assert.Equal(_pictureData, addedPicBytes);
        }

        [Fact]
        public void DeletePicture_ShouldReturnOk()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateController();
            var result = sit.DeletePicture("Barcelona1.jpg");

            // Assert
            Assert.IsType<OkResult>(result);
            _fileSystemService.Received().DeleteFile(@"Snapshot_Pictures\Barcelona1.jpg");
        }

        private void SetUp()
        {
            _snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{PictureFileName = "Barcelona1.jpg"},
                new LocationSnapshot{PictureFileName = "Prague1.jpg"},
                new LocationSnapshot{PictureFileName = "Frankfurt1.jpg"}
            };
            _miniatureData = new byte[] { 255, 216, 255 };
            _pictureData = new byte[] { 224, 0, 16 };
            _snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            _snapshotDataService.GetSnapshotsByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(_snapshots);
                    return tcs.Task;
                });
            _fileSystemService = Substitute.For<IFileSystemService>();
            _fileSystemService.FileExisits(Arg.Any<string>())
                .Returns(true);
            _fileSystemService.ReadAllBytesAsync(Arg.Any<string>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<byte[]>();
                    tcs.SetResult(_pictureData);
                    return tcs.Task;
                });
            _imageService = Substitute.For<IImageService>();
            _imageService.GetThumbnailAsync(Arg.Any<string>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<byte[]>();
                    tcs.SetResult(_miniatureData);
                    return tcs.Task;
                });
        }
        
        private SnapshotPicturesController CreateController()
        {
            return new SnapshotPicturesController(Substitute.For<IHostingEnvironment>(),
                _fileSystemService,
                _imageService,
                _snapshotDataService,
                Substitute.For<ILogger<SnapshotPicturesController>>());
        }
    }
}
