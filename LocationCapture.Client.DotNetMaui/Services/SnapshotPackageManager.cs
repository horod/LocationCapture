using LocationCapture.BL;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using System.IO.Compression;
using System.Text.Json;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class SnapshotPackageManager : ISnapshotPackageManager
    {
        private readonly IPictureService _pictureService;

        public SnapshotPackageManager(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        public async Task<string> CompressSnapshots(ICollection<LocationSnapshot> snapshots)
        {
            var now = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var packageFolderName = $"LocationCapture_SnapshotPackage_{now}";
            var packageFolderPath = Path.Combine(FileSystem.Current.CacheDirectory, packageFolderName);

            Directory.CreateDirectory(packageFolderPath);

            foreach (var snapshot in snapshots)
            {
                var metadata = JsonSerializer.Serialize(snapshot);
                var metadataFilePath = Path.Combine(packageFolderPath, snapshot.PictureFileName.Replace("jpg", "json"));

                await File.WriteAllTextAsync(metadataFilePath, metadata);

                var bytes = await _pictureService.GetSnapshotContentAsync(snapshot);
                await File.WriteAllBytesAsync(Path.Combine(packageFolderPath, snapshot.PictureFileName), bytes);
            }

            var zipPath = Path.Combine(FileSystem.Current.CacheDirectory, $"{packageFolderName}.zip");
            ZipFile.CreateFromDirectory(packageFolderPath, zipPath);

            return zipPath;
        }

        public void DecompressPackage(string packagePath)
        {
            if (!File.Exists(packagePath))
            {
                throw new IOException("Package does not exist");
            }

            var packageName = Path.GetFileNameWithoutExtension(packagePath);
            var packageExt = Path.GetExtension(packagePath);

            if (packageExt != ".zip")
            {
                throw new ArgumentException("The package has to be a ZIP archive.");
            }

            var packageFolderPath = Path.Combine(FileSystem.Current.CacheDirectory, packageName);

            ZipFile.ExtractToDirectory(packagePath, packageFolderPath, true);
        }

        public async Task<byte[]> GetSnapshotContentFromPackageFolder(string packageName, string pictureFilePath)
        {
            var packageFolderPath = Path.Combine(FileSystem.Current.CacheDirectory, packageName);

            if (!Directory.Exists(packageFolderPath))
            {
                throw new IOException("Package folder not found");
            }

            if (!File.Exists(pictureFilePath))
            {
                throw new IOException("Snapshot does not exist");
            }

            var pictureBytes = await File.ReadAllBytesAsync(pictureFilePath);

            return pictureBytes;
        }

        public async Task<ICollection<SnapshotExportImportDescriptor>> GetSnapshotsFromPackageFolder(string packageName)
        {
            var packageFolderPath = Path.Combine(FileSystem.Current.CacheDirectory, packageName);

            if (!Directory.Exists(packageFolderPath))
            {
                throw new IOException("Package folder not found");
            }

            var snapshots = Directory.GetFiles(packageFolderPath, "LocationCapture_*.json");

            if (snapshots.Length == 0)
            {
                throw new ArgumentException("The package does not contain any snapshots");
            }

            var results = new List<SnapshotExportImportDescriptor>();

            foreach (var snapshotMetaFile in snapshots)
            {
                var picturePath = snapshotMetaFile.Replace("json", "jpg");

                if (!File.Exists(picturePath))
                {
                    throw new ArgumentException("The package is corrupted");
                }

                var pictureSize = Math.Round(new FileInfo(picturePath).Length / 1024d / 1024d, 2);

                var snapshotMetadata = await File.ReadAllTextAsync(snapshotMetaFile);
                var snapshot = JsonSerializer.Deserialize<LocationSnapshot>(snapshotMetadata);

                var snapshotDescriptor = new SnapshotExportImportDescriptor
                {
                    Snapshot = snapshot,
                    SnapshotPath = picturePath,
                    SnapshotSize = pictureSize
                };

                results.Add(snapshotDescriptor);
            }

            return results;
        }

        public void RemovePackage(string packageName, bool removeZipFile)
        {
            var packageFolderPath = Path.Combine(FileSystem.Current.CacheDirectory, packageName);

            if (!Directory.Exists(packageFolderPath))
            {
                throw new IOException("Package folder not found");
            }

            Directory.Delete(packageFolderPath, true);

            if (!removeZipFile) return;

            var zipPath = Path.Combine(FileSystem.Current.CacheDirectory, $"{packageName}.zip");

            if (!File.Exists(zipPath))
            {
                throw new IOException("Package ZIP archive not found");
            }

            File.Delete(zipPath);
        }
    }
}
