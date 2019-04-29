using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface IBitmapConverter
    {
        Task<object> GetBitmapAsync(byte[] data);
        Task<byte[]> EncodeBitmap(object bitmap);
        Task<byte[]> GetBytesFromStream(object randomAccessStream);
    }
}
