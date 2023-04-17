using LocationCapture.Enums;
using LocationCapture.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public class SuggestionsService : ISuggestionsService
    {
        private readonly string ModelName = "text-davinci-003";

        private readonly IWebClient _webClient;
        private readonly ILocationDataService _locationDataService;
        private readonly IAppSettingsProvider _appSettingsProvider;

        public SuggestionsService(
            IWebClient webClient, 
            ILocationDataService locationDataService,
            IAppSettingsProvider appSettingsProvider)
        {
            _webClient = webClient;
            _locationDataService = locationDataService;
            _appSettingsProvider = appSettingsProvider;
        }

        public async Task<ICollection<LocationSuggestion>> GetLocationDescription(LocationSnapshot locationSnapshot)
        {
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();

            var payload = new
            {
                model = ModelName,
                prompt = $"Here's a set of GPS coordinates: {locationSnapshot.Latitude}, {locationSnapshot.Longitude}. What's the name of this place? Structure the answer along the lines of 'The name of this place is...'.",
                temperature = 0,
                max_tokens = 64
            };
            var response = await _webClient.PostAsync<object, dynamic>(appSettings.SuggestionsApiUri, payload, appSettings.SuggestionsApiKey);

            string possibleLocationName = response.choices[0].text;
            possibleLocationName = possibleLocationName?.Trim()?.Replace("The name of this place is ", "") ?? possibleLocationName;

            var givenLocationName = (await _locationDataService.GetAllLocationsAsync())
                .First(x => x.Id == locationSnapshot.LocationId)
                .Name;

            payload = new
            {
                model = ModelName,
                prompt = $"Is there a place called {givenLocationName} close to {possibleLocationName}? Simply answer 'Yes' or 'No'.",
                temperature = 0,
                max_tokens = 8
            };
            response = await _webClient.PostAsync<object, dynamic>(appSettings.SuggestionsApiUri, payload, appSettings.SuggestionsApiKey);

            bool? locationNamesMatch = ((string)response.choices[0].text)?.Trim()?.StartsWith("Yes");
            var fullLocationName = locationNamesMatch == true ? $"{givenLocationName}, close to {possibleLocationName}" : possibleLocationName;

            var descPayload = new
            {
                model = ModelName,
                prompt = $"Describe briefly the following place: {fullLocationName}.",
                temperature = 0,
                max_tokens = 128
            };
            response = await _webClient.PostAsync<object, dynamic>(appSettings.SuggestionsApiUri, descPayload, appSettings.SuggestionsApiKey);

            string locationDescription = response.choices[0].text;

            var results = new List<LocationSuggestion>
            {
                new LocationSuggestion { Content = fullLocationName?.Trim(), Type = LocationSuggestionType.Description, IsGeolocatable = true },
                new LocationSuggestion { Content = locationDescription?.Trim(), Type = LocationSuggestionType.Description, IsGeolocatable = false }
            };

            return results;
        }

        public async Task<ICollection<LocationSuggestion>> GetLocationSuggestions(string locationHint, LocationSuggestionType suggestionType)
        {
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();

            var suggestionPrompt = $"What are three <suggestion_details> in {locationHint}? Add a brief description to each position. Structure the response as an ordered list.";

            var suggestionDetails = suggestionType switch
            {
                LocationSuggestionType.Top3Hotels => "good hotels",
                LocationSuggestionType.Top3Restaurants => "good restaurants",
                LocationSuggestionType.Top3PlacesToVisit => "interesting places to visit",
                LocationSuggestionType.Top3ThingsToDo => "interesting things to do",
                _ => throw new ArgumentOutOfRangeException(suggestionType.ToString())
            };

            suggestionPrompt = suggestionPrompt.Replace("<suggestion_details>", suggestionDetails);

            var payload = new
            {
                model = ModelName,
                prompt = suggestionPrompt,
                temperature = 0,
                max_tokens = 1024
            };
            var response = await _webClient.PostAsync<object, dynamic>(appSettings.SuggestionsApiUri, payload, appSettings.SuggestionsApiKey);

            string rawResults = response.choices[0].text;

            var resultsCount = (rawResults.Contains("1.") ? 1 : 0) + (rawResults.Contains("2.") ? 1 : 0) + (rawResults.Contains("3.") ? 1 : 0);

            var results = new List<LocationSuggestion>();

            for (var i = 1; i < resultsCount + 1; i++)
            {
                string content;
                int start, len;

                if (i == resultsCount)
                {
                    start = rawResults.IndexOf($"{i}.") + 2;
                    len = rawResults.Length - start;
                }
                else
                {
                    start = rawResults.IndexOf($"{i}.") + 2;
                    len = rawResults.IndexOf($"{i + 1}.") - start;
                }

                content = rawResults.Substring(start, len).Trim();

                if (!string.IsNullOrEmpty(content))
                {
                    var venue = string.Empty;

                    if (content.Contains(":"))
                    {
                        venue = content.Substring(0, content.IndexOf(":")).Trim();
                    }
                    else if (content.Contains("-"))
                    {
                        venue = content.Substring(0, content.IndexOf("-")).Trim();
                    }

                    results.Add(new LocationSuggestion
                    {
                        Content = content,
                        Type = suggestionType,
                        IsGeolocatable = suggestionType != LocationSuggestionType.Top3ThingsToDo,
                        Venue = venue
                    });
                }
            }

            return results;
        }

        public async Task<LocationDescriptor> GetVenueGpsCoordinates(string venue, string locationHint, LocationSuggestionType suggestionType)
        {
            var notFoundResult = new LocationDescriptor { Latitude = double.MinValue, Longitude = double.MinValue };

            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();

            var suggestionPrompt = $"Give me the GPS coordinates of the <suggestion_details> called {venue} in {locationHint}. Format the coordinates as [latitude,longitude].";

            var suggestionDetails = suggestionType switch
            {
                LocationSuggestionType.Top3Hotels => "hotel",
                LocationSuggestionType.Top3Restaurants => "restaurant",
                LocationSuggestionType.Top3PlacesToVisit => "place",
                _ => throw new ArgumentOutOfRangeException(suggestionType.ToString())
            };

            suggestionPrompt = suggestionPrompt.Replace("<suggestion_details>", suggestionDetails);

            var payload = new
            {
                model = ModelName,
                prompt = suggestionPrompt,
                temperature = 0,
                max_tokens = 32
            };
            var response = await _webClient.PostAsync<object, dynamic>(appSettings.SuggestionsApiUri, payload, appSettings.SuggestionsApiKey);

            string rawResults = response.choices[0].text;

            var rawCoords = rawResults.Replace("[", "").Replace("]", "").Trim().Split(',');

            if (rawCoords.Length != 2) return notFoundResult;

            var hasLatitude = double.TryParse(rawCoords[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double latitude);
            var hasLongitude = double.TryParse(rawCoords[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double longitude);

            if (!hasLatitude || !hasLongitude)
            {
                return notFoundResult;
            }
            else
            {
                return new LocationDescriptor { Latitude = latitude, Longitude = longitude };
            }
        }
    }
}
