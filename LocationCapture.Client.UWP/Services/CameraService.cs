using LocationCapture.BL;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class CameraService : ICameraService
    {
        private readonly IDialogService _dialogService;
        private CaptureElement _captureElement;
        private MediaCapture _mediaCapture;
        private DisplayRequest _displayRequest;
        private bool _isPreviewing;
        private ICameraRotationHelper _cameraRotationHelper;
        private DeviceInformation _cameraDevice;
        private bool _externalCamera;
        private IBitmapConverter _bitmapConverter;
        private readonly ILoggingService _loggingService;

        public CameraService(IDialogService dialogService,
            ICameraRotationHelper cameraRotationHelper,
            IBitmapConverter bitmapConverter,
            ILoggingService loggingService)
        {
            _dialogService = dialogService;
            _cameraRotationHelper = cameraRotationHelper;
            _bitmapConverter = bitmapConverter;
            _loggingService = loggingService;
        }

        public void SetCaptureElement(object captureElement)
        {
            _captureElement = (CaptureElement)captureElement;
        }

        public async Task InitializeCamera()
        {
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null
                && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
            _cameraDevice = desiredDevice ?? allVideoDevices.FirstOrDefault();

            if (_cameraDevice == null)
            {
                _loggingService.Warning("No camera device found!");
                await _dialogService.ShowAsync("No camera device found!");
                return;
            }

            var settings = new MediaCaptureInitializationSettings { VideoDeviceId = _cameraDevice.Id };
            _mediaCapture = new MediaCapture();

            try
            {
                await _mediaCapture.InitializeAsync();
            }
            catch (UnauthorizedAccessException ex)
            {
                _loggingService.Warning("The app was denied access to the camera. Details: {Ex}", ex);
                await _dialogService.ShowAsync("The app was denied access to the camera.");
                return;
            }

            // Handle camera device location
            if (_cameraDevice.EnclosureLocation == null ||
                _cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
            {
                _externalCamera = true;
            }
            else
            {
                _externalCamera = false;
            }

            _cameraRotationHelper.SetCameraEnclosureLocation(_cameraDevice.EnclosureLocation);
            _cameraRotationHelper.OrientationChanged += CameraOrientationChanged;

            _captureElement.Source = _mediaCapture;

            await _mediaCapture.StartPreviewAsync();
            await SetPreviewRotationAsync();
            _isPreviewing = true;
        }

        private async void CameraOrientationChanged(object sender, bool updatePreview)
        {
            if (updatePreview)
            {
                await SetPreviewRotationAsync();
            }
        }

        private async Task SetPreviewRotationAsync()
        {
            if (!_externalCamera)
            {
                // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
                var rotation = _cameraRotationHelper.GetCameraPreviewOrientation();
                var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
                Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");
                var displayRotationClockwise = 360 - _cameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(rotation);
                props.Properties.Add(RotationKey, displayRotationClockwise);
                await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
            }
        }

        public async Task<byte[]> CapturePhotoWithOrientationAsync()
        {
            var captureStream = new InMemoryRandomAccessStream();
            var outputStream = new InMemoryRandomAccessStream();

            try
            {
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
            }
            catch (Exception ex)
            {
                _loggingService.Warning("Exception when taking a photo: {Ex}", ex);
                await _dialogService.ShowAsync("Could not take a photo. Please try again.");
                return new byte[0];
            }

            var decoder = await BitmapDecoder.CreateAsync(captureStream);
            var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);
            var cameraOrientation = _cameraRotationHelper.GetCameraCaptureOrientation();
            var simplePhotoOrientation = _cameraRotationHelper.MirrorOrientation(cameraOrientation);
            var photoOrientation = _cameraRotationHelper.ConvertSimpleOrientationToPhotoOrientation(simplePhotoOrientation);
            var properties = new BitmapPropertySet {{ "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };
            await encoder.BitmapProperties.SetPropertiesAsync(properties);
            await encoder.FlushAsync();
            var rawBytes = await _bitmapConverter.GetBytesFromStream(outputStream);

            return rawBytes;
        }

        public async Task StartPreviewAsync()
        {
            _mediaCapture = new MediaCapture();
            _displayRequest = new DisplayRequest();

            try
            {
                await _mediaCapture.InitializeAsync();

                _displayRequest.RequestActive();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException ex)
            {
                _loggingService.Warning("The app was denied access to the camera. Details: {Ex}", ex);
                await _dialogService.ShowAsync("The app was denied access to the camera.");
                return;
            }

            try
            {
                _captureElement.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();
                _isPreviewing = true;
            }
            catch (System.IO.FileLoadException ex)
            {
                _loggingService.Warning("The camera preview can't be displayed, because another app has exclusive access. Details: {Ex}", ex);
                await _dialogService.ShowAsync("The camera preview can't be displayed, because another app has exclusive access.");
            }
        }

        public async Task<object> CapturePictureAsync()
        {
            var encodingProperties = ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8);
            var lowLagCapture = await _mediaCapture.PrepareLowLagPhotoCaptureAsync(encodingProperties);

            var capturedPhoto = await lowLagCapture.CaptureAsync();
            var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

            await lowLagCapture.FinishAsync();

            return softwareBitmap;
        }

        public async Task CleanupCameraAsync()
        {
            if (_mediaCapture != null)
            {
                if (_isPreviewing)
                {
                    await _mediaCapture.StopPreviewAsync();
                }

                await _captureElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _captureElement.Source = null;

                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                });

                _cameraRotationHelper.Dispose();
            }
        }
    }
}
