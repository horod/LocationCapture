using LocationCapture.Models;
using System.Collections.Generic;

namespace LocationCapture.Client.MVVM.Models
{
    public class SnapshotDetailsViewNavParams
    {
        public LocationSnapshot LocationSnapshot { get; set; }
        public SnapshotsViewNavParams SnapshotsViewState { get; set; }
        public ICollection<SnapshoNavigationMapEntry> SnapshoNavigationMap { get; set; }
    }

    public class SnapshoNavigationMapEntry
    {
        public int SnapshotId { get; set; }

        public int PreviousSnapshotId { get; set; }

        public int NextSnapshotId { get; set; }
    }

}
