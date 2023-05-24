using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using System;
using System.Threading.Tasks;

namespace LocationCapture.Client.UWP.Services
{
    public class AppStateProvider : IAppStateProvider
    {
        public Task<AppState> GetAppStateAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveAppStateAsync(AppState appSettings)
        {
            throw new NotImplementedException();
        }
    }
}
