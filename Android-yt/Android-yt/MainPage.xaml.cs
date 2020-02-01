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

namespace YoutubeMp3
{
    
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        
        private int totalCount, CurrentCount;
        private static readonly HttpClient client = new HttpClient();

        public MainPage()
        {
            InitializeComponent();
            try
            {
                _ = getSetting();
            }
            catch(Exception e)
            {
                txtStatus.Text += e.Message;
            }
        }

        private async Task getSetting()
        {
            var response = await client.GetAsync("http://iswine.ml/.jamg/enabled");
            string responseString = await response.Content.ReadAsStringAsync();
            if (responseString.ToString().Replace("\n", string.Empty) != "true")
            {
                Device.BeginInvokeOnMainThread(async () => { 
                    await DisplayAlert("Alert", "This app is under maintenance", "OK");
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                });
            }
            
        }

        private void btnDownload_Clicked_1(object sender, EventArgs e)
        {
            btnDownload.IsEnabled = false;
            string[] url = { };
            try {
                url = txtUrl.Text.Split('\n');
                int count = url.Length;
                CurrentCount = 0;
                totalCount = count;
                txtStatus.Text += $"\nDownloading {totalCount} Files...\n";
                for (int x = 0; x < count; x++)
                {
                    if (url[x].Contains("youtube.com/watch?v=") || url[x].Contains("youtu.be"))
                    {
                        _ = GetVideo(url[x]);
                    }
                    else
                    {
                        var context = Android.App.Application.Context;
                        var tostMessage = $"{url[x]} is invalid.";
                        var durtion = ToastLength.Long;
                        Toast.MakeText(context, tostMessage, durtion).Show();
                    }
                }



            } catch { 
                txtStatus.Text += $"\nURL Cannot be empty!\n";
                btnDownload.IsEnabled = true;
            }

            
        }

        private async Task GetVideo(string url)
        {

            txtStatus.Text += $"Fetching {url}\n";

            var id = YoutubeClient.ParseVideoId(url);
            var client = new YoutubeClient();
            var video = await client.GetVideoAsync(id);

            string path = @"/mnt/sdcard/AA-DownloadedMusic/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            txtStatus.Text += $"Get Stream Info Set...\n";
            var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);
            txtStatus.Text += $"Geting Audio...\n";
            var streamInfo = await Task.Run(() => streamInfoSet.Audio.WithHighestBitrate());

            //var ext = streamInfo.Container.GetFileExtension();
            txtStatus.Text += $"Saving to file...\n";
            // Download stream to file
            try
            {
                await client.DownloadMediaStreamAsync(streamInfo, $@"{path}{video.Title.Replace('/', ' ')}.mp3");
            } catch (Exception e)
            {
                txtStatus.Text += $"\n=======================\nReport this error to me\n{e.Message}\n=======================\n\n";
            }

            txtStatus.Text += $"{CurrentCount + 1}/{totalCount} completed\n";
            CurrentCount++;

            if (totalCount == CurrentCount)
            {
                txtStatus.Text += "\nTask Completed Successfully!\nIf you found bugs and errors please contact me.\n\nFB: Jamuel Galicia";
                btnDownload.IsEnabled = true;
                GC.Collect();
            }
        }
    }
}