using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.ViewModels;
using Microsoft.Practices.Unity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LocationCapture.Client.UWP.Views
{
    public sealed partial class CameraView : ViewBase
    {
        [Dependency]
        public CameraViewModel ViewModel
        {
            set { this.DataContext = value; }
            get { return this.DataContext as CameraViewModel; }
        }

        public override INavigationTarget NavigationTarget => ViewModel;

        public CameraView()
        {
            this.InitializeComponent();

            Application.Current.Suspending += OnApplicationSuspending;
        }

        private async void OnApplicationSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            if (Frame.CurrentSourcePageType == typeof(CameraView))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await ViewModel.DisposeAsync();
                deferral.Complete();
                Frame.Navigate(typeof(SnapshotsView), ViewModel.NavigationParam);
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await ViewModel.DisposeAsync();
            base.OnNavigatedFrom(e);
        }
    }
}
