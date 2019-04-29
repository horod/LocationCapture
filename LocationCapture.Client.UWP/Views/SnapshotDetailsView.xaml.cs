using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.ViewModels;
using Microsoft.Practices.Unity;
using Windows.UI.Xaml;

namespace LocationCapture.Client.UWP.Views
{
    public sealed partial class SnapshotDetailsView : ViewBase
    {
        [Dependency]
        public SnapshotDetailsViewModel ViewModel
        {
            set { this.DataContext = value; }
            get { return this.DataContext as SnapshotDetailsViewModel; }
        }

        public override INavigationTarget NavigationTarget => ViewModel;

        public SnapshotDetailsView()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += OnWindowSizeChanged;
        }

        private void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SnapshotContentPresenter.Width = e.Size.Width;
        }
    }
}
