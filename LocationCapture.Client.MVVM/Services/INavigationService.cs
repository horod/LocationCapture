using LocationCapture.Client.MVVM.Enums;

namespace LocationCapture.Client.MVVM.Services
{
    public interface INavigationService
    {
        void GoTo(AppViews navTarget, object navParam = null);
    }
}
