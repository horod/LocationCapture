using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class GeolocationViewModel : NotificationBase, INavigationTarget
    {
        private readonly INavigationService _navigationService;
        private readonly IMapService _mapService;

        public object NavigationParam { get; set; }

        public GeolocationViewModel(INavigationService navigationService,
            IMapService mapService)
        {
            _navigationService = navigationService;
            _mapService = mapService;
        }

        public async void OnMapControlLoaded(object sender, object e)
        {
            await _mapService.SetMapControlAsync(sender);
        }

        public async Task OnLoaded()
        {
            var navParam = (SnapshotDetailsViewNavParams)NavigationParam;
            var snapshot = navParam.LocationSnapshot;
            await _mapService.ShowLocation(snapshot);
        }

        public void GoBack()
        {
            _mapService.ReleaseMapControl();
            _navigationService.GoTo(AppViews.SnapshotDetails, NavigationParam);
        }

        public async Task OnNavigatedTo()
        {
            await OnLoaded();
        }
    }
}