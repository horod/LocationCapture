using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using Location = Microsoft.Maui.Devices.Sensors.Location;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class MapService : IMapService
    {
        public void ReleaseMapControl()
        {
            
        }

        public Task SetMapControlAsync(object mapControl)
        {
            return Task.CompletedTask;
        }

        public async Task ShowLocation(LocationSnapshot snapshot)
        {
            var location = new Location(snapshot.Latitude, snapshot.Longitude);

            try
            {
                await Map.Default.OpenAsync(location);
            }
            catch (Exception)
            {
                // No map application available to open
            }
        }
    }
}
