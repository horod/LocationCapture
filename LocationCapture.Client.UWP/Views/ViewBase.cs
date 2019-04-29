using LocationCapture.Client.MVVM.Infrastructure;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LocationCapture.Client.UWP.Views
{
    public abstract class ViewBase : Page
    {
        public abstract INavigationTarget NavigationTarget { get; }

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            NavigationTarget.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            NavigationTarget.NavigationParam = e.Parameter;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= BackRequested;
        }
    }
}
