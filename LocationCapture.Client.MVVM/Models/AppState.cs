using LocationCapture.Client.MVVM.Enums;

namespace LocationCapture.Client.MVVM.Models
{
    public class AppState
    {
        public AppViews CurrentView { get; set; }

        public object NavigationParam { get; set; }

        public AppState Clone()
        {
            return (AppState)MemberwiseClone();
        }
    }
}
