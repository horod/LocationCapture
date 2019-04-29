using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class PropertiesViewModel : NotificationBase, INavigationTarget
    {
        private readonly INavigationService _navigationService;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IDataSourceGovernor _dataSourceGovernor;
        private readonly IConnectivityService _connectivityService;
        private readonly IDialogService _dialogService;

        public object NavigationParam { get; set; }

        private AppSettings _AppSettings;
        public AppSettings AppSettings
        {
            get { return _AppSettings; }
            set { SetProperty(ref _AppSettings, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        public PropertiesViewModel(INavigationService navigationService,
            IAppSettingsProvider appSettingsProvider,
            IDataSourceGovernor dataSourceGovernor,
            IConnectivityService connectivityService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _appSettingsProvider = appSettingsProvider;
            _dataSourceGovernor = dataSourceGovernor;
            _connectivityService = connectivityService;
            _dialogService = dialogService;
        }

        public async void OnLoaded()
        {
            IsBusy = true;
            AppSettings = await _appSettingsProvider.GetAppSettingsAsync();
            IsBusy = false;
        }

        public async void SaveChanges()
        {
            IsBusy = true;
            await _appSettingsProvider.SaveAppSettingsAsync(AppSettings);
            await _dataSourceGovernor.ChooseDataSourceAsync();
            IsBusy = false;

            _navigationService.GoTo(AppViews.Locations, NavigationParam);
        }

        public void RevertChanges()
        {
            _navigationService.GoTo(AppViews.Locations, NavigationParam);
        }

        public void GoBack()
        {
            _navigationService.GoTo(AppViews.Locations, NavigationParam);
        }

        public async void DataSourceChanged()
        {
            if (!AppSettings.UseWebApi) return;

            IsBusy = true;
            if (!(await _connectivityService.IsWebApiAvailableAsync()))
            {
                await _dialogService.ShowAsync("The LocationCapture web API is not available.");
                AppSettings.UseWebApi = false;
                RaisePropertyChanged(nameof(AppSettings));
            }
            IsBusy = false;
        }
    }
}