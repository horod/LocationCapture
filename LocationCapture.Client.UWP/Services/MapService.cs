using LocationCapture.Models;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using LocationCapture.BL;
using System.Threading.Tasks;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class MapService : IMapService
    {
        private MapControl _mapControl;
        private readonly IAppSettingsProvider _appSettingsProvider;

        public MapService(IAppSettingsProvider appSettingsProvider)
        {
            _appSettingsProvider = appSettingsProvider;
        }

        public async Task SetMapControlAsync(object mapControl)
        {
            _mapControl = (MapControl)mapControl;
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();
            _mapControl.MapServiceToken = appSettings.MapsApiKey;
        }

        public void ShowLocation(LocationSnapshot snapshot)
        {
            BasicGeoposition snapshotPosition = new BasicGeoposition { Latitude = snapshot.Latitude, Longitude = snapshot.Longitude };
            Geopoint snapshotPoint = new Geopoint(snapshotPosition);
            var snapshotIcon = new MapIcon
            {
                Location = snapshotPoint,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0
            };

            _mapControl.MapElements.Add(snapshotIcon);
            _mapControl.Center = snapshotPoint;
            _mapControl.ZoomLevel = 15;
        }

        public void ReleaseMapControl()
        {
            _mapControl = null;
        }
    }
}
