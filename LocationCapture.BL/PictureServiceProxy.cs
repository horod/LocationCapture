using LocationCapture.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public class PictureServiceProxy : IPictureService
    {
        private string SnapshotPicturesRoute => "snapshot-pictures";

        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IWebClient _webClient;
        private readonly IMiniaturesCache _miniaturesCache;
        private ConcurrentDictionary<int, SnapshotMiniature> _snapshotMiniaturesCache = new ConcurrentDictionary<int, SnapshotMiniature>();

        public PictureServiceProxy(IAppSettingsProvider appSettingsProvider,
            IWebClient webClient,
            IMiniaturesCache miniaturesCache)
        {
            _appSettingsProvider = appSettingsProvider;
            _webClient = webClient;
            _miniaturesCache = miniaturesCache;
        }

        private async Task<string> GetPicturesUrl()
        {
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();
            return string.Join("/", appSettings.LocationCaptureApiUri, SnapshotPicturesRoute);
        }

        public async Task<byte[]> GetSnapshotContentAsync(LocationSnapshot snapshot)
        {
            return await GetSnapshotContentAsync(snapshot, false);
        }

        private async Task<byte[]> GetSnapshotContentAsync(LocationSnapshot snapshot, bool isMiniature)
        {
            var picturesUrl = await GetPicturesUrl();
            var queryString = isMiniature ? $"?isMiniature={isMiniature}" : string.Empty;
            picturesUrl = $"{picturesUrl}/{snapshot.PictureFileName}{queryString}";

            var result = await _webClient.GetStringAsync(picturesUrl);
            var picture = Convert.FromBase64String(result);

            return picture;
        }

        public async Task<IEnumerable<SnapshotMiniature>> GetSnapshotMiniaturesAsync(IEnumerable<LocationSnapshot> snapshots)
        {
            if (!snapshots.Any()) return new List<SnapshotMiniature>();

            var miniatures = _miniaturesCache.GetSnapshotMiniatures(snapshots);
            if (miniatures.Count() == snapshots.Count()) return miniatures;

            var picturesUrl = await GetPicturesUrl();
            picturesUrl = $"{picturesUrl}?snapshotsIds={string.Join(",", snapshots.Select(_ => _.Id))}";

            var result = await _webClient.GetAsync<IEnumerable<ExpandoObject>>(picturesUrl);

            miniatures = result.Select(_ =>
                {
                    var descriptor = (IDictionary<string, object>)_;
                    return new
                    {
                        PictureFileName = descriptor["pictureFileName"].ToString(),
                        Thumbnail = descriptor["thumbnail"].ToString()
                    };
                }).Join(snapshots, desc => desc.PictureFileName, snap => snap.PictureFileName, (desc, snap) => 
                {
                    var miniature =  new SnapshotMiniature
                    {
                        Snapshot = snap,
                        Data = Convert.FromBase64String(desc.Thumbnail)
                    };

                    _miniaturesCache.AddSnapshotMiniature(miniature);

                    return miniature;
                })
                .ToList();

            return miniatures;
        }

        public async Task RemoveSnapshotContentAsync(LocationSnapshot snapshot)
        {
            _miniaturesCache.RemoveSnapshotMiniature(snapshot.Id);

            var picturesUrl = await GetPicturesUrl();
            picturesUrl = $"{picturesUrl}/{snapshot.PictureFileName}";

            await _webClient.DeleteAsync<object>(picturesUrl);
        }

        public async Task<int> SaveSnapshotContentAsync(LocationSnapshot snapshot, byte[] data)
        {
            var picturesUrl = await GetPicturesUrl();

            dynamic payload = new ExpandoObject();
            payload.pictureFile = Convert.ToBase64String(data);
            payload.pictureFileName = snapshot.PictureFileName;

            await _webClient.PostAsync<object, object>(picturesUrl, payload);
            var miniatureBytes = await GetSnapshotContentAsync(snapshot, true);
            _miniaturesCache.AddSnapshotMiniature(new SnapshotMiniature { Snapshot = snapshot, Data = miniatureBytes});

            return data.Length;
        }
    }
}
