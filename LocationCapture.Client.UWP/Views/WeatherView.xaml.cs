using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.ViewModels;
using Microsoft.Practices.Unity;

namespace LocationCapture.Client.UWP.Views
{
    public sealed partial class WeatherView : ViewBase
    {
        [Dependency]
        public WeatherViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as WeatherViewModel; }
        }

        public override INavigationTarget NavigationTarget => ViewModel;

        public WeatherView()
        {
            this.InitializeComponent();
        }
    }
}
