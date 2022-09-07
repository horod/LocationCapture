using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LocationCapture.Models;

namespace LocationCapture.BL
{
    public class MiniaturesCache : IMiniaturesCache
    {
        private ConcurrentDictionary<int, SnapshotMiniature> _snapshotMiniaturesCache = new ConcurrentDictionary<int, SnapshotMiniature>();

        private int MaxCacheSize => 104857600;

        public IEnumerable<SnapshotMiniature> GetSnapshotMiniatures(IEnumerable<LocationSnapshot> snapshots)
        {
            if (snapshots.All(_ => _snapshotMiniaturesCache.ContainsKey(_.Id)))
            {
                return _snapshotMiniaturesCache.Where(_ => snapshots.Select(__ => __.Id).Contains(_.Key))
                    .Select(_ => _.Value);
            }
            else
            {
                return new List<SnapshotMiniature>();
            }
        }

        public SnapshotMiniature GetSnapshotMiniature(LocationSnapshot snapshot)
        {
            var result = new SnapshotMiniature { Snapshot = snapshot, Data = new byte[0] };

            _snapshotMiniaturesCache.TryGetValue(snapshot.Id, out result);

            return result;
        }

        public void AddSnapshotMiniature(SnapshotMiniature miniature)
        {
            PreventCacheOverflow(miniature.Data.Length);

            if (!_snapshotMiniaturesCache.ContainsKey(miniature.Snapshot.Id)) _snapshotMiniaturesCache.TryAdd(miniature.Snapshot.Id, miniature);
        }

        private void PreventCacheOverflow(int bytesToAddCount)
        {
            var expectedCacheSize = _snapshotMiniaturesCache.Sum(_ => _.Value.Data.Length) + bytesToAddCount;

            if (expectedCacheSize > MaxCacheSize)
            {
                _snapshotMiniaturesCache.Clear();
            }
        }

        public void RemoveSnapshotMiniature(int snapshotId)
        {
            SnapshotMiniature removedMiniature;
            if (_snapshotMiniaturesCache.ContainsKey(snapshotId)) _snapshotMiniaturesCache.TryRemove(snapshotId, out removedMiniature);
        }

        public void Clear()
        {
            _snapshotMiniaturesCache.Clear();
        }
    }
}
