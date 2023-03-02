using LocationCapture.Client.DotNetMaui.Views;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.ViewModels;
using Microsoft.Extensions.Logging;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class SnapshotsView : ViewBase
{
    private readonly ILogger<SnapshotsView> _logger;

    public SnapshotsView(SnapshotsViewModel viewModel, ILogger<SnapshotsView> logger)
	{
        BindingContext = viewModel;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        _logger = logger;

        InitializeComponent();
	}

    private SnapshotsViewModel ViewModel => BindingContext as SnapshotsViewModel;

    private void OnThumbnailTapped(object sender, EventArgs e)
    {
        var thumbnail = (sender as Grid).BindingContext as SnapshotThumbnail;

        if (ViewModel.IsInSelectMode)
        {
            if (snapshots.SelectedItems.Contains(thumbnail))
            {
                snapshots.SelectedItems.Remove(thumbnail);
            }
            else
            {
                snapshots.SelectedItems.Add(thumbnail);
            }

            ViewModel.OnSnapshotSelected(snapshots.SelectedItems, null);
        }
        else
        {
            ViewModel.OnSnapshotClicked(null, thumbnail);
        }
    }

    private void OnSnapshotAdding(object sender, EventArgs e)
    {
        ViewModel.OnSnapshotAdding();
    }

    private void OnSwitchedToSelectMode(object sender, EventArgs e)
    {
        ViewModel.BeginSelectSnapshot();
    }

    private async void OnRemoveSelectedSnapshot(object sender, EventArgs e)
    {
        await ViewModel.RemoveSelectedSnapshots();
    }

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectionMode) && ViewModel.SelectionMode == LocationCapture.Client.MVVM.Enums.SelectionMode.None)
        {
            snapshots.SelectedItems.Clear();
        }
    }
}