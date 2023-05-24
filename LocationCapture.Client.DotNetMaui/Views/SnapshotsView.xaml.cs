using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.ViewModels;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class SnapshotsView : ViewBase
{
    public SnapshotsView(SnapshotsViewModel viewModel)
	{
        BindingContext = viewModel;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

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

    private async void OnImportSnapshots(object sender, EventArgs e)
    {
        await ViewModel.ImportSnapshots();
    }

    private void OnSwitchedToSelectMode(object sender, EventArgs e)
    {
        ViewModel.BeginSelectSnapshot();
    }

    private async void OnRemoveSelectedSnapshot(object sender, EventArgs e)
    {
        await ViewModel.RemoveSelectedSnapshots();
    }

    private async void OnExportSelectedSnapshot(object sender, EventArgs e)
    {
        await ViewModel.ExportSelectedSnapshots();
    }

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectionMode) && ViewModel.SelectionMode == MVVM.Enums.SelectionMode.None)
        {
            snapshots.SelectedItems.Clear();
        }
    }
}