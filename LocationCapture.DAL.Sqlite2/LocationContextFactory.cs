using LocationCapture.BL;

namespace LocationCapture.DAL.Sqlite2
{
    public class LocationContextFactory : ILocationContextFactory
    {
        private readonly IAppSettingsProvider _appSettingsProvider;

        public LocationContextFactory(IAppSettingsProvider appSettingsProvider)
        {
            _appSettingsProvider = appSettingsProvider;
        }

        public ILocationContext Create()
        {
            var appSettings = _appSettingsProvider.GetAppSettingsAsync().Result;
            var dbContext = new SqliteLocationDbContext(appSettings.DbConnectionString);
            return new LocationContext(dbContext);
        }
    }
}
