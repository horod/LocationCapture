using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface ICameraService
    {
        void SetCaptureElement(object captureElement);
        Task InitializeCamera();
        Task StartPreviewAsync();
        Task<byte[]> CapturePhotoWithOrientationAsync();
        Task<object> CapturePictureAsync();
        Task CleanupCameraAsync();
        void EndCapturingPhoto();
    }
}
