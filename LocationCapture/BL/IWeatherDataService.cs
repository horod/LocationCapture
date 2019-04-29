using LocationCapture.Models;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public interface IWeatherDataService
    {
        Task<WeatherData> GetWeatherDataForLocationAsync(LocationSnapshot locationSnapshot);
    }
}
