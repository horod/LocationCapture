using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.MVVM.ViewModels;
using LocationCapture.Models;
using Location = LocationCapture.Models.Location;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class LocationsView : ViewBase
{
    private readonly INavigationService _navigationService;

    public LocationsView(LocationsViewModel viewModel, INavigationService navigationService)
	{
        _navigationService = navigationService;

        BindingContext = viewModel;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        InitializeComponent();
    }

    private LocationsViewModel ViewModel => BindingContext as LocationsViewModel;

    private void OnLocationTapped(object sender, EventArgs e)
    {
        var location = (sender as Grid).BindingContext as Location;

        if (ViewModel.IsInSelectMode)
        {
            if (locations.SelectedItems.Contains(location))
            {
                locations.SelectedItems.Remove(location);
            }
            else
            {
                locations.SelectedItems.Add(location);
            }

            ViewModel.OnLocationSelected(locations.SelectedItems, null);
        }
        else if (ViewModel.IsInBrowseMode)
        {
            ViewModel.OnLocationClicked(null, location);
        }
    }

    private void OnSnapshotGroupTapped(object sender, EventArgs e)
    {
        var group = (sender as Grid).BindingContext as SnapshotGroup;

        ViewModel.OnSnapshotGroupClicked(null, group);
    }

    private void OnSwitchedToSelectMode(object sender, EventArgs e)
    {
        ViewModel.BeginSelectLocation();
    }

    private void OnBeginAddLocation(object sender, EventArgs e)
    {
        ViewModel.BeginAddLocation();
    }

    private void OnBeginRenameLocation(object sender, EventArgs e)
    {
        ViewModel.BeginRenameLocation();
    }

    private async void OnRemoveSelectedLocations(object sender, EventArgs e)
    {
        await ViewModel.RemoveSelectedLocations();
    }

    private async void OnSaveChanges(object sender, EventArgs e)
    {
        await ViewModel.SaveChanges();
    }

    private void OnRevertChanges(object sender, EventArgs e)
    {
        ViewModel.RevertChanges();
    }

    private void OnGoToAppSettings(object sender, EventArgs e)
    {
        ViewModel.GoToAppSettings();
    }

    private void OnGoToLogs(object sender, EventArgs e)
    {
        _navigationService.GoTo(AppViews.Logs, ViewModel.GroupBy);
    }

    private async void OnImportSelectedLocations(object sender, EventArgs e)
    {
        await ViewModel.ImportSelectedLocations();
    }

    private void OnStopLocationsImport(object sender, EventArgs e)
    {
        ViewModel.StopLocationsImport();
    }

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectionMode) && ViewModel.SelectionMode == MVVM.Enums.SelectionMode.None)
        {
            locations.SelectedItems.Clear();
        }
    }
}