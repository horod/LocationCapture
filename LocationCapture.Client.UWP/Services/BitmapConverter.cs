using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class BitmapConverter : IBitmapConverter
    {
        public async Task<object> GetBitmapAsync(byte[] data)
        {
            var bitmapImage = new BitmapImage();
            if (data == null || data.Length == 0) return bitmapImage;

            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(stream))
                {
                    writer.WriteBytes(data);
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    writer.DetachStream();
                }

                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);
            }

            return bitmapImage;
        }

        public async Task<byte[]> EncodeBitmap(object bitmap)
        {
            byte[] array = null;
            var softwareBitmap = bitmap as SoftwareBitmap;
            if (softwareBitmap == null) return array = new byte[0];

            using (var ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms);
                encoder.SetSoftwareBitmap(softwareBitmap);

                try
                {
                    await encoder.FlushAsync();
                }
                catch
                {
                    return new byte[0];
                }

                array = new byte[ms.Size];
                await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
            }

            return array;
        }

        public async Task<byte[]> GetBytesFromStream(object randomAccessStream)
        {
            var uwpStream = randomAccessStream as IRandomAccessStream;
            if (uwpStream == null) return new byte[0];

            Stream stream = uwpStream.AsStream();
            byte[] bytes = new byte[Convert.ToUInt32(uwpStream.Size)];
            stream.Position = 0;
            await stream.ReadAsync(bytes, 0, bytes.Length);

            return bytes;
        }
    }
}
