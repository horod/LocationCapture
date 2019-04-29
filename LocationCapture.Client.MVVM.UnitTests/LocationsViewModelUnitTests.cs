using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.MVVM.ViewModels;
using LocationCapture.Enums;
using LocationCapture.Models;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.Client.MVVM.UnitTests
{
    public class LocationsViewModelUnitTests
    {
        private ILocationSnapshotDataService _snapshotDataService;
        private ILocationDataService _locationDataService;
        private IDialogService _dialogService;
        private IPictureService _pictureService;
        private ILocationDataImporter _locationDataImporter;
        private IDataSourceGovernor _dataSourceGovernor;
        private IConnectivityService _connectivityService;
        private INavigationService _navigationService;
        private IPlatformSpecificActions _platformSpecificActions;
        private List<Location> _locations;
        private List<SnapshotGroup> _snapshotGroups;

        [Fact]
        public async void OnLoaded_ShouldLoadLocations()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            await sit.OnLoaded();

            // Assert
            var index = 0;
            foreach(var location in sit.Locations)
            {
                Assert.Equal(_locations[index].Id, location.Id);
                Assert.Equal(_locations[index].Name, location.Name);
                index++;
            }
        }

        [Fact]
        public async void OnLoaded_ShouldLoadSnapshots()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            sit.NavigationParam = GroupByCriteria.CreatedDate;
            await sit.OnLoaded();

            // Assert
            var index = 0;
            foreach (var group in sit.SnapshotsGroups)
            {
                Assert.Equal(_snapshotGroups[index].Name, group.Name);
                index++;
            }
        }

        [Fact]
        public void BeginAddLocation_ShouldAdjustModes()
        {
            // Act
            var sit = CreateViewModel();
            sit.BeginAddLocation();

            // Assert
            Assert.NotNull(sit.SelectedLocation);
            Assert.False(sit.IsInBrowseMode);
            Assert.True(sit.IsInEditMode);
            Assert.False(sit.IsInSelectMode);
            Assert.False(sit.IsItemClickEnabled);
            Assert.Equal(SelectionMode.None, sit.SelectionMode);
        }

        [Fact]
        public void BeginRenameLocation_ShouldAdjustModes()
        {
            // Arrange
            var selectedLoc = new Location {Id = 1, Name = "SelectedLoc"};

            // Act
            var sit = CreateViewModel();
            sit.SelectedLocations = new List<Location>{ selectedLoc };
            sit.BeginRenameLocation();

            // Assert
            Assert.Equal(selectedLoc.Id, sit.SelectedLocation.Id);
            Assert.Equal(selectedLoc.Name, sit.SelectedLocation.Name);
            Assert.False(sit.IsInBrowseMode);
            Assert.True(sit.IsInEditMode);
            Assert.False(sit.IsInSelectMode);
            Assert.False(sit.IsItemClickEnabled);
            Assert.Equal(SelectionMode.None, sit.SelectionMode);
        }

        [Fact]
        public void BeginSelectLocation_ShouldAdjustModes()
        {
            // Act
            var sit = CreateViewModel();
            sit.BeginSelectLocation();

            // Assert
            Assert.Empty(sit.SelectedLocations);
            Assert.False(sit.IsInBrowseMode);
            Assert.False(sit.IsInEditMode);
            Assert.True(sit.IsInSelectMode);
            Assert.False(sit.IsItemClickEnabled);
            Assert.Equal(SelectionMode.Multiple, sit.SelectionMode);
        }

        [Fact]
        public async void SaveChanges_ShouldAddLocation()
        {
            // Arrange
            SetUp();
            LocationsViewModel sit = null;
            var newLocationName = "NewLocation";
            _locationDataService.AddLocationAsync(Arg.Any<Location>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<Location>();
                    tcs.SetResult(sit.SelectedLocation);
                    return tcs.Task;
                });

            // Act
            sit = CreateViewModel();
            await sit.OnLoaded();
            sit.BeginAddLocation();
            sit.SelectedLocation.Name = newLocationName;
            await sit.SaveChanges();

            // Assert
            Assert.Equal(4, sit.Locations.Count);
            Assert.Equal(newLocationName, sit.Locations[3].Name);
        }

        [Fact]
        public async void SaveChanges_ShouldRenameLocation()
        {
            // Arrange
            SetUp();
            LocationsViewModel sit = null;
            var newLocationName = "RenamedLocation";
            _locationDataService.RenameLocationAsync(Arg.Any<int>(), Arg.Any<string>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<Location>();
                    tcs.SetResult(sit.SelectedLocation);
                    return tcs.Task;
                });

            // Act
            sit = CreateViewModel();
            await sit.OnLoaded();
            sit.SelectedLocations = new List<Location> { sit.Locations[2] };
            sit.BeginRenameLocation();
            sit.SelectedLocation.Name = newLocationName;
            await sit.SaveChanges();

            // Assert
            Assert.Equal(3, sit.Locations.Count);
            Assert.Equal(newLocationName, sit.Locations[2].Name);
        }

        [Fact]
        public async void RemoveSelectedLocations_ShouldSucceed()
        {
            // Arrange
            SetUp();
            LocationsViewModel sit = null;
            var snapshotsToDelete = new List<LocationSnapshot>
            {
                new LocationSnapshot { Id = 1 },
                new LocationSnapshot { Id = 2 }
            };
            _snapshotDataService.GetSnapshotsByLocationIdAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(snapshotsToDelete);
                    return tcs.Task;
                });
            _snapshotDataService.RemoveSnapshotAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<LocationSnapshot>();
                    tcs.SetResult(new LocationSnapshot());
                    return tcs.Task;
                });
            _pictureService.RemoveSnapshotContentAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ => Task.CompletedTask);
            _locationDataService.RemoveLocationAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<Location>();
                    tcs.SetResult(sit.Locations[1]);
                    return tcs.Task;
                });

            // Act
            sit = CreateViewModel();
            await sit.OnLoaded();
            sit.SelectedLocations = new List<Location> { sit.Locations[1] };
            await sit.RemoveSelectedLocations();

            // Assert
            Assert.Equal(2, sit.Locations.Count);
            Assert.DoesNotContain(sit.Locations, _ => _.Name == "Prague");
            await _snapshotDataService.Received().RemoveSnapshotAsync(snapshotsToDelete[0].Id);
            await _snapshotDataService.Received().RemoveSnapshotAsync(snapshotsToDelete[1].Id);
            await _pictureService.Received().RemoveSnapshotContentAsync(snapshotsToDelete[0]);
            await _pictureService.Received().RemoveSnapshotContentAsync(snapshotsToDelete[1]);
        }

        [Fact]
        public async void ImportSelectedLocations_ShouldSucceed()
        {
            // Arrange
            SetUp();
            List<Location> locationsToBeImported = null;
            var selectedLocations = new List<Location>
            {
                new Location { Id = 1 },
                new Location { Id = 2 }
            };
            _dataSourceGovernor.GetCurrentDataSourceTypeAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<DataSourceType>();
                    tcs.SetResult(DataSourceType.Local);
                    return tcs.Task;
                });
            _connectivityService.IsWebApiAvailableAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<bool>();
                    tcs.SetResult(true);
                    return tcs.Task;
                });
            _locationDataImporter.ImportAsync(Arg.Any<List<Location>>(), Arg.Any<CancellationToken>())
                .Returns(_ =>
                {
                    locationsToBeImported = _.Arg<IEnumerable<Location>>().ToList();
                    return Task.CompletedTask;
                });

            // Act
            var sit = CreateViewModel();
            sit.SelectedLocations = selectedLocations;
            await sit.ImportSelectedLocations();

            // Assert
            Assert.Equal(selectedLocations, locationsToBeImported);
        }

        [Fact]
        public void OnLocationClicked_ShouldNavigateToSnapshotsView()
        {
            // Arrange
            var clickedLocation = new Location();
            ShouldNavigateToSnapshotsView(clickedLocation, (LocationsViewModel sit) => sit.OnLocationClicked(null, null));
        }

        [Fact]
        public void OnSnapshotGroupClicked_ShouldNavigateToSnapshotsView()
        {
            // Arrange
            var clickedSnapshotGroup = new SnapshotGroup();
            ShouldNavigateToSnapshotsView(clickedSnapshotGroup, (LocationsViewModel sit) => sit.OnSnapshotGroupClicked(null, null));
        }

        private void ShouldNavigateToSnapshotsView<TClickedEntity>(TClickedEntity clickedEntity, Action<LocationsViewModel> actionToTest)
        {
            // Arrange
            SetUp();
            _platformSpecificActions.GetClickedItem<TClickedEntity>(Arg.Any<object>())
                .Returns(_ => clickedEntity);
            var targetView = AppViews.Locations;
            SnapshotsViewNavParams actualNavParam = null;

            _navigationService.When(_ => _.GoTo(Arg.Any<AppViews>(), Arg.Any<object>()))
                .Do(_ =>
                {
                    targetView = _.Arg<AppViews>();
                    actualNavParam = (SnapshotsViewNavParams)_.Arg<object>();
                });

            // Act
            var sit = CreateViewModel();
            sit.GroupBy = GroupByCriteria.CreatedDate;
            actionToTest(sit);

            // Assert
            Assert.Equal(clickedEntity, actualNavParam.SnapshotsIdsource);
            Assert.Equal(sit.GroupBy, actualNavParam.GroupByCriteria);
            Assert.Equal(AppViews.Snapshots, targetView);
        }

        private void SetUp()
        {
            _locations = new List<Location>
            {
                new Location{Id = 1, Name = "Barcelona"},
                new Location{Id = 2, Name = "Prague"},
                new Location{Id = 3, Name = "Frankfurt"}
            };
            _snapshotGroups = new List<SnapshotGroup>
            {
                new SnapshotGroup {Name = "December 2018"},
                new SnapshotGroup {Name = "March 2019"}
            };
            Task<IEnumerable<SnapshotGroup>> dataFetchOperation()
            {
                var tcs = new TaskCompletionSource<IEnumerable<SnapshotGroup>>();
                tcs.SetResult(_snapshotGroups);
                return tcs.Task;
            };
            _snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            _snapshotDataService.ChooseGroupByOperation(Arg.Any<GroupByCriteria>())
                .Returns(dataFetchOperation);
            _locationDataService = Substitute.For<ILocationDataService>();
            _locationDataService.GetAllLocationsAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<Location>>();
                    tcs.SetResult(_locations);
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
            _pictureService = Substitute.For<IPictureService>();
            _locationDataImporter = Substitute.For<ILocationDataImporter>();
            _dataSourceGovernor = Substitute.For<IDataSourceGovernor>();
            _connectivityService = Substitute.For<IConnectivityService>();
            _navigationService = Substitute.For<INavigationService>();
            _platformSpecificActions = Substitute.For<IPlatformSpecificActions>();
        }

        private LocationsViewModel CreateViewModel()
        {
            return new LocationsViewModel(_locationDataService,
                _snapshotDataService,
                _navigationService,
                _dialogService,
                _pictureService,
                _locationDataImporter,
                _dataSourceGovernor,
                _connectivityService,
                _platformSpecificActions);
        }
    }
}
