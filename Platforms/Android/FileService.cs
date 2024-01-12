namespace DownTube.Platforms.Droid
{
    public class FileService : IFileService
    {
        public string GetPublicSavePath(string fileName)
        {
            var folderPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            if (folderPath == null) return "";
            return Path.Combine(folderPath.AbsolutePath, fileName);
        }
    }
}
