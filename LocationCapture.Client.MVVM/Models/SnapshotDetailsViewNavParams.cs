using LocationCapture.Models;

namespace LocationCapture.Client.MVVM.Models
{
    public class SnapshotDetailsViewNavParams
    {
        public LocationSnapshot LocationSnapshot { get; set; }
        public object SnapshotsViewState { get; set; }
    }
}
