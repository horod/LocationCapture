using System.Threading.Tasks;

namespace LocationCapture.WebAPI.Services
{
    public class FileSystemService : IFileSystemService
    {
        public void DeleteFile(string path)
        {
            System.IO.File.Delete(path);
        }

        public bool FileExisits(string path)
        {
            return System.IO.File.Exists(path);
        }

        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            return await System.IO.File.ReadAllBytesAsync(path);
        }

        public async Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            await System.IO.File.WriteAllBytesAsync(path, bytes);
        }
    }
}
