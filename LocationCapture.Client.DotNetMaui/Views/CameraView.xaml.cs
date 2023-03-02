using LocationCapture.Client.DotNetMaui.Views;
using LocationCapture.Client.MVVM.ViewModels;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class CameraView : ViewBase
{
	public CameraView(CameraViewModel viewModel)
	{
        BindingContext = viewModel;

        InitializeComponent();
	}
}