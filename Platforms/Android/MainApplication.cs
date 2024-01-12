using Android.App;
using Android.Runtime;
using DownTube.Platforms.Droid;

namespace DownTube
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {  
        }

        protected override MauiApp CreateMauiApp()
        {
            DependencyService.Register<IFileService, FileService>();
            return MauiProgram.CreateMauiApp();
        }
    }
}
