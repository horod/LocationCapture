using System.Collections.Generic;

namespace LocationCapture.Client.MVVM.Services
{
    public interface IPlatformSpecificActions
    {
        List<TEntity> GetSelectedItems<TEntity>(object selectionInfo);
        TEntity GetClickedItem<TEntity>(object clickInfo);
    }
}
