using LocationCapture.Enums;
using LocationCapture.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public interface ISuggestionsService
    {
        Task<ICollection<LocationSuggestion>> GetLocationSuggestions(
            string locationHint,
            LocationSuggestionType suggestionType);

        Task<ICollection<LocationSuggestion>> GetLocationDescription(
            LocationSnapshot locationSnapshot);

        Task<LocationDescriptor> GetVenueGpsCoordinates(
            string venue,
            string locationHint,
            LocationSuggestionType suggestionType);
    }
}
