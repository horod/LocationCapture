using Microsoft.Practices.Unity;
using Windows.UI.Xaml.Controls;

namespace LocationCapture.Client.UWP.Infrastructure
{
    public class DiFrame : Frame
    {
        private IUnityContainer _container;

        public DiFrame(IUnityContainer container)
        {
            _container = container;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            _container.BuildUp(newContent.GetType(), newContent);
        }
    }
}
