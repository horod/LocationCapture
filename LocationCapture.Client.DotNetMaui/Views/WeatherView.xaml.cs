using LocationCapture.Client.DotNetMaui.Views;
using LocationCapture.Client.MVVM.ViewModels;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class WeatherView : ViewBase
{
	public WeatherView(WeatherViewModel viewModel)
	{
		BindingContext = viewModel;

		InitializeComponent();
	}
}