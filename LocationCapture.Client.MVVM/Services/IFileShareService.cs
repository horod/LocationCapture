using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface IFileShareService
    {
        Task ShareAsync(string title, ICollection<string> filePaths);
    }
}
