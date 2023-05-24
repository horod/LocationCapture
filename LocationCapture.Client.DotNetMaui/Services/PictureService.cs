using LocationCapture.BL;
using LocationCapture.Models;
using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class PictureService : IPictureService
    {
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly IMiniaturesCache _miniaturesCache;

        public PictureService(
            IMiniaturesCache miniaturesCache,
            ILocationSnapshotDataService locationSnapshotDataService)
        {
            _locationSnapshotDataService = locationSnapshotDataService;
            _miniaturesCache = miniaturesCache;
        }

        public async Task<byte[]> GetSnapshotContentAsync(LocationSnapshot snapshot)
        {
            try
            {
                var picturePath = Path.Combine(FileSystem.Current.AppDataDirectory, snapshot.PictureFileName);

                var result = await File.ReadAllBytesAsync(picturePath);

                return result;
            }
            catch
            {
                // Picture does not exist in the specified location
                return new byte[0];
            }
        }

        private async Task<SnapshotMiniature> GetSnapshotMiniatureAsync(LocationSnapshot snapshot, byte[] data)
        {
            IImage image;

            using (var ms = new MemoryStream(data))
            {
                image = PlatformImage.FromStream(ms);

                if (image != null)
                {
                    using var thumbnail = image?.Downsize(110, true);

                    var thumbnailData = await thumbnail?.AsBytesAsync();

                    return new SnapshotMiniature { Snapshot = snapshot, Data = thumbnailData };
                }
            }

            return new SnapshotMiniature { Snapshot = snapshot, Data = new byte[0] };
        }

        public Task<SnapshotMiniature> GetSnapshotMiniatureAsync(LocationSnapshot snapshot)
        {
            // Check if the miniature is already available in the cache
            var miniature = _miniaturesCache.GetSnapshotMiniature(snapshot);
            if (miniature?.Data?.Length > 0) return Task.FromResult(miniature);

            // If not, try decoding it from LocationSnapshot.Thumbnail
            var thumbnail = snapshot.Thumbnail;

            var thumbnailAsBytes = Convert.FromBase64String(thumbnail);

            miniature = new SnapshotMiniature { Snapshot = snapshot, Data = thumbnailAsBytes };

            _miniaturesCache.AddSnapshotMiniature(miniature);

            return Task.FromResult(miniature);
        }

        public Task<IEnumerable<SnapshotMiniature>> GetSnapshotMiniaturesAsync(IEnumerable<LocationSnapshot> snapshots)
        {
            throw new NotImplementedException();
        }

        public Task RemoveSnapshotContentAsync(LocationSnapshot snapshot)
        {
            _miniaturesCache.RemoveSnapshotMiniature(snapshot.Id);

            var picturePath = Path.Combine(FileSystem.Current.AppDataDirectory, snapshot.PictureFileName);

            try
            {
                File.Delete(picturePath);
            }
            catch
            {
                // Picture does not exist in the specified location
            }

            return Task.CompletedTask;
        }

        public async Task<int> SaveSnapshotContentAsync(LocationSnapshot snapshot, byte[] data)
        {
            var picturePath = Path.Combine(FileSystem.Current.AppDataDirectory, snapshot.PictureFileName);

            await File.WriteAllBytesAsync(picturePath, data);

            var miniature = await GetSnapshotMiniatureAsync(snapshot, data);

            _miniaturesCache.AddSnapshotMiniature(miniature);

            var thumbnailAsString = Convert.ToBase64String(miniature.Data);

            snapshot.Thumbnail = thumbnailAsString;

            await _locationSnapshotDataService.UpdateSnapshotAsync(snapshot);

            return data.Length;
        }

        public Task<bool> SnapshotContentExists(LocationSnapshot snapshot)
        {
            var picturePath = Path.Combine(FileSystem.Current.AppDataDirectory, snapshot.PictureFileName);

            return Task.FromResult(File.Exists(picturePath));
        }
    }
}
