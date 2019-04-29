using LocationCapture.Models;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface ILocationService
    {
        Task<LocationDescriptor> GetCurrentLocationAsync();
    }
}
