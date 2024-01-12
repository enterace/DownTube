using DownTube.Platforms.iOS;
using Foundation;

namespace DownTube
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {

        protected override MauiApp CreateMauiApp()
        {
            DependencyService.Register<IFileService, FileService>();
            return MauiProgram.CreateMauiApp();
        }
    }
}
