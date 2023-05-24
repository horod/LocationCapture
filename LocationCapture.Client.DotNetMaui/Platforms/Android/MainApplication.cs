using Android.App;
using Android.Runtime;

//// Needed for Picking photo/video
//[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]

//// Needed for Taking photo/video
//[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
//[assembly: UsesPermission(Android.Manifest.Permission.Camera)]

//// Needed for Geolocation
//[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
//[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
//[assembly: UsesFeature("android.hardware.location", Required = false)]
//[assembly: UsesFeature("android.hardware.location.gps", Required = false)]
//[assembly: UsesFeature("android.hardware.location.network", Required = false)]

namespace LocationCapture.Client.DotNetMaui
{
    [Application(UsesCleartextTraffic = true, LargeHeap = true)]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}