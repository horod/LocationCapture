using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class FilePickerService : IFilePickerService
    {
        public async Task<string> PickAsync(string title, FileType fileType)
        {
            var zipFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/zip", "application/x-zip-compressed", "multipart/x-zip" } }, // MIME type
                });

            var filePickerType = fileType switch
            {
                FileType.Images => FilePickerFileType.Images,
                FileType.Jpeg => FilePickerFileType.Jpeg,
                FileType.Pdf => FilePickerFileType.Pdf,
                FileType.Png => FilePickerFileType.Png,
                FileType.Videos => FilePickerFileType.Videos,
                FileType.Zip => zipFileType,
                _ => throw new ArgumentOutOfRangeException(fileType.ToString())
            };

            PickOptions options = new()
            {
                PickerTitle = title,
                FileTypes = filePickerType,
            };

            var result = await FilePicker.Default.PickAsync(options);

            return (result != null) ? result.FullPath : string.Empty;
        }
    }
}
