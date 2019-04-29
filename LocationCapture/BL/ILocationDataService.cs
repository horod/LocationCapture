using LocationCapture.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public interface ILocationDataService
    {
        Task<IEnumerable<Location>> GetAllLocationsAsync();

        Task<Location> AddLocationAsync(Location locationToAdd);

        Task<Location> RenameLocationAsync(int locationId, string newName);

        Task<Location> RemoveLocationAsync(int locationId);
    }
}
