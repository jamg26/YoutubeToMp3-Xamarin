using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Plugin.Permissions;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YoutubeMp3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}