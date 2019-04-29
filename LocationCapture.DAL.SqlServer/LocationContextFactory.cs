using LocationCapture.Models;

namespace LocationCapture.DAL.SqlServer
{
    public class LocationContextFactory : ILocationContextFactory
    {
        private readonly AppSettings _appSettings;

        public LocationContextFactory(Microsoft.Extensions.Options.IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public ILocationContext Create()
        {
            var dbContext = new SqlServerLocationDbContext(_appSettings.DbConnectionString);
            return new LocationContext(dbContext);
        }
    }
}
