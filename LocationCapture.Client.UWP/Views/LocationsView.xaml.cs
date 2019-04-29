using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.ViewModels;
using Microsoft.Practices.Unity;

namespace LocationCapture.Client.UWP.Views
{
    public sealed partial class LocationsView : ViewBase
    {
        [Dependency]
        public LocationsViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as LocationsViewModel; }
        }

        public override INavigationTarget NavigationTarget => ViewModel;

        public LocationsView()
        {
            this.InitializeComponent();
        }
    }
}
