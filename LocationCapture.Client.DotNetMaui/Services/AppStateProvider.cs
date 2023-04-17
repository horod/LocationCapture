using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class AppStateProvider : IAppStateProvider
    {
        private const string AppStateFileName = "appState.json";

        private AppState _appState;

        public async Task<AppState> GetAppStateAsync()
        {
            if (_appState != null) return _appState.Clone();
            return await ReadAppState();
        }

        public async Task SaveAppStateAsync(AppState appState)
        {
            await WriteAppState(appState);
            _appState = appState;
        }

        private async Task<AppState> ReadAppState()
        {
            AppState appState;

            var appStatePath = Path.Combine(FileSystem.Current.AppDataDirectory, AppStateFileName);

            if (!File.Exists(appStatePath))
            {
                appState = GenerateDefaultAppState();
                _appState = appState;
                await WriteAppState(appState);
                return appState;
            }

            var appStateAsJson = await File.ReadAllTextAsync(appStatePath);
            appState = JsonSerializer.Deserialize<AppState>(appStateAsJson);

            var navParamAsJson = appState.NavigationParam.ToString();
            appState.NavigationParam = appState.CurrentView switch
            {
                AppViews.Locations => JsonSerializer.Deserialize<GroupByCriteria>(navParamAsJson),
                AppViews.Logs => JsonSerializer.Deserialize<GroupByCriteria>(navParamAsJson),
                AppViews.Properties => JsonSerializer.Deserialize<GroupByCriteria>(navParamAsJson),
                AppViews.Snapshots => JsonSerializer.Deserialize<SnapshotsViewNavParams>(navParamAsJson),
                AppViews.Camera => JsonSerializer.Deserialize<SnapshotsViewNavParams>(navParamAsJson),
                AppViews.SnapshotDetails => JsonSerializer.Deserialize<SnapshotDetailsViewNavParams>(navParamAsJson),
                AppViews.Weather => JsonSerializer.Deserialize<SnapshotDetailsViewNavParams>(navParamAsJson),
                AppViews.Suggestions => JsonSerializer.Deserialize<SuggestionsViewNavParams>(navParamAsJson),
                _ => throw new ArgumentOutOfRangeException(appState.CurrentView.ToString())
            };

            _appState = appState;

            return appState;
        }

        private async Task WriteAppState(AppState appState)
        {
            var appStatePath = Path.Combine(FileSystem.Current.AppDataDirectory, AppStateFileName);

            await File.WriteAllTextAsync(appStatePath, JsonSerializer.Serialize(appState));
        }

        private AppState GenerateDefaultAppState()
        {
            return new AppState
            {
                CurrentView = AppViews.Locations,
                NavigationParam = GroupByCriteria.None
            };
        }
    }
}
