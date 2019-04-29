using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LocationCapture.WebAPI.Services
{
    public class ImageService : IImageService
    {
        private static int ExifOrientationID => 0x112; //274

        public async Task<byte[]> GetThumbnailAsync(string imagePath)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var image = Image.FromFile(imagePath))
                {
                    ExifRotate(image);

                    int width = 100;
                    int X = image.Width;
                    int Y = image.Height;
                    int height = (width * Y) / X;

                    using (var thumbnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            thumbnail.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
                            var thumbnailBytes = m.ToArray();
                            return thumbnailBytes;
                        }
                    }
                }
            });
        }

        private void ExifRotate(Image img)
        {
            if (!img.PropertyIdList.Contains(ExifOrientationID))
                return;

            var prop = img.GetPropertyItem(ExifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);
        }
    }
}
