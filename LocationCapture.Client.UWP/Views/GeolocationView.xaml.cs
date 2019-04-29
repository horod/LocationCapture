using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.ViewModels;
using Microsoft.Practices.Unity;

namespace LocationCapture.Client.UWP.Views
{
    public sealed partial class GeolocationView : ViewBase
    {
        [Dependency]
        public GeolocationViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as GeolocationViewModel; }
        }

        public override INavigationTarget NavigationTarget => ViewModel;

        public GeolocationView()
        {
            this.InitializeComponent();
        }
    }
}
