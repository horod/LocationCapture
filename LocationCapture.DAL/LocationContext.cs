using System.Collections.Generic;
using LocationCapture.Models;
using Microsoft.EntityFrameworkCore;

namespace LocationCapture.DAL
{
    public class LocationContext : ILocationContext
    {
        private readonly LocationDbContext _dbContext;

        public LocationContext(LocationDbContext locationDbContext)
        {
            _dbContext = locationDbContext;
        }

        public IEnumerable<Location> Locations => _dbContext.Locations.Include(_ => _.LocationSnapshots);

        public IEnumerable<LocationSnapshot> LocationSnapshots => _dbContext.LocationSnapshots;

        public TEntity Add<TEntity>(TEntity entityToAdd) where TEntity : class
        {
            _dbContext.Add(entityToAdd);
            _dbContext.SaveChanges();
            return entityToAdd;
        }

        public TEntity Remove<TEntity>(TEntity entityToRemove) where TEntity : class
        {
            _dbContext.Remove(entityToRemove);
            _dbContext.SaveChanges();
            return entityToRemove;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
