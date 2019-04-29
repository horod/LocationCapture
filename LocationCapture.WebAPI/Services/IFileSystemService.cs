using System.Threading.Tasks;

namespace LocationCapture.WebAPI.Services
{
    public interface IFileSystemService
    {
        bool FileExisits(string path);
        Task WriteAllBytesAsync(string path, byte[] bytes);
        Task<byte[]> ReadAllBytesAsync(string path);
        void DeleteFile(string path);
    }
}
