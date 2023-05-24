using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.ViewModels;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class SnapshotExportImportView : ViewBase
{
	public SnapshotExportImportView(SnapshotExportImportViewModel viewModel)
	{
        BindingContext = viewModel;

        InitializeComponent();
	}

    private SnapshotExportImportViewModel ViewModel => BindingContext as SnapshotExportImportViewModel;

    private void OnDescriptorTapped(object sender, EventArgs e)
    {
        var descriptor = (sender as Grid).BindingContext as SnapshotExportImportDescriptor;

        if (descriptors.SelectedItems.Contains(descriptor))
        {
            descriptors.SelectedItems.Remove(descriptor);
        }
        else
        {
            descriptors.SelectedItems.Add(descriptor);
        }
    }

    private async void OnSaveAll(object sender, EventArgs e)
    {
        await ViewModel.OnSaveAllSnapshots();
    }

    private async void OnSaveSelected(object sender, EventArgs e)
    {
        var selectedItems = descriptors.SelectedItems.Cast<SnapshotExportImportDescriptor>().ToList();

        await ViewModel.OnSaveSelectedSnapshots(selectedItems);
    }

    private async void OnShareAll(object sender, EventArgs e)
    {
        await ViewModel.OnShareAllSnapshots();
    }

    private async void OnShareSelected(object sender, EventArgs e)
    {
        var selectedItems = descriptors.SelectedItems.Cast<SnapshotExportImportDescriptor>().ToList();

        await ViewModel.OnShareSelectedSnapshots(selectedItems);
    }
}