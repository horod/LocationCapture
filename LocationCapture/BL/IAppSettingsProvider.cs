using LocationCapture.Models;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public interface IAppSettingsProvider
    {
        Task<AppSettings> GetAppSettingsAsync();
        Task SaveAppSettingsAsync(AppSettings appSettings);
    }
}
