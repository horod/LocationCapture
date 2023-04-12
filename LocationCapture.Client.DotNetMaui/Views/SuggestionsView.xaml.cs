using LocationCapture.Client.MVVM.ViewModels;
using LocationCapture.Models;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class SuggestionsView : ViewBase
{
	public SuggestionsView(SuggestionsViewModel viewModel)
	{
		BindingContext = viewModel;

		InitializeComponent();
	}

    private SuggestionsViewModel ViewModel => BindingContext as SuggestionsViewModel;

    private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (ViewModel?.NavigationParam != null)
		{
            await ViewModel?.OnSuggestionTypeChanged();
			results.RowHeight = ViewModel?.SelectedSuggestionType == Enums.LocationSuggestionType.Description ? 500 : 100;
        }
    }

	private async void OnSuggestionTapped(object sender, EventArgs e)
	{
        var suggestion = (sender as StackLayout).BindingContext as LocationSuggestion;

		if (suggestion == null || !suggestion.IsGeolocatable)
		{
			return;
		}

		await ViewModel?.OnNavigatedToVenue(suggestion);
    }
}