using LocationCapture.Models;

namespace LocationCapture.Client.MVVM.Models
{
    public class SnapshotDetailsViewNavParams
    {
        public LocationSnapshot LocationSnapshot { get; set; }
        public SnapshotsViewNavParams SnapshotsViewState { get; set; }
    }
}
