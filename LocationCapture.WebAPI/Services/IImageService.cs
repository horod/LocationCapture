using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationCapture.WebAPI.Services
{
    public interface IImageService
    {
        Task<byte[]> GetThumbnailAsync(string imagePath);
    }
}
