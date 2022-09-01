using LocationCapture.Models;
using Microsoft.EntityFrameworkCore;

namespace LocationCapture.DAL
{
    public abstract class LocationDbContext : DbContext
    {
        protected string DbIdentifier;

        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationSnapshot> LocationSnapshots { get; set; }

        protected LocationDbContext() : base() { }

        protected LocationDbContext(string dbIdentifier)
        {
            DbIdentifier = dbIdentifier;
        }
    }
}
