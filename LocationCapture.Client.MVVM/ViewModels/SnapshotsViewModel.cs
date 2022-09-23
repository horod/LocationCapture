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
using System.Threading.Tasks;
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

        private string _LocationInfo;
        public string LocationInfo
        {
            get { return _LocationInfo; }
            set { SetProperty(ref _LocationInfo, value); }
        }

        public bool CanAddSnapshot => _groupByCriteria == GroupByCriteria.None && !IsInSelectMode;

        public SnapshotsViewModel(ILocationSnapshotDataService locationSnapshotDataService,
            INavigationService navigationService,
            IPictureService pictureService,
            IBitmapConverter bitmapConverter,
            IDialogService dialogService,
            IPlatformSpecificActions platformSpecificActions)
        {
            _locationSnapshotDataService = locationSnapshotDataService;
            _navigationService = navigationService;
            _pictureService = pictureService;
            _bitmapConverter = bitmapConverter;
            _dialogService = dialogService;
            _platformSpecificActions = platformSpecificActions;

            SnapshotThumbnails = new ObservableCollection<SnapshotThumbnail>();
            SelectedThumbnails = new List<SnapshotThumbnail>();
            SelectionMode = SelectionMode.None;
            IsItemClickEnabled = true;
        }

        public async Task OnLoaded()
        {
            IsBusy = true;
            var navParam = (SnapshotsViewNavParams)NavigationParam;
            _groupByCriteria = navParam.GroupByCriteria;
            var payload = navParam.SnapshotsIdsource;
            Parent = payload;
            IEnumerable<LocationSnapshot> snapshots = null;

            RaisePropertyChanged(nameof(CanAddSnapshot));

            if (payload is Location location)
            {
                snapshots = await _locationSnapshotDataService.GetSnapshotsByLocationIdAsync(location.Id);
                LocationInfo = $"{location.Name} ({snapshots.Count()})";
            }
            else if (payload is SnapshotGroup group)
            {
                snapshots = await _locationSnapshotDataService.GetSnapshotsByIdsAsync(group.SnapshotIds);
                LocationInfo = $"{group.Name} ({snapshots.Count()})";
            }

            IsBusy = false;

            foreach (var snapshot in snapshots)
            {
                var miniature = await _pictureService.GetSnapshotMiniatureAsync(snapshot);
                var thumbnail = await _bitmapConverter.GetBitmapAsync(miniature.Data);
                SnapshotThumbnails.Add(new SnapshotThumbnail { Snapshot = miniature.Snapshot, Thumbnail = thumbnail });
            }
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
            SetDefaultView();
            IsBusy = false;
        }

        public void GoBack()
        {
            if(!IsInSelectMode)
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
                SnapshotsViewState = NavigationParam
            };
            _navigationService.GoTo(AppViews.SnapshotDetails, navParam);
        }
    }
}
