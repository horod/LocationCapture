using LocationCapture.Client.MVVM.Enums;

namespace LocationCapture.Client.MVVM.Models
{
    public class SnapshotExportImportViewNavParams
    {
        public string PackagePath { get; set; }

        public SnapshotExportImportMode Mode { get; set; }

        public SnapshotsViewNavParams SnapshotsViewState { get; set; }
    }
}
