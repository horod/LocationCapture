using LocationCapture.Models;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface IMapService
    {
        Task SetMapControlAsync(object mapControl);

        void ShowLocation(LocationSnapshot snapshot);

        void ReleaseMapControl();
    }
}
