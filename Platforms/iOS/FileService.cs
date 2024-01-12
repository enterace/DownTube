namespace DownTube.Platforms.iOS
{
    public class FileService : IFileService
    {
        public string GetPublicSavePath(string fileName)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, fileName);
        }
    }
}
