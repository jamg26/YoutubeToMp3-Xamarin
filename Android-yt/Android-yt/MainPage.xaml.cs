using Android.Widget;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using System.Net.Http;
using YoutubeExplode.Converter;

namespace YoutubeMp3
{

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        private int totalCount, CurrentCount;
        private static readonly HttpClient client = new HttpClient();
        private string activated, message, option;
        private Android.Content.Context context = Android.App.Application.Context;

        public MainPage()
        {
            InitializeComponent();
        }

        private async Task isActivated()
        {
            
            var response = await client.GetAsync("http://pastebin.com/raw/Kc2p01gk");
            string responseString = await response.Content.ReadAsStringAsync();

            string[] res = responseString.ToString().Split('\n');
            activated = res[0].Trim();
            message = res[1];

            if (activated != "true")
            {
                await DisplayAlert("Alert", message, "OK");
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
        }

        private async void btnDownload_Clicked_1(object sender, EventArgs e)
        {
            btnDownload.IsEnabled = false;
            _ = isActivated();
            string[] url = { };
            txtStatus.Text += $"\nPlease wait while I'm doing your request.\n";
            try
            {
                option = picker.SelectedItem.ToString();
                url = txtUrl.Text.Split('\n');
                int count = url.Length;
                CurrentCount = 0;
                totalCount = count;

                btnDownload.IsEnabled = false;
                for (int x = 0; x < count; x++)
                {
                    if (url[x].Contains("youtube.com/watch?v=") || url[x].Contains("youtu.be"))
                    {
                        await GetVideo(url[x]);
                    }
                    else
                    {
                        txtStatus.Text += $"\n{url[x]} is invalid.\n";
                    }
                }
                Toast.MakeText(context, "Done", ToastLength.Long);
                btnDownload.IsEnabled = true;
        }
        catch (Exception ee)
        {
            txtStatus.Text += $"\n=======================\nERROR\n{ee.Message}\n=======================\n\n";
            btnDownload.IsEnabled = true;
        }


}

        private async Task GetVideo(string url)
        {

            txtStatus.Text += $"Fetching\n{url}\n";
            var converter = new YoutubeConverter();
            var id = YoutubeClient.ParseVideoId(url);
            var client = new YoutubeClient();
            
            var video = await client.GetVideoAsync(id);

            string path = @"/mnt/sdcard/Downloads/YoutubeToMp3/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            txtStatus.Text += $"Get Stream Info Set...\n";
            var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);

            if (option == "MP3")
            {
                txtStatus.Text += $"Geting Audio...\n";
                var streamInfo = streamInfoSet.Audio.WithHighestBitrate();


                var ext = streamInfo.Container.GetFileExtension();

                txtStatus.Text += $"Saving to file...\n";
                // Download stream to file
                try
                {
                    string audioPath = $@"{path}{video.Title.Replace('/', ' ')}.";
                    await client.DownloadMediaStreamAsync(streamInfo, $@"{audioPath}.{ext}");


                    //var mediaStreamInfos = new MediaStreamInfo[] { streamInfo };
                    //await converter.DownloadAndProcessMediaStreamsAsync(mediaStreamInfos, $@"{audioPath}.mp3", "mp3");
                }
                catch (Exception e)
                {
                    txtStatus.Text += $"\n=======================\nReport this error to me\n{e.Message}\n=======================\n\n";
                }

            }
            else
            {
                txtStatus.Text += $"Geting Video...\n";
                var streamInfo = streamInfoSet.Muxed.WithHighestVideoQuality();

                var ext = streamInfo.Container.GetFileExtension();

                txtStatus.Text += $"Saving to file...\n";
                // Download stream to file
                try
                {
                    await client.DownloadMediaStreamAsync(streamInfo, $@"{path}{video.Title.Replace('/', ' ')}.{ext}");
                }
                catch (Exception e)
                {
                    txtStatus.Text += $"\n=======================\nReport this error to me\n{e.Message}\n=======================\n\n";
                }
            }

            txtStatus.Text += $"{CurrentCount + 1}/{totalCount} completed\n";
            CurrentCount++;

            if (totalCount == CurrentCount)
            {
                txtStatus.Text += "\nTask Completed Successfully!\nIf you found bugs and errors please contact me.\n\nFB: Jamuel Galicia\n";
                btnDownload.IsEnabled = true;
                GC.Collect();
            }


        }
    }
}