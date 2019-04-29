using LocationCapture.Enums;
using LocationCapture.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public interface ILocationSnapshotDataService
    {
        Task<IEnumerable<LocationSnapshot>> GetSnapshotsByLocationIdAsync(int locationId);

        Task<IEnumerable<LocationSnapshot>> GetSnapshotsByIdsAsync(IEnumerable<int> snapshotIds);

        Task<LocationSnapshot> AddSnapshotAsync(LocationSnapshot snapshotToAdd);

        Task<LocationSnapshot> UpdateSnapshotAsync(LocationSnapshot snapshot);

        Task<LocationSnapshot> RemoveSnapshotAsync(int snapshotId);

        Func<Task<IEnumerable<SnapshotGroup>>> ChooseGroupByOperation(GroupByCriteria groupBy);
    }
}
