using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class FileShareService : IFileShareService
    {
        public async Task ShareAsync(string title, ICollection<string> filePaths)
        {
            if (filePaths.Count == 1)
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = title,
                    File = new ShareFile(filePaths.Single())
                });
            }
            else if (filePaths.Count > 1)
            {
                var filesToShare = filePaths.Select(x => new ShareFile(x))
                    .ToList();

                await Share.Default.RequestAsync(new ShareMultipleFilesRequest
                {
                    Title = title,
                    Files = filesToShare
                });
            }
        }
    }
}
