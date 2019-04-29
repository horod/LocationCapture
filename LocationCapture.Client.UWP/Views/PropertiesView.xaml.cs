using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.ViewModels;
using Microsoft.Practices.Unity;

namespace LocationCapture.Client.UWP.Views
{
    public sealed partial class PropertiesView : ViewBase
    {
        [Dependency]
        public PropertiesViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as PropertiesViewModel; }
        }

        public override INavigationTarget NavigationTarget => ViewModel;

        public PropertiesView()
        {
            this.InitializeComponent();
        }
    }
}
