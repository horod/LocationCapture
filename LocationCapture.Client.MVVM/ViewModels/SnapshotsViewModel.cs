using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Enums;
using LocationCapture.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Location = LocationCapture.Models.Location;
using SelectionMode = LocationCapture.Client.MVVM.Enums.SelectionMode;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class SnapshotsViewModel : NotificationBase, INavigationTarget
    {
        private GroupByCriteria _groupByCriteria;
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly INavigationService _navigationService;
        private readonly IPictureService _pictureService;
        private readonly IBitmapConverter _bitmapConverter;
        private readonly IDialogService _dialogService;
        private readonly IPlatformSpecificActions _platformSpecificActions;
        private readonly IAppStateProvider _appStateProvider;
        private readonly ISnapshotPackageManager _packageManager;
        private readonly IFilePickerService _filePickerService;
        private readonly ILoggingService _loggingService;
        private ICollection<SnapshoNavigationMapEntry> _snapshoNavigationMap;

        public object NavigationParam { get; set; }

        private object _Parent;
        public object Parent
        {
            get { return _Parent; }
            set { SetProperty(ref _Parent, value); }
        }

        private ObservableCollection<SnapshotThumbnail> _SnapshotThumbnails;
        public ObservableCollection<SnapshotThumbnail> SnapshotThumbnails
        {
            get { return _SnapshotThumbnails; }
            set { SetProperty(ref _SnapshotThumbnails, value); }
        }

        private bool _IsInSelectMode;
        public bool IsInSelectMode
        {
            get { return _IsInSelectMode; }
            set
            {
                SetProperty(ref _IsInSelectMode, value);
                RaisePropertyChanged(nameof(CanAddSnapshot));
            }
        }

        private SelectionMode _SelectionMode;
        public SelectionMode SelectionMode
        {
            get { return _SelectionMode; }
            set { SetProperty(ref _SelectionMode, value); }
        }

        private List<SnapshotThumbnail> _SelectedThumbnails;
        public List<SnapshotThumbnail> SelectedThumbnails
        {
            get { return _SelectedThumbnails; }
            set { SetProperty(ref _SelectedThumbnails, value); }
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

        public string LocationInfo => Parent switch
        {
            Location location => $"{location.Name} ({SnapshotThumbnails.Count})",
            SnapshotGroup group => $"{group.Name} ({SnapshotThumbnails.Count})",
            _ => string.Empty
        };

        public bool CanAddSnapshot => _groupByCriteria == GroupByCriteria.None && !IsInSelectMode;

        public SnapshotsViewModel(ILocationSnapshotDataService locationSnapshotDataService,
            INavigationService navigationService,
            IPictureService pictureService,
            IBitmapConverter bitmapConverter,
            IDialogService dialogService,
            IPlatformSpecificActions platformSpecificActions,
            IAppStateProvider appStateProvider,
            ISnapshotPackageManager packageManager,
            IFilePickerService filePickerService,
            ILoggingService loggingService)
        {
            _locationSnapshotDataService = locationSnapshotDataService;
            _navigationService = navigationService;
            _pictureService = pictureService;
            _bitmapConverter = bitmapConverter;
            _dialogService = dialogService;
            _platformSpecificActions = platformSpecificActions;
            _appStateProvider = appStateProvider;
            _packageManager = packageManager;
            _filePickerService = filePickerService;
            _loggingService = loggingService;

            SnapshotThumbnails = new ObservableCollection<SnapshotThumbnail>();
            SelectedThumbnails = new List<SnapshotThumbnail>();
            SelectionMode = SelectionMode.None;
            IsItemClickEnabled = true;
        }

        public async Task OnLoaded()
        {
            IsBusy = true;

            var navParam = (SnapshotsViewNavParams)NavigationParam;
            List<LocationSnapshot> snapshots = null;

            try
            {
                _groupByCriteria = navParam.GroupByCriteria;
                var payload = (object)navParam.SelectedLocation ?? navParam.SelectedGroup;
                Parent = payload;

                RaisePropertyChanged(nameof(CanAddSnapshot));

                if (payload is Location location)
                {
                    snapshots = (await _locationSnapshotDataService.GetSnapshotsByLocationIdAsync(location.Id))
                        .ToList();
                }
                else if (payload is SnapshotGroup group)
                {
                    snapshots = (await _locationSnapshotDataService.GetSnapshotsByIdsAsync(group.SnapshotIds))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                var snapshotsLoadingError = "Could not load snapshots.";
                _loggingService.Warning(snapshotsLoadingError + " Details: {Ex}", ex);
                await _dialogService.ShowAsync(snapshotsLoadingError);
                GoBack();
                return;
            }

            IsBusy = false;

            for (var i = 0; i < snapshots.Count; i++)
            {
                var snapshot = snapshots[i];
                int previousSnapshotId = 0, nextSnapshotId = 0;
                
                if (i > 0)
                {
                    previousSnapshotId = snapshots[i - 1].Id;
                }

                if (i < snapshots.Count - 1)
                {
                    nextSnapshotId = snapshots[i + 1].Id;
                }

                var miniature = await _pictureService.GetSnapshotMiniatureAsync(snapshot);
                var thumbnail = await _bitmapConverter.GetBitmapAsync(miniature.Data);
                
                SnapshotThumbnails.Add(new SnapshotThumbnail { Snapshot = miniature.Snapshot, Thumbnail = thumbnail, PreviousSnapshotId = previousSnapshotId, NextSnapshotId = nextSnapshotId });
            }

            _snapshoNavigationMap = SnapshotThumbnails.Select(x => new SnapshoNavigationMapEntry
            {
                SnapshotId = x.Snapshot.Id,
                NextSnapshotId = x.NextSnapshotId,
                PreviousSnapshotId = x.PreviousSnapshotId
            }).ToList();

            RaisePropertyChanged(nameof(LocationInfo));
        }

        public void BeginSelectSnapshot()
        {
            SelectedThumbnails.Clear();
            IsInSelectMode = true;
            SelectionMode = SelectionMode.Multiple;
            IsItemClickEnabled = false;
        }

        public void OnSnapshotSelected(object sender, object e)
        {
            SelectedThumbnails = _platformSpecificActions.GetSelectedItems<SnapshotThumbnail>(sender);
        }

        public async Task RemoveSelectedSnapshots()
        {
            if (SelectedThumbnails.Count == 0)
            {
                await _dialogService.ShowAsync("Please select at least one snapshot.");
                return;
            }

            var response = await _dialogService.ShowConfirmationAsync("Do you really want to remove the selected snapshots?");
            if (response == ConfirmationAnswer.Cancel) return;

            IsBusy = true;
            var selectedThumbnails = SelectedThumbnails.ToList();
            foreach (var selThumb in selectedThumbnails)
            {
                var removedSnapshot = await _locationSnapshotDataService.RemoveSnapshotAsync(selThumb.Snapshot.Id);
                await _pictureService.RemoveSnapshotContentAsync(removedSnapshot);
                SnapshotThumbnails.Remove(SnapshotThumbnails.First(_ => _.Snapshot.Id == removedSnapshot.Id));
            };
            RaisePropertyChanged(nameof(LocationInfo));
            SetDefaultView();
            IsBusy = false;
        }

        public async Task ExportSelectedSnapshots()
        {
            if (SelectedThumbnails.Count == 0 || SelectedThumbnails.Count > 10)
            {
                await _dialogService.ShowAsync("Please select at least one, but no more than 10 snapshots.");
                return;
            }

            IsBusy = true;

            var packagePath = await _packageManager.CompressSnapshots(SelectedThumbnails.Select(x => x.Snapshot).ToList());
            var navParam = new SnapshotExportImportViewNavParams
            {
                Mode = SnapshotExportImportMode.Export,
                PackagePath = packagePath,
                SnapshotsViewState = (SnapshotsViewNavParams)NavigationParam
            };

            IsBusy = false;

            _navigationService.GoTo(AppViews.SnapshotExportImport, navParam);
        }

        public async Task ImportSnapshots()
        {
            var packagePath = await _filePickerService.PickAsync("Select a snapshot package to import", FileType.Zip);

            if (string.IsNullOrEmpty(packagePath)) return;

            IsBusy = true;

            _packageManager.DecompressPackage(packagePath);

            var navParam = new SnapshotExportImportViewNavParams
            {
                Mode = SnapshotExportImportMode.Import,
                PackagePath = packagePath,
                SnapshotsViewState = (SnapshotsViewNavParams)NavigationParam
            };

            IsBusy = false;

            _navigationService.GoTo(AppViews.SnapshotExportImport, navParam);
        }

        public void GoBack()
        {
            if (!IsInSelectMode)
            {
                _navigationService.GoTo(AppViews.Locations, _groupByCriteria);
            }
            else
            {
                SetDefaultView();
            }
        }

        private void SetDefaultView()
        {
            IsInSelectMode = false;
            SelectionMode = SelectionMode.None;
            IsItemClickEnabled = true;
        }

        public void OnSnapshotAdding()
        {
            _navigationService.GoTo(AppViews.Camera, NavigationParam);
        }

        public void OnSnapshotClicked(object sender, object e)
        {
            var clickedThumbnail = _platformSpecificActions.GetClickedItem<SnapshotThumbnail>(e);
            var navParam = new SnapshotDetailsViewNavParams
            {
                LocationSnapshot = clickedThumbnail.Snapshot,
                SnapshoNavigationMap = _snapshoNavigationMap,
                SnapshotsViewState = (SnapshotsViewNavParams)NavigationParam
            };
            _navigationService.GoTo(AppViews.SnapshotDetails, navParam);
        }

        public async Task OnNavigatedTo()
        {
            await OnLoaded();
        }

        public async Task SaveState()
        {
            var appState = new AppState
            {
                CurrentView = AppViews.Snapshots,
                NavigationParam = new SnapshotsViewNavParams
                {
                    GroupByCriteria = _groupByCriteria,
                    SelectedLocation = Parent as Location,
                    SelectedGroup = Parent as SnapshotGroup
                }
            };

            await _appStateProvider.SaveAppStateAsync(appState);
        }
    }
}