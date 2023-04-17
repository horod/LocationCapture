using LocationCapture.Enums;

namespace LocationCapture.Client.MVVM.Models
{
    public class SuggestionsViewNavParams
    {
        public LocationSuggestionType SelectedSuggestionType { get; set; }
        public string LocationName { get; set; }
        public SnapshotDetailsViewNavParams SnapshotDetailsViewState { get; set; }
    }
}
