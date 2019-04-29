using LocationCapture.Models;

namespace LocationCapture.Client.MVVM.Models
{
    public class SnapshotThumbnail : NotificationBase
    {
        private LocationSnapshot _Snapshot;
        public LocationSnapshot Snapshot
        {
            get { return _Snapshot; }
            set { SetProperty(ref _Snapshot, value); }
        }

        private object _Thumbnail;
        public object Thumbnail
        {
            get { return _Thumbnail; }
            set { SetProperty(ref _Thumbnail, value); }
        }
    }
}
