using LocationCapture.Models;

namespace LocationCapture.Client.MVVM.Models
{
    public class SnapshotExportImportDescriptor
    {
        public LocationSnapshot Snapshot { get; set; }

        public string SnapshotPath { get; set; }

        public double SnapshotSize { get; set; }

        public object SnapshotThumbnail { get; set; }
    }
}
