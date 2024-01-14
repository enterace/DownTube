using System.Collections.ObjectModel;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace DownTube;

public partial class VideoPage : ContentPage
{
    
	private YoutubeExplode.Videos.Video? currentVideo;
    private YoutubeExplode.Videos.Streams.StreamManifest? currentStreamManifest;
    private ObservableCollection<string> qualityOptions = new ();

    public VideoPage(YoutubeExplode.Videos.Video selectedVideo)
	{
		InitializeComponent();

        // Setting the current selected Video
        currentVideo = selectedVideo;

        // Setting the UI title and duration of the video
        videoTitle.Text = currentVideo.Title;
        videoDuration.Text = currentVideo.Duration.ToString();

        // Changing the visability of the buttons and picker
        manifestButton.IsVisible = true;
        downloadVideoButton.IsVisible = false;
        qualityOptionsPicker.IsVisible = false;

        // Setting the quality picker item source
        qualityOptionsPicker.ItemsSource = qualityOptions;

        // Gettign the largest thumbnail and showing it
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

    private async Task<string> DownloadStream(YoutubeExplode.Videos.Streams.IStreamInfo streamInfo, IProgress<double> progress)
    {
        // Initialize a youtube client
        YoutubeClient youtubeClient = new YoutubeClient();

        // Generate a timestamp
        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss") + DateTime.Now.Microsecond.ToString();

        // Get the platform specific file service
        IFileService FileService = DependencyService.Get<IFileService>();

        // Get the save file path from file service
        string filePath = FileService.GetPublicSavePath($"DownTube-{timestamp}.{streamInfo.Container.Name}");

        // Download the video to the path-
        await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);

        // Return the file path
        return filePath;
    }

    private async void OnVideoDownloadClicked(object sender, EventArgs e)
    {
        // Check if the current manifest is null (Again, it can't be)
        if (currentStreamManifest == null) return;

        // Check if we even have selected a quality option
        if(qualityOptionsPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Error", "No quality setting was selected", "OK");
            return;
        }

        // Disable the download button
        downloadVideoButton.IsEnabled = false;

        // Disable the picker
        qualityOptionsPicker.IsEnabled = false;

        // Error handeling
        try
        {
            // Running on separate thread again
            await Task.Run(async () =>
            {
                // A variable for the selected stream option
                YoutubeExplode.Videos.Streams.IVideoStreamInfo? selectedStreamInfo = null;


                // Getting the stream infos from the stream manifest
                IEnumerable<YoutubeExplode.Videos.Streams.IVideoStreamInfo> streamInfos = currentStreamManifest.GetMuxedStreams();

                // Looping over stream infos
                foreach (YoutubeExplode.Videos.Streams.IVideoStreamInfo streamInfo in streamInfos)
                {
                    // Building the loops quality tag
                    string qualityTag = $"{streamInfo.VideoQuality.Label} - {streamInfo.VideoResolution.Width}x{streamInfo.VideoResolution.Height}";

                    // Building the selected quality tag
                    object? selectedQualityTag = qualityOptionsPicker.ItemsSource[qualityOptionsPicker.SelectedIndex];

                    // Check for null tags
                    if(selectedQualityTag == null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "Selection error: #1", "OK"));
                        return;
                    }

                    // Check if it's string
                    if (selectedQualityTag.GetType() != typeof(string))
                    {
                        MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "Selection error: #2", "OK"));
                        return;
                    }

                    // Check if the stream info tag matches the selected option
                    if (qualityTag == (string) selectedQualityTag)
                    {

                        // Set the selected matching stream if so 
                        selectedStreamInfo = streamInfo;
                    }
                }

                // Check if nothing was selected
                if(selectedStreamInfo == null)
                {
                    MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "Selection error: #3", "OK"));
                    return;
                }

                // The progress percent manager
                IProgress<double> progress = new Progress<double>(percentage =>
                {
                    // Update UI with the download progress
                    MainThread.BeginInvokeOnMainThread(() => downloadVideoButton.Text = $"Downloading... {percentage:P0}");
                });

                // Downloading the stream to disk
                string filePath = await DownloadStream(selectedStreamInfo, progress);

                // Reporting to the user
                MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Success", $"Successfully downloaded the video. Saved to: \n\n{filePath}", "OK"));
            });
        } catch (Exception ex)
        {
            await DisplayAlert("Error", "Couldn't download the video", "OK");
            Console.WriteLine(ex);
        }
        finally
        {
            // Enable the download button that we have disabled
            downloadVideoButton.IsEnabled = true;

            // Enable the quality picker that we have disabled
            qualityOptionsPicker.IsEnabled = true;

            // Change the text of the button
            downloadVideoButton.Text = "Download Video";
        }
        
    }

    private async void OnManifestClicked(object sender, EventArgs e)
    {
        // Checking if current video is null (Which can't be)
        if (currentVideo == null) return;

        // Disabling the button to prevent overusage of  the Event
        manifestButton.IsEnabled = false;

        // Error handeling
        try
        {
            // Checking for storage permissions
            PermissionStatus permissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                await DisplayAlert("Error", "This action needs storage access", "OK");
                manifestButton.IsEnabled = true;
                return;
            }

            // Running all of the time consuming operations on a separate thread (Because of some android limitations)
            await Task.Run(async () =>
            {
                // Running UI operation on the main thread
                // Changing the button text to inform the user
                MainThread.BeginInvokeOnMainThread(() => manifestButton.Text = "Prepairing . . .");

                // Creating a new youtube client
                YoutubeClient youtubeClient = new YoutubeClient();

                // Gathering stream manifest from youtube (It's quite time consuming)
                YoutubeExplode.Videos.Streams.StreamManifest streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(currentVideo.Url);

                // Changing our current stream manifest
                currentStreamManifest = streamManifest;

                // Getting the stream infos from the stream manifest
                IEnumerable<YoutubeExplode.Videos.Streams.IVideoStreamInfo> streamInfos = streamManifest.GetMuxedStreams();

                // Looping over stream infos
                foreach(YoutubeExplode.Videos.Streams.IVideoStreamInfo streamInfo in streamInfos)
                {

                    // Add the qualitiy option to the picker list
                    MainThread.BeginInvokeOnMainThread(() => qualityOptions.Add($"{streamInfo.VideoQuality.Label} - {streamInfo.VideoResolution.Width}x{streamInfo.VideoResolution.Height}"));
                }

                // Show the download button   
                MainThread.BeginInvokeOnMainThread(() => downloadVideoButton.IsVisible = true);

                // Show the quality picker   
                MainThread.BeginInvokeOnMainThread(() => qualityOptionsPicker.IsVisible = true);

                // Hide the manifest button
                MainThread.BeginInvokeOnMainThread(() => manifestButton.IsVisible = false);

                
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
            manifestButton.Text = "Get Manifest";
            manifestButton.IsEnabled = true;
        }
    }
}