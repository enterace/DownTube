using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace DownTube;

public partial class VideoPage : ContentPage
{
	YoutubeExplode.Videos.Video? currentVideo;
    public VideoPage(YoutubeExplode.Videos.Video selectedVideo)
	{
		InitializeComponent();
        currentVideo = selectedVideo;
        videoTitle.Text = currentVideo.Title;
        videoDuration.Text = currentVideo.Duration.ToString();

        YoutubeExplode.Common.Thumbnail biggestThumbnail = currentVideo.Thumbnails[0];
        foreach (YoutubeExplode.Common.Thumbnail thumbnail in currentVideo.Thumbnails)
        {
            if (thumbnail.Resolution.Area.CompareTo(biggestThumbnail.Resolution.Area) > 0)
            {
                biggestThumbnail = thumbnail;
            }
        }

        videoThumbnail.Source = ImageSource.FromUri(new Uri(biggestThumbnail.Url));
    }

    private async void OnDownloadClicked(object sender, EventArgs e)
    {
        if (currentVideo == null) return;

        downloadButton.IsEnabled = false;

        try
        {
            PermissionStatus permissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                await DisplayAlert("Error", "No permission to save the file", "OK");
                downloadButton.IsEnabled = true;
                return;
            }

            await Task.Run(async () =>
            {
                YoutubeClient youtubeClient = new YoutubeClient();
                MainThread.BeginInvokeOnMainThread(() => downloadButton.Text = "Prepairing . . .");
                YoutubeExplode.Videos.Streams.StreamManifest streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(currentVideo.Url);
                YoutubeExplode.Videos.Streams.IVideoStreamInfo streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");

                var FileService = DependencyService.Get<IFileService>();

                string filePath = FileService.GetPublicSavePath($"DownTube-{timestamp}.{streamInfo.Container.Name}");

                IProgress<double> progress = new Progress<double>(percentage =>
                {
                    // Update UI with the download progress (e.g., display the percentage)
                    MainThread.BeginInvokeOnMainThread(() => downloadButton.Text = $"Downloading... {percentage:P0}");
                });

                await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);

                MainThread.BeginInvokeOnMainThread(async() => await DisplayAlert("Success", $"Successfully downloaded the video. Saved to: {filePath}", "OK"));
            });
        }
        catch (YoutubeExplode.Exceptions.VideoUnavailableException) {
            await DisplayAlert("Error", "This video cannot be downloaded", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Couldn't download the video", "OK");
            Console.WriteLine(ex);
        }
        finally
        {
            downloadButton.Text = "Download Video";
            downloadButton.IsEnabled = true;
        }
    }
}