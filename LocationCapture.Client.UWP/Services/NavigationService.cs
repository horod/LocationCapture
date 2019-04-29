using System;
using LocationCapture.Client.MVVM.Enums;
using Windows.UI.Xaml.Controls;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _rootFrame;

        private string ViewTypeNameTemplate => "LocationCapture.Client.UWP.Views.{0}View";

        public NavigationService(Frame rootFrame)
        {
            _rootFrame = rootFrame;
        }

        public void GoTo(AppViews navTarget, object navParam = null)
        {
            var viewTypeName = string.Format(ViewTypeNameTemplate, navTarget);
            var viewType = Type.GetType(viewTypeName);
            _rootFrame.Navigate(viewType, navParam);
        }
    }
}
