using YoutubeExplode;

namespace DownTube
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // The event handler for the download button
        private async void OnLoadClicked(object sender, EventArgs e)
        {
            // Getting the video url entry
            string enteredUrl = videoUrlEntry.Text;

            // Disabling the load button to prevent overuse
            loadButton.IsEnabled = false;

            // Checing if the entry is even a URL
            if (!Uri.TryCreate(enteredUrl, UriKind.Absolute, out _))
            {
                await DisplayAlert("Invalid URL", "Please enter a valid url", "OK");
                loadButton.IsEnabled = true;
                return;
            }

            // Event handler
            try {
                
                // Creating a new youtube client
                YoutubeClient youtubeClient = new YoutubeClient();

                // Getting the video object
                YoutubeExplode.Videos.Video video = await youtubeClient.Videos.GetAsync(enteredUrl);

                // Opening a new video page with the video object
                await Navigation.PushAsync(new VideoPage(video));
            } catch(Exception exception)
            {
                await DisplayAlert("Error", "We couldn't open the URL", "OK");
                Console.WriteLine(exception );
                loadButton.IsEnabled = true;
                return;
            }

            // Resetting the load button
            loadButton.IsEnabled = true;
        }
    }

}
