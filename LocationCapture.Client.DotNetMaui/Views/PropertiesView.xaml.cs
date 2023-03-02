using LocationCapture.Client.DotNetMaui.Views;
using LocationCapture.Client.MVVM.ViewModels;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class PropertiesView : ViewBase
{
	public PropertiesView(PropertiesViewModel viewModel)
	{
        BindingContext = viewModel;

        InitializeComponent();
	}

    private PropertiesViewModel ViewModel => BindingContext as PropertiesViewModel;

    private void OnSaveChanges(object sender, EventArgs e)
    {
        ViewModel.SaveChanges();
    }

    private void OnRevertChanges(object sender, EventArgs e)
    {
        ViewModel.RevertChanges();
    }

    private void OnDataSourceChanged(object sender, EventArgs e)
    {
        ViewModel.DataSourceChanged();
    }
}