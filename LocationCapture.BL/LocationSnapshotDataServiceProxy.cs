using LocationCapture.Enums;
using LocationCapture.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public class LocationSnapshotDataServiceProxy : ILocationSnapshotDataService
    {
        private static string SnapshotsRoute = "snapshots";
        private static string SnapshotGroupsRoute = "snapshot-groups";

        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IWebClient _webClient;

        public LocationSnapshotDataServiceProxy(IAppSettingsProvider appSettingsProvider,
            IWebClient webClient)
        {
            _appSettingsProvider = appSettingsProvider;
            _webClient = webClient;
        }

        private async Task<string> GetSnapshotsUrl()
        {
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();
            return string.Join("/", appSettings.LocationCaptureApiUri, SnapshotsRoute);
        }

        private async Task<string> GetGroupsUrl()
        {
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();
            return string.Join("/", appSettings.LocationCaptureApiUri, SnapshotGroupsRoute);
        }

        public async Task<LocationSnapshot> AddSnapshotAsync(LocationSnapshot snapshotToAdd)
        {
            var snapshotsUrl = await GetSnapshotsUrl();
            var result = await _webClient.PostAsync<LocationSnapshot, LocationSnapshot>(snapshotsUrl, snapshotToAdd);

            return result;
        }

        public Func<Task<IEnumerable<SnapshotGroup>>> ChooseGroupByOperation(GroupByCriteria groupBy)
        {
            var groups = Task.Run(async () => await GetSnapshotGroups(groupBy).ConfigureAwait(false)).Result;

            return new Func<Task<IEnumerable<SnapshotGroup>>>(() => Task.FromResult(groups));
        }

        private async Task<IEnumerable<SnapshotGroup>> GetSnapshotGroups(GroupByCriteria groupBy)
        {
            var groupsUrl = await GetGroupsUrl();
            groupsUrl = $"{groupsUrl}?groupBy={groupBy.ToString()}";
            var result = await _webClient.GetAsync<IEnumerable<SnapshotGroup>>(groupsUrl);

            return result;
        }

        public async Task<IEnumerable<LocationSnapshot>> GetSnapshotsByIdsAsync(IEnumerable<int> snapshotIds)
        {
            var snapshotsUrl = await GetSnapshotsUrl();
            snapshotsUrl = $"{snapshotsUrl}/by-ids?snapshotsIds={string.Join(",", snapshotIds)}";
            var result = await _webClient.GetAsync<IEnumerable<LocationSnapshot>>(snapshotsUrl);

            return result;
        }

        public async Task<IEnumerable<LocationSnapshot>> GetSnapshotsByLocationIdAsync(int locationId)
        {
            var snapshotsUrl = await GetSnapshotsUrl();
            snapshotsUrl = $"{snapshotsUrl}/by-location-id?locationId={locationId}";
            var result = await _webClient.GetAsync<IEnumerable<LocationSnapshot>>(snapshotsUrl);

            return result;
        }

        public async Task<LocationSnapshot> RemoveSnapshotAsync(int snapshotId)
        {
            var snapshotsUrl = await GetSnapshotsUrl();
            snapshotsUrl = $"{snapshotsUrl}/{snapshotId}";
            var result = await _webClient.DeleteAsync<LocationSnapshot>(snapshotsUrl);

            return result;
        }

        public async Task<LocationSnapshot> UpdateSnapshotAsync(LocationSnapshot snapshot)
        {
            var snapshotsUrl = await GetSnapshotsUrl();
            snapshotsUrl = $"{snapshotsUrl}/{snapshot.Id}";
            var result = await _webClient.PutAsync<LocationSnapshot, LocationSnapshot>(snapshotsUrl, snapshot);

            return result;
        }
    }
}
