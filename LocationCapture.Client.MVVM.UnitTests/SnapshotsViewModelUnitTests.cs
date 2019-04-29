using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.MVVM.ViewModels;
using LocationCapture.Enums;
using LocationCapture.Models;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.Client.MVVM.UnitTests
{
    public class SnapshotsViewModelViewModelUnitTests
    {
        private ILocationSnapshotDataService _snapshotDataService;
        private IDialogService _dialogService;
        private IPictureService _pictureService;
        private IBitmapConverter _bitmapConverter;
        private INavigationService _navigationService;
        private IPlatformSpecificActions _platformSpecificActions;
        private List<LocationSnapshot> _snapshots;
        private List<SnapshotMiniature> _miniatures;
        private object _thumbnail;

        [Fact]
        public async void OnLoaded_ShouldLoadSnapshotsByLocationId()
        {
            var navParams = new SnapshotsViewNavParams { SnapshotsIdsource = new Location() };
            await OnLoadedBase(navParams);
        }

        [Fact]
        public async void OnLoaded_ShouldLoadSnapshotsByIds()
        {
            var navParams = new SnapshotsViewNavParams { SnapshotsIdsource = new SnapshotGroup() };
            await OnLoadedBase(navParams);
        }

        private async Task OnLoadedBase(SnapshotsViewNavParams navParams)
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            sit.NavigationParam = navParams;
            await sit.OnLoaded();

            // Assert
            Assert.Equal(3, sit.SnapshotThumbnails.Count);
            var index = 0;
            foreach (var thumbnail in sit.SnapshotThumbnails)
            {
                Assert.Equal(_snapshots[index], sit.SnapshotThumbnails[index].Snapshot);
                index++;
            }
            Assert.Equal(navParams.SnapshotsIdsource, sit.Parent);
        }

        [Fact]
        public async void RemoveSelectedSnapshots_ShouldSucceed()
        {
            // Arrange
            SetUp();
            var navParams = new SnapshotsViewNavParams { SnapshotsIdsource = new Location() };
            var thumbnails = new List<SnapshotThumbnail>
            {
                new SnapshotThumbnail{Snapshot = _snapshots[0]},
                new SnapshotThumbnail{Snapshot = _snapshots[1]},
                new SnapshotThumbnail{Snapshot = _snapshots[2]}
            };
            SnapshotsViewModel sit = null;
            _snapshotDataService.RemoveSnapshotAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<LocationSnapshot>();
                    tcs.SetResult(_snapshots[1]);
                    return tcs.Task;
                });
            _pictureService.RemoveSnapshotContentAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ => Task.CompletedTask);

            // Act
            sit = CreateViewModel();
            sit.NavigationParam = navParams;
            await sit.OnLoaded();            
            sit.BeginSelectSnapshot();
            sit.SelectedThumbnails = new List<SnapshotThumbnail> { thumbnails[1] };
            await sit.RemoveSelectedSnapshots();

            // Assert
            Assert.Equal(2, sit.SnapshotThumbnails.Count);
            Assert.DoesNotContain(sit.SnapshotThumbnails.Select(_ => _.Snapshot), _ => _.PictureFileName == "Barcelona_2.jpg");
            await _snapshotDataService.Received().RemoveSnapshotAsync(_snapshots[1].Id);
            await _pictureService.Received().RemoveSnapshotContentAsync(_snapshots[1]);
        }

        [Fact]
        public void GoBack_ShouldSetDefaultView()
        {
            // Act
            var sit = CreateViewModel();
            sit.BeginSelectSnapshot();
            sit.GoBack();

            // Assert
            Assert.False(sit.IsInSelectMode);
            Assert.Equal(SelectionMode.None, sit.SelectionMode);
            Assert.True(sit.IsItemClickEnabled);
        }

        [Fact]
        public void GoBack_ShouldNavigateToLocationsView()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            sit.GoBack();

            // Assert
            _navigationService.Received().GoTo(AppViews.Locations, GroupByCriteria.None);
        }

        [Fact]
        public void OnSnapshotAdding_ShouldNavigateToCameraView()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            sit.OnSnapshotAdding();

            // Assert
            _navigationService.Received().GoTo(AppViews.Camera, sit.NavigationParam);
        }

        [Fact]
        public void OnSnapshotClicked_ShouldNavigateToSnapshotDetailsView()
        {
            // Arrange
            SetUp();
            var targetView = AppViews.Locations;
            SnapshotDetailsViewNavParams actualNavParam = null;
            var clickedThumbnail = new SnapshotThumbnail { Snapshot = new LocationSnapshot() };
            _platformSpecificActions.GetClickedItem<SnapshotThumbnail>(Arg.Any<object>())
                .Returns(_ => clickedThumbnail);
            _navigationService.When(_ => _.GoTo(Arg.Any<AppViews>(), Arg.Any<object>()))
                .Do(_ =>
                {
                    targetView = _.Arg<AppViews>();
                    actualNavParam = (SnapshotDetailsViewNavParams)_.Arg<object>();
                });

            // Act
            var sit = CreateViewModel();
            sit.NavigationParam = new object();
            sit.OnSnapshotClicked(null, null);

            // Assert
            Assert.Equal(clickedThumbnail.Snapshot, actualNavParam.LocationSnapshot);
            Assert.Equal(sit.NavigationParam, actualNavParam.SnapshotsViewState);
            Assert.Equal(AppViews.SnapshotDetails, targetView);
        }

        private void SetUp()
        {
            _snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, PictureFileName = "Barcelona_1.jpg"},
                new LocationSnapshot{Id = 2, PictureFileName = "Barcelona_2.jpg"},
                new LocationSnapshot{Id = 3, PictureFileName = "Barcelona_3.jpg"}
            };
            _miniatures = new List<SnapshotMiniature>
            {
                new SnapshotMiniature{Snapshot = _snapshots[0]},
                new SnapshotMiniature{Snapshot = _snapshots[1]},
                new SnapshotMiniature{Snapshot = _snapshots[2]}
            };
            _thumbnail = new object();
            _snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            _snapshotDataService.GetSnapshotsByLocationIdAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(_snapshots);
                    return tcs.Task;
                });
            _snapshotDataService.GetSnapshotsByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(_snapshots);
                    return tcs.Task;
                });
            _pictureService = Substitute.For<IPictureService>();            
            _pictureService.GetSnapshotMiniaturesAsync(Arg.Any<IEnumerable<LocationSnapshot>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<SnapshotMiniature>>();
                    tcs.SetResult(_miniatures);
                    return tcs.Task;
                });
            _bitmapConverter = Substitute.For<IBitmapConverter>();
            _bitmapConverter.GetBitmapAsync(Arg.Any<byte[]>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<object>();
                    tcs.SetResult(_thumbnail);
                    return tcs.Task;
                });
            _dialogService = Substitute.For<IDialogService>();
            _dialogService.ShowConfirmationAsync(Arg.Any<string>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<ConfirmationAnswer>();
                    tcs.SetResult(ConfirmationAnswer.OK);
                    return tcs.Task;
                });
            _navigationService = Substitute.For<INavigationService>();
            _platformSpecificActions = Substitute.For<IPlatformSpecificActions>();
        }

        private SnapshotsViewModel CreateViewModel()
        {
            return new SnapshotsViewModel(_snapshotDataService,
                _navigationService,
                _pictureService,
                _bitmapConverter,
                _dialogService,
                _platformSpecificActions);
        }
    }
}
