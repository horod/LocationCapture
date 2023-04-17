using LocationCapture.Client.MVVM.Models;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface IAppStateProvider
    {
        Task<AppState> GetAppStateAsync();

        Task SaveAppStateAsync(AppState appSettings);
    }
}
