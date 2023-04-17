using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Enums;
using LocationCapture.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Location = LocationCapture.Models.Location;
using SelectionMode = LocationCapture.Client.MVVM.Enums.SelectionMode;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class LocationsViewModel : NotificationBase, INavigationTarget
    {
        private enum EditOperation
        {
            Add,
            Rename
        }

        private EditOperation _editOperation;
        private readonly ILocationDataService _locationDataService;
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPictureService _pictureService;
        private readonly ILocationDataImporter _locationDataImporter;
        private readonly IDataSourceGovernor _dataSourceGovernor;
        private readonly IConnectivityService _connectivityService;
        private readonly IPlatformSpecificActions _platformSpecificActions;
        private readonly IAppStateProvider _appStateProvider;
        private CancellationTokenSource _importCancellationTokenSource;

        public object NavigationParam { get; set; }

        private List<Location> _SelectedLocations;
        public List<Location> SelectedLocations
        {
            get { return _SelectedLocations; }
            set { SetProperty(ref _SelectedLocations, value); }
        }

        private Location _SelectedLocation;
        public Location SelectedLocation
        {
            get { return _SelectedLocation; }
            set { SetProperty(ref _SelectedLocation, value); }
        }

        private SelectionMode _SelectionMode;
        public SelectionMode SelectionMode
        {
            get { return _SelectionMode; }
            set { SetProperty(ref _SelectionMode, value); }
        }

        private ObservableCollection<Location> _Locations;
        public ObservableCollection<Location> Locations
        {
            get { return _Locations; }
            set { SetProperty(ref _Locations, value); }
        }

        private ObservableCollection<SnapshotGroup> _SnapshotsGroups;
        public ObservableCollection<SnapshotGroup> SnapshotsGroups
        {
            get { return _SnapshotsGroups; }
            set { SetProperty(ref _SnapshotsGroups, value); }
        }

        private List<GroupByCriteria> _GroupByOptions;
        public List<GroupByCriteria> GroupByOptions
        {
            get { return _GroupByOptions; }
            set { SetProperty(ref _GroupByOptions, value); }
        }

        private GroupByCriteria _GroupBy;
        public GroupByCriteria GroupBy
        {
            get { return _GroupBy; }
            set
            {
                var shouldRefreshData = true;
                if (_GroupBy == value) shouldRefreshData = false;
                SetProperty(ref _GroupBy, value);
                if (shouldRefreshData) _ = GroupByChanged();
            }
        }

        private bool _IsViewGrouped;
        public bool IsViewGrouped
        {
            get { return _IsViewGrouped; }
            set { SetProperty(ref _IsViewGrouped, value); }
        }

        private bool _IsInEditMode;
        public bool IsInEditMode
        {
            get { return _IsInEditMode; }
            set { SetProperty(ref _IsInEditMode, value); }
        }

        private bool _IsInBrowseMode;
        public bool IsInBrowseMode
        {
            get { return _IsInBrowseMode; }
            set { SetProperty(ref _IsInBrowseMode, value); }
        }

        private bool _IsInSelectMode;
        public bool IsInSelectMode
        {
            get { return _IsInSelectMode; }
            set { SetProperty(ref _IsInSelectMode, value); }
        }

        private bool _IsItemClickEnabled;
        public bool IsItemClickEnabled
        {
            get { return _IsItemClickEnabled; }
            set { SetProperty(ref _IsItemClickEnabled, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        private bool _IsImportInProgress;
        public bool IsImportInProgress
        {
            get { return _IsImportInProgress; }
            set { SetProperty(ref _IsImportInProgress, value); }
        }

        public LocationsViewModel(ILocationDataService locationDataService,
            ILocationSnapshotDataService locationSnapshotDataService,
            INavigationService navigationService,
            IDialogService dialogService,
            IPictureService pictureService,
            ILocationDataImporter locationDataImporter,
            IDataSourceGovernor dataSourceGovernor,
            IConnectivityService connectivityService,
            IPlatformSpecificActions platformSpecificActions,
            IAppStateProvider appStateProvider)
        {
            _locationDataService = locationDataService;
            _locationSnapshotDataService = locationSnapshotDataService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _pictureService = pictureService;
            _locationDataImporter = locationDataImporter;
            _dataSourceGovernor = dataSourceGovernor;
            _connectivityService = connectivityService;
            _platformSpecificActions = platformSpecificActions;
            _appStateProvider = appStateProvider;

            GroupByOptions = new List<GroupByCriteria>
            {
                GroupByCriteria.None,
                GroupByCriteria.CreatedDate,
                GroupByCriteria.Longitude,
                GroupByCriteria.Latitude,
                GroupByCriteria.Altitude
            };
            SelectedLocations = new List<Location>();
            GroupBy = GroupByCriteria.None;
            SelectionMode = SelectionMode.None;
            IsItemClickEnabled = true;
            IsInBrowseMode = true;
            IsInEditMode = false;
            IsInSelectMode = false;
        }

        public async Task OnLoaded()
        {
            GroupBy = (NavigationParam is GroupByCriteria) ? (GroupByCriteria)NavigationParam : GroupByCriteria.None;
            await GroupByChanged();
        }

        public void BeginAddLocation()
        {
            _editOperation = EditOperation.Add;
            SelectedLocation = new Location();
            IsInBrowseMode = false;
            IsInEditMode = true;
            IsInSelectMode = false;
            SelectionMode = SelectionMode.None;
            IsItemClickEnabled = false;
        }

        public async void BeginRenameLocation()
        {
            PopulateSelectedLocation();
            if (SelectedLocation == null)
            {
                await _dialogService.ShowAsync("Please select exactly one location.");
                return;
            }
            _editOperation = EditOperation.Rename;
            IsInBrowseMode = false;
            IsInEditMode = true;
            IsInSelectMode = false;
            SelectionMode = SelectionMode.None;
            IsItemClickEnabled = false;
        }

        private void PopulateSelectedLocation()
        {
            var selectedLocation = SelectedLocations.LastOrDefault();
            if (selectedLocation == null) return;
            SelectedLocation = new Location
            {
                Id = selectedLocation.Id,
                Name = selectedLocation.Name
            };
        }

        public async Task SaveChanges()
        {
            if (string.IsNullOrEmpty(SelectedLocation.Name))
            {
                await _dialogService.ShowAsync("Location name cannot be empty.");
                return;
            }

            IsBusy = true;
            if (_editOperation == EditOperation.Add)
            {
                var newLocation = await _locationDataService.AddLocationAsync(SelectedLocation);
                Locations.Add(newLocation);
            }
            else if (_editOperation == EditOperation.Rename)
            {
                var renamedLocation = await _locationDataService.RenameLocationAsync(SelectedLocation.Id, SelectedLocation.Name);
                Locations.First(_ => _.Id == renamedLocation.Id).Name = renamedLocation.Name;
            }

            SetDefaultView();
            IsBusy = false;
        }

        public void RevertChanges()
        {
            SetDefaultView();
        }

        public void BeginSelectLocation()
        {
            SelectedLocations.Clear();
            IsInBrowseMode = false;
            IsInEditMode = false;
            IsInSelectMode = true;
            SelectionMode = SelectionMode.Multiple;
            IsItemClickEnabled = false;
        }

        public void GoToAppSettings()
        {
            _navigationService.GoTo(AppViews.Properties, GroupBy);
        }

        public void OnLocationSelected(object sender, object e)
        {
            SelectedLocations = _platformSpecificActions.GetSelectedItems<Location>(sender);
        }

        public async Task RemoveSelectedLocations()
        {
            if (SelectedLocations.Count == 0)
            {
                await _dialogService.ShowAsync("Please select at least one location.");
                return;
            }

            var response = await _dialogService.ShowConfirmationAsync("Do you really want to remove the selected locations?");
            if (response == ConfirmationAnswer.Cancel) return;

            IsBusy = true;
            var selectedLocations = SelectedLocations.ToList();
            foreach (var selLoc in selectedLocations)
            {
                var snapshots = await _locationSnapshotDataService.GetSnapshotsByLocationIdAsync(selLoc.Id);
                var deletionTasks = snapshots.Select(async _ =>
                {
                    await _locationSnapshotDataService.RemoveSnapshotAsync(_.Id);
                    await _pictureService.RemoveSnapshotContentAsync(_);
                });
                await Task.WhenAll(deletionTasks);
                var removedLocation = await _locationDataService.RemoveLocationAsync(selLoc.Id);
                Locations.Remove(Locations.First(_ => _.Id == removedLocation.Id));
            };
            SetDefaultView();
            IsBusy = false;
        }

        public async Task ImportSelectedLocations()
        {
            if ((await _dataSourceGovernor.GetCurrentDataSourceTypeAsync()) == DataSourceType.Remote)
            {
                await _dialogService.ShowAsync("You are currently using the LocationCapture web API as a data source.\r\nPlease switch to the local data source to synchronize locations.");
                return;
            }

            if (SelectedLocations.Count == 0)
            {
                await _dialogService.ShowAsync("Please select at least one location.");
                return;
            }

            IsBusy = true;
            if (!(await _connectivityService.IsWebApiAvailableAsync()))
            {
                await _dialogService.ShowAsync("The LocationCapture web API is not available. Synchronization failed.");
                IsBusy = false;
                return;
            }

            IsImportInProgress = true;

            _importCancellationTokenSource = new CancellationTokenSource();
            await _locationDataImporter.ImportAsync(SelectedLocations, _importCancellationTokenSource.Token);
            _importCancellationTokenSource.Dispose();
            SetDefaultView();

            IsBusy = false;
            IsImportInProgress = false;
        }

        public void StopLocationsImport()
        {
            _importCancellationTokenSource.Cancel();
        }

        private void SetDefaultView()
        {
            IsInBrowseMode = true;
            IsInEditMode = false;
            IsInSelectMode = false;
            SelectionMode = SelectionMode.None;
            IsItemClickEnabled = true;
            SelectedLocation = null;
        }

        private async Task GroupByChanged()
        {
            if (IsBusy) return;

            IsBusy = true;

            IsViewGrouped = (GroupBy != GroupByCriteria.None);
            SetDefaultView();
            if (GroupBy == GroupByCriteria.None)
            {
                var locations = await _locationDataService.GetAllLocationsAsync();
                Locations = new ObservableCollection<Location>(locations);
            }
            else
            {
                var dataFetchOperation = _locationSnapshotDataService.ChooseGroupByOperation(GroupBy);
                var snapshotGroups = await dataFetchOperation();
                SnapshotsGroups = new ObservableCollection<SnapshotGroup>(snapshotGroups);
            }

            IsBusy = false;
        }

        public void GoBack()
        {
            SetDefaultView();
        }

        public void OnLocationClicked(object sender, object e)
        {
            var clickedLocation = _platformSpecificActions.GetClickedItem<Location>(e);
            NavigateToSnapshots(clickedLocation);
        }

        public void OnSnapshotGroupClicked(object sender, object e)
        {
            var clickedSnapshotGroup = _platformSpecificActions.GetClickedItem<SnapshotGroup>(e);
            NavigateToSnapshots(clickedSnapshotGroup);
        }

        private void NavigateToSnapshots(object payload)
        {
            var navParam = new SnapshotsViewNavParams
            {
                GroupByCriteria = GroupBy,
                SelectedLocation = payload is Location ? payload as Location : null,
                SelectedGroup = payload is SnapshotGroup ? payload as SnapshotGroup : null,
            };
            _navigationService.GoTo(AppViews.Snapshots, navParam);
        }

        public async Task OnNavigatedTo()
        {
            await OnLoaded();
        }

        public async Task SaveState()
        {
            var appState = new AppState
            {
                CurrentView = AppViews.Locations,
                NavigationParam = GroupBy
            };

            await _appStateProvider.SaveAppStateAsync(appState);
        }
    }
}