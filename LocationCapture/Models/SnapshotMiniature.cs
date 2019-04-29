namespace LocationCapture.Models
{
    public class SnapshotMiniature : NotificationBase
    {
        private LocationSnapshot _Snapshot;
        public LocationSnapshot Snapshot
        {
            get { return _Snapshot; }
            set { SetProperty(ref _Snapshot, value); }
        }

        private byte[] _Data;
        public byte[] Data
        {
            get { return _Data; }
            set { SetProperty(ref _Data, value); }
        }
    }
}
