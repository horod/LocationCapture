using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface IConnectivityService
    {
        bool IsInternetAvailable();
        Task<bool> IsWebApiAvailableAsync();
    }
}
