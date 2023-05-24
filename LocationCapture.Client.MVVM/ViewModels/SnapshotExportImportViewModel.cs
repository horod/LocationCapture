using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class SnapshotExportImportViewModel : NotificationBase, INavigationTarget
    {
        private readonly ISnapshotPackageManager _packageManager;
        private readonly IAppStateProvider _appStateProvider;
        private readonly INavigationService _navigationService;
        private readonly IFileShareService _fileShareService;
        private readonly IPictureService _pictureService;
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly IBitmapConverter _bitmapConverter;
        private readonly ILoggingService _loggingService;
        private readonly IDialogService _dialogService;
        private string _packageName;

        private readonly string CorruptedPackageError = "Failed getting snapshots from the package folder. The package is corrupted or empty.";

        public object NavigationParam { get; set; }
        private SnapshotExportImportViewNavParams NavParam => (SnapshotExportImportViewNavParams)NavigationParam;

        public bool IsInImportMode => NavParam?.Mode == SnapshotExportImportMode.Import;
        public bool IsInExportMode => NavParam?.Mode == SnapshotExportImportMode.Export;

        private ObservableCollection<SnapshotExportImportDescriptor> _SnapshotDescriptors;
        public ObservableCollection<SnapshotExportImportDescriptor> SnapshotDescriptors
        {
            get { return _SnapshotDescriptors; }
            set { SetProperty(ref _SnapshotDescriptors, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        public SnapshotExportImportViewModel(ISnapshotPackageManager packageManager,
            IFileShareService fileShareService,
            INavigationService navigationService,
            IAppStateProvider appStateProvider,
            IPictureService pictureService,
            ILocationSnapshotDataService locationSnapshotDataService,
            IBitmapConverter bitmapConverter,
            ILoggingService loggingService,
            IDialogService dialogService)
        {
            _packageManager = packageManager;
            _appStateProvider = appStateProvider;
            _navigationService = navigationService;
            _fileShareService = fileShareService;
            _pictureService = pictureService;
            _locationSnapshotDataService = locationSnapshotDataService;
            _bitmapConverter = bitmapConverter;
            _loggingService = loggingService;
            _dialogService = dialogService;

            SnapshotDescriptors = new ObservableCollection<SnapshotExportImportDescriptor>();
        }

        public void GoBack()
        {
            _packageManager.RemovePackage(_packageName, IsInExportMode);
            
            _navigationService.GoTo(AppViews.Snapshots, NavParam.SnapshotsViewState);
        }

        public async Task OnNavigatedTo()
        {
            IsBusy = true;

            _packageName = Path.GetFileNameWithoutExtension(NavParam.PackagePath);

            ICollection<SnapshotExportImportDescriptor> descriptors = null;

            try
            {
                descriptors = await _packageManager.GetSnapshotsFromPackageFolder(_packageName);
            }
            catch (Exception ex)
            {
                _loggingService.Warning(CorruptedPackageError + " Details: {Ex}", ex);
            }

            if (descriptors == null || descriptors.Count == 0)
            {
                await _dialogService.ShowAsync(CorruptedPackageError);
                GoBack();
                return;
            }

            IsBusy = false;

            foreach (var descriptor in descriptors)
            {
                var miniature = await _pictureService.GetSnapshotMiniatureAsync(descriptor.Snapshot);
                var thumbnail = await _bitmapConverter.GetBitmapAsync(miniature.Data);
                descriptor.SnapshotThumbnail = thumbnail;

                SnapshotDescriptors.Add(descriptor);
            }

            RaisePropertyChanged(nameof(IsInExportMode));
            RaisePropertyChanged(nameof(IsInImportMode));
        }

        public async Task OnShareAllSnapshots()
        {
            await _fileShareService.ShareAsync("Share all snapshots", new List<string> { NavParam.PackagePath });
        }

        public async Task OnShareSelectedSnapshots(ICollection<SnapshotExportImportDescriptor> descriptors)
        {
            await _fileShareService.ShareAsync("Share selected snapshots", descriptors.Select(x => x.SnapshotPath).ToList());
        }

        public async Task OnSaveAllSnapshots()
        {
            await SaveImportedSnapshots(SnapshotDescriptors);
        }

        public async Task OnSaveSelectedSnapshots(ICollection<SnapshotExportImportDescriptor> descriptors)
        {
            await SaveImportedSnapshots(descriptors);
        }

        public async Task SaveImportedSnapshots(ICollection<SnapshotExportImportDescriptor> descriptors)
        {
            var snapshotExists = false;

            IsBusy = true;

            foreach (var descriptor in descriptors)
            {
                if (await _pictureService.SnapshotContentExists(descriptor.Snapshot))
                {
                    snapshotExists = true;
                    _loggingService.Warning($"Snapshot named {descriptor.Snapshot.PictureFileName} already exists.");
                    continue;
                }

                byte[] pictureData = null;

                try
                {
                    pictureData = await _packageManager.GetSnapshotContentFromPackageFolder(_packageName, descriptor.SnapshotPath);
                }
                catch (Exception ex)
                {
                    _loggingService.Warning(CorruptedPackageError + " Details: {Ex}", ex);
                }

                if (pictureData == null || pictureData.Length == 0)
                {
                    await _dialogService.ShowAsync(CorruptedPackageError);
                    break;
                }

                var snapshot = new LocationSnapshot
                {
                    LocationId = NavParam.SnapshotsViewState.SelectedLocation.Id,
                    DateCreated = descriptor.Snapshot.DateCreated,
                    Thumbnail = descriptor.Snapshot.Thumbnail,
                    PictureFileName = descriptor.Snapshot.PictureFileName,
                    Latitude = descriptor.Snapshot.Latitude,
                    Longitude = descriptor.Snapshot.Longitude,
                    Altitude = descriptor.Snapshot.Altitude,
                };

                var newSnapshot = await _locationSnapshotDataService.AddSnapshotAsync(snapshot);
                await _pictureService.SaveSnapshotContentAsync(newSnapshot, pictureData);
            }

            IsBusy = false;

            if (snapshotExists)
            {
                await _dialogService.ShowAsync("Some snapshots could not be saved. See the logs for details.");
            }
            else
            {
                await _dialogService.ShowAsync("Snapshots imported successfully.");
            }
        }

        public async Task SaveState()
        {
            var appState = new AppState
            {
                CurrentView = AppViews.SnapshotExportImport,
                NavigationParam = new SnapshotExportImportViewNavParams
                {
                    PackagePath = NavParam.PackagePath,
                    Mode = NavParam.Mode,
                    SnapshotsViewState = NavParam.SnapshotsViewState
                }
            };

            await _appStateProvider.SaveAppStateAsync(appState);
        }
    }
}
