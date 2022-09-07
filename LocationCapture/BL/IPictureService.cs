using LocationCapture.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public interface IPictureService
    {
        Task<IEnumerable<SnapshotMiniature>> GetSnapshotMiniaturesAsync(IEnumerable<LocationSnapshot> snapshots);

        Task<SnapshotMiniature> GetSnapshotMiniatureAsync(LocationSnapshot snapshot);

        Task<byte[]> GetSnapshotContentAsync(LocationSnapshot snapshot);

        Task<int> SaveSnapshotContentAsync(LocationSnapshot snapshot, byte[] data);

        Task RemoveSnapshotContentAsync(LocationSnapshot snapshot);
    }
}
