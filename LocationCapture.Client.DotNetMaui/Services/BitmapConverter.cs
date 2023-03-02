using LocationCapture.Client.MVVM.Services;
using static Android.Graphics.ImageDecoder;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class BitmapConverter : IBitmapConverter
    {
        public Task<byte[]> EncodeBitmap(object bitmap)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetBitmapAsync(byte[] data)
        {
            var result = ImageSource.FromStream(() =>
            {
                var imageStream = new MemoryStream(data);

                return imageStream;
            });

            return Task.FromResult((object)result);
        }

        public Task<byte[]> GetBytesFromStream(object randomAccessStream)
        {
            throw new NotImplementedException();
        }
    }
}
