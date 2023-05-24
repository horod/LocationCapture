using LocationCapture.Client.MVVM.Enums;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface IFilePickerService
    {
        // returns full picture file path
        Task<string> PickAsync(string title, FileType fileType);
    }
}
