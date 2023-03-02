using LocationCapture.Client.MVVM.Services;
using System.Collections;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class PlatformSpecificActions : IPlatformSpecificActions
    {
        public TEntity GetClickedItem<TEntity>(object clickInfo)
        {
            return (TEntity)clickInfo;
        }

        public List<TEntity> GetSelectedItems<TEntity>(object selectionInfo)
        {
            return ((IEnumerable)selectionInfo).Cast<TEntity>().ToList();
        }
    }
}
