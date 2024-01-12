using YoutubeExplode;

namespace DownTube
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoadClicked(object sender, EventArgs e)
        {
            String enteredUrl = videoUrlEntry.Text;
            loadButton.IsEnabled = false;

            if (!Uri.TryCreate(enteredUrl, UriKind.Absolute, out _))
            {
                await DisplayAlert("Invalid URL", "Please enter a valid url", "OK");
                loadButton.IsEnabled = true;
                return;
            }

            try {
                YoutubeClient youtubeClient = new YoutubeClient();
                YoutubeExplode.Videos.Video video = await youtubeClient.Videos.GetAsync(enteredUrl);

                await Navigation.PushAsync(new VideoPage(video));
            } catch(Exception exception)
            {
                await DisplayAlert("Error", "We couldn't open the URL", "OK");
                Console.WriteLine(exception );
                loadButton.IsEnabled = true;
                return;
            }
            loadButton.IsEnabled = true;
        }
    }

}
