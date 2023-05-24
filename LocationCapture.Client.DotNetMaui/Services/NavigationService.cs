using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.DotNetMaui.Views;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class NavigationService : INavigationService
    {
        public NavigationService(){  }

        public async void GoTo(AppViews navTarget, object navParam = null)
        {
            var target = $"{navTarget}View";

            if (navParam != null)
            {
                var parameters = new Dictionary<string, object>
                    {
                        { nameof(INavigationTarget.NavigationParam), navParam }
                    };

                await Shell.Current.GoToAsync(target, parameters);
            }
            else
            {
                await Shell.Current.GoToAsync(target);
            }
        }

        public static void RegisterRoutes()
        {
            Routing.RegisterRoute($"{nameof(AppViews.Locations)}View", typeof(LocationsView));
            Routing.RegisterRoute($"{nameof(AppViews.Snapshots)}View", typeof(SnapshotsView));
            Routing.RegisterRoute($"{nameof(AppViews.Properties)}View", typeof(PropertiesView));
            Routing.RegisterRoute($"{nameof(AppViews.SnapshotDetails)}View", typeof(SnapshotDetailsView));
            Routing.RegisterRoute($"{nameof(AppViews.Camera)}View", typeof(CameraView));
            Routing.RegisterRoute($"{nameof(AppViews.Weather)}View", typeof(WeatherView));
            Routing.RegisterRoute($"{nameof(AppViews.Logs)}View", typeof(LogsView));
            Routing.RegisterRoute($"{nameof(AppViews.Suggestions)}View", typeof(SuggestionsView));
            Routing.RegisterRoute($"{nameof(AppViews.SnapshotExportImport)}View", typeof(SnapshotExportImportView));
        }
    }
}
