using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.Client.UWP.Services
{
    public class SnapshotPackageManager : ISnapshotPackageManager
    {
        public Task<string> CompressSnapshots(ICollection<LocationSnapshot> snapshots)
        {
            throw new NotImplementedException();
        }

        public void DecompressPackage(string packagePath)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetSnapshotContentFromPackageFolder(string packageName, string pictureFilePath)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<SnapshotExportImportDescriptor>> GetSnapshotsFromPackageFolder(string packageName)
        {
            throw new NotImplementedException();
        }

        public void RemovePackage(string packageName, bool removeZipFile)
        {
            throw new NotImplementedException();
        }
    }
}
