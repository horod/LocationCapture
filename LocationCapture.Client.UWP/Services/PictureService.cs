using LocationCapture.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocationCapture.Models;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class PictureService : IPictureService
    {
        private readonly IBitmapConverter _bitmapConverter;
        private readonly IMiniaturesCache _miniaturesCache;

        public PictureService(IBitmapConverter bitmapConverter,
            IMiniaturesCache miniaturesCache)
        {
            _bitmapConverter = bitmapConverter;
            _miniaturesCache = miniaturesCache;
        }

        public async Task<IEnumerable<SnapshotMiniature>> GetSnapshotMiniaturesAsync(IEnumerable<LocationSnapshot> snapshots)
        {
            var results = new List<SnapshotMiniature>();
            if (!snapshots.Any()) return results;

            var miniatures = _miniaturesCache.GetSnapshotMiniatures(snapshots);
            if (miniatures.Count() == snapshots.Count()) return miniatures;

            var pictureFiles = await GetPictureFiles();
            foreach (var snapshot in snapshots)
            {
                var pictureFile = pictureFiles.FirstOrDefault(_ => _.Name == snapshot.PictureFileName);
                var miniature = (pictureFile != null) ? await GetSnapshotMiniatureAsync(snapshot, pictureFile) : new SnapshotMiniature { Snapshot = snapshot };
                results.Add(miniature);
                if (miniature.Data == null) continue;
                _miniaturesCache.AddSnapshotMiniature(miniature);
            }

            return results;
        }

        private async Task<IReadOnlyList<StorageFile>> GetPictureFiles()
        {
            var picturesFolder = KnownFolders.CameraRoll;
            var queryOptions = new QueryOptions { FolderDepth = FolderDepth.Deep, IndexerOption = IndexerOption.OnlyUseIndexer };
            var queryResults = picturesFolder.CreateFileQueryWithOptions(queryOptions);
            var pictures = await queryResults.GetFilesAsync();

            return pictures;
        }

        private async Task<SnapshotMiniature> GetSnapshotMiniatureAsync(LocationSnapshot snapshot, StorageFile picture)
        {
            using (var thumbnail = await picture.GetThumbnailAsync(ThumbnailMode.ListView, 100, ThumbnailOptions.UseCurrentScale))
            {
                var bytes = await _bitmapConverter.GetBytesFromStream(thumbnail);
                return new SnapshotMiniature { Snapshot = snapshot, Data = bytes };
            }
        }

        public async Task<byte[]> GetSnapshotContentAsync(LocationSnapshot snapshot)
        {
            var picturesFolder = KnownFolders.CameraRoll;
            try
            {
                var pictureFile = await picturesFolder.GetFileAsync(snapshot.PictureFileName);
                var picture = await GetPictureAsync(pictureFile);
                return picture;
            }
            catch
            {
                // Picture does not exist in the specified location
                return new byte[0];
            }
        }

        private async Task<byte[]> GetPictureAsync(StorageFile pictureFile)
        {
            using (var picStream = await pictureFile.OpenReadAsync())
            {
                var pictureBytes = await _bitmapConverter.GetBytesFromStream(picStream);
                return pictureBytes;
            }
        }

        public async Task<int> SaveSnapshotContentAsync(LocationSnapshot snapshot, byte[] data)
        {
            var cameraRollFolder = KnownFolders.CameraRoll;

            var pictureFile = await cameraRollFolder.CreateFileAsync(snapshot.PictureFileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(pictureFile, data);

            var miniature = await GetSnapshotMiniatureAsync(snapshot, pictureFile);
            _miniaturesCache.AddSnapshotMiniature(miniature);

            return data.Length;
        }

        public async Task RemoveSnapshotContentAsync(LocationSnapshot snapshot)
        {
            _miniaturesCache.RemoveSnapshotMiniature(snapshot.Id);

            try
            {
                var picturesFolder = KnownFolders.CameraRoll;
                var pictureFile = await picturesFolder.GetFileAsync(snapshot.PictureFileName);
                await pictureFile.DeleteAsync();
            }
            catch
            {
                // Picture does not exist in the specified location
            }
        }
    }
}
