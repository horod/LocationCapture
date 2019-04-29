using LocationCapture.Models;
using System.Collections.Generic;

namespace LocationCapture.BL
{
    public interface IMiniaturesCache
    {
        IEnumerable<SnapshotMiniature> GetSnapshotMiniatures(IEnumerable<LocationSnapshot> snapshots);

        void AddSnapshotMiniature(SnapshotMiniature miniature);

        void RemoveSnapshotMiniature(int snapshotId);

        void Clear();
    }
}
