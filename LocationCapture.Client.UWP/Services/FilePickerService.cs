using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Services;
using System;
using System.Threading.Tasks;

namespace LocationCapture.Client.UWP.Services
{
    public class FilePickerService : IFilePickerService
    {
        public Task<string> PickAsync(string title, FileType fileType)
        {
            throw new NotImplementedException();
        }
    }
}
