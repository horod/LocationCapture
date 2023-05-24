using LocationCapture.Client.MVVM.Models;
using LocationCapture.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface ISnapshotPackageManager
    {
        Task<string> CompressSnapshots(ICollection<LocationSnapshot> snapshots);
        void DecompressPackage(string packagePath);
        Task<ICollection<SnapshotExportImportDescriptor>> GetSnapshotsFromPackageFolder(string packageName);
        Task<byte[]> GetSnapshotContentFromPackageFolder(string packageName, string pictureFilePath);
        void RemovePackage(string packageName, bool removeZipFile);
    }
}
