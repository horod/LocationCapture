using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.ViewModels;
using Microsoft.Practices.Unity;

namespace LocationCapture.Client.UWP.Views
{
    public sealed partial class SnapshotsView : ViewBase
    {
        [Dependency]
        public SnapshotsViewModel ViewModel
        {
            set { this.DataContext = value; }
            get { return this.DataContext as SnapshotsViewModel; }
        }

        public override INavigationTarget NavigationTarget => ViewModel;

        public SnapshotsView()
        {
            this.InitializeComponent();
        }
    }
}
