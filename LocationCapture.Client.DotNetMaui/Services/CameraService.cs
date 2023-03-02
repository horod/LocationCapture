using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class CameraService : ICameraService
    {
        public async Task<byte[]> CapturePhotoWithOrientationAsync()
        {
            byte[] result = null;

            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                
                if (photo != null)
                {
                    using var sourceStream = await photo.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    
                    sourceStream.CopyTo(memoryStream);
                    result = memoryStream.ToArray();
                }
            }

            return result ?? new byte[0];
        }

        public Task<object> CapturePictureAsync()
        {
            return Task.FromResult<object>(null);
        }

        public Task CleanupCameraAsync()
        {
            return Task.CompletedTask;
        }

        public void EndCapturingPhoto()
        {
            var navTarget = Shell.Current.CurrentPage?.BindingContext as CameraViewModel;
            navTarget?.GoBack();
        }

        public async Task InitializeCamera()
        {
            var navTarget = Shell.Current.CurrentPage?.BindingContext as CameraViewModel;
            await navTarget?.OnCaptureSnapshot();
        }

        public void SetCaptureElement(object captureElement)
        {
        }

        public Task StartPreviewAsync()
        {
            return Task.CompletedTask;
        }
    }
}
