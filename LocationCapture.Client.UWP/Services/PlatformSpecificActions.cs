using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class PlatformSpecificActions : IPlatformSpecificActions
    {
        public TEntity GetClickedItem<TEntity>(object clickInfo)
        {
            ItemClickEventArgs e = (ItemClickEventArgs)clickInfo;
            return (TEntity)e.ClickedItem;
        }

        public List<TEntity> GetSelectedItems<TEntity>(object selectionInfo)
        {
            ListViewBase selector = (ListViewBase)selectionInfo;
            return selector.SelectedItems.Cast<TEntity>().ToList();
        }
    }
}
