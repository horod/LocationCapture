using LocationCapture.Enums;
using LocationCapture.Models;

namespace LocationCapture.Client.MVVM.Models
{
    public class SnapshotsViewNavParams
    {
        public GroupByCriteria GroupByCriteria { get; set; }
        public Location SelectedLocation { get; set; }
        public SnapshotGroup SelectedGroup { get; set; }
    }
}
