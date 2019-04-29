using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Storage.FileProperties;

namespace LocationCapture.Client.UWP.Services
{
    public interface ICameraRotationHelper : IDisposable
    {
        event EventHandler<bool> OrientationChanged;

        void SetCameraEnclosureLocation(EnclosureLocation cameraEnclosureLocation);

        bool IsEnclosureLocationExternal(EnclosureLocation enclosureLocation);

        SimpleOrientation GetUIOrientation();

        SimpleOrientation GetCameraCaptureOrientation();

        SimpleOrientation GetCameraPreviewOrientation();

        PhotoOrientation ConvertSimpleOrientationToPhotoOrientation(SimpleOrientation orientation);

        int ConvertSimpleOrientationToClockwiseDegrees(SimpleOrientation orientation);

        SimpleOrientation MirrorOrientation(SimpleOrientation orientation);
    }
}
