using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Infrastructure
{
    public interface INavigationTarget
    {
        object NavigationParam { get; set; }
        void GoBack();
        Task OnNavigatedTo();
        Task SaveState();
    }
}
