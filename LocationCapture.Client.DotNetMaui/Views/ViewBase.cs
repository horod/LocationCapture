using LocationCapture.Client.MVVM.Infrastructure;

namespace LocationCapture.Client.DotNetMaui.Views
{
    public class ViewBase : ContentPage, IQueryAttributable
    {
        public ViewBase()
        {
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(() =>
                {
                    var navTarget = BindingContext as INavigationTarget;
                    navTarget?.GoBack();
                }),
                IsVisible = false
            });
        }

        private INavigationTarget ViewModel => BindingContext as INavigationTarget;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var navParam = query[nameof(INavigationTarget.NavigationParam)];

            ViewModel.NavigationParam = navParam;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            await ViewModel.OnNavigatedTo();
        }
    }
}
