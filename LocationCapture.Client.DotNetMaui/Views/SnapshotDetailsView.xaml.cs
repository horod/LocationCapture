using LocationCapture.Client.DotNetMaui.Views;
using LocationCapture.Client.MVVM.ViewModels;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class SnapshotDetailsView : ViewBase
{
    private readonly GeolocationViewModel _geolocationViewModel;

    public SnapshotDetailsView(SnapshotDetailsViewModel viewModel, GeolocationViewModel geolocationViewModel)
	{
        BindingContext = viewModel;
        _geolocationViewModel = geolocationViewModel;

        InitializeComponent();
	}

    private SnapshotDetailsViewModel ViewModel => BindingContext as SnapshotDetailsViewModel;

    private void OnImageTapped(object sender, EventArgs e)
    {
        ViewModel.ImageTapped();

        if (ViewModel.IsCommandBarVisible)
        {
            MainContainer.Background = new SolidColorBrush(Colors.White);
        }
        else
        {
            MainContainer.Background = new SolidColorBrush(Colors.Black);
        }
    }

    private async void OnShowLocation(object sender, EventArgs e)
    {
        _geolocationViewModel.NavigationParam = ViewModel.NavigationParam;
        await _geolocationViewModel.OnLoaded();
    }

    private void OnShowWeather(object sender, EventArgs e)
    {
        ViewModel.ShowWeather();
    }

    private void OnShowSuggestions(object sender, EventArgs e)
    {
        ViewModel.ShowSuggestions();
    }

    private async void OnImageSwiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Left)
        {
            await ViewModel.ImageSwipedLeft();
        }
        else if (e.Direction == SwipeDirection.Right)
        {
            await ViewModel.ImageSwipedRight();
        }
    }
}