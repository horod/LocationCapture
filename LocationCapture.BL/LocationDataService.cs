using System.Collections.Generic;
using LocationCapture.Models;
using LocationCapture.DAL;
using System.Linq;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public class LocationDataService : ILocationDataService
    {
        private readonly ILocationContextFactory _dataContextFactory;

        public LocationDataService(ILocationContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        private IEnumerable<Location> GetAllLocations()
        {
            using (var context = _dataContextFactory.Create())
            {
                return context.Locations
                    .ToList();
            }
        }

        private Location AddLocation(Location locationToAdd)
        {
            using (var context = _dataContextFactory.Create())
            {
                return context.Add(locationToAdd);
            }
        }

        private Location RenameLocation(int locationId, string newName)
        {
            using (var context = _dataContextFactory.Create())
            {
                var locationToRename = context.Locations
                    .First(_ => _.Id == locationId);
                locationToRename.Name = newName;
                context.SaveChanges();

                return locationToRename;
            }
        }

        private Location RemoveLocation(int locationId)
        {
            using (var context = _dataContextFactory.Create())
            {
                var locationToRemove = context.Locations
                    .First(_ => _.Id == locationId);

                return context.Remove(locationToRemove);
            }
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            var result = await Task.Factory.StartNew(() => GetAllLocations());
            return result;
        }

        public async Task<Location> AddLocationAsync(Location locationToAdd)
        {
            var result = await Task.Factory.StartNew(() => AddLocation(locationToAdd));
            return result;
        }

        public async Task<Location> RenameLocationAsync(int locationId, string newName)
        {
            var result = await Task.Factory.StartNew(() => RenameLocation(locationId, newName));
            return result;
        }

        public async Task<Location> RemoveLocationAsync(int locationId)
        {
            var result = await Task.Factory.StartNew(() => RemoveLocation(locationId));
            return result;
        }
    }
}
