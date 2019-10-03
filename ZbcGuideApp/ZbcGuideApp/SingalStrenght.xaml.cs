using Android.Content;
using Android.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Android;
using Android.Locations;

namespace ZbcGuideApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingalStrenght : ContentPage
	{
        WifiConnection a;
        public SingalStrenght()
        {
            InitializeComponent();
            a = new WifiConnection();
            
            //wifiChecker = new Thread(CheckForWifi);
        }

        private async void GetStrenght(object sender, EventArgs e)
        {
            LocationManager mc = (LocationManager)WifiConnection.context.GetSystemService(Context.LocationService);
            if (mc.IsProviderEnabled(LocationManager.GpsProvider))
                System.Diagnostics.Debug.WriteLine("Enabled");
            else
            {
                bool x = await DisplayAlert("Need Gps", "Need Gps", "ok", "no");

                if(x == true)
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    intent.AddFlags(ActivityFlags.MultipleTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
            }


            if (WifiConnection.oc.Count != 0)
                WifiConnection.oc.Clear();
            System.Diagnostics.Debug.WriteLine("button clicked");
            a.GetWifiNetworks();
            //wifiChecker.Start();
            System.Diagnostics.Debug.WriteLine("Searching for wifi");

            ListOfAccessPoints.ItemsSource = WifiConnection.oc;

            ////List<AccessPoint> testData = new List<AccessPoint>() { new AccessPoint() { PrintInfo = "a" }, new AccessPoint() { Ssid = "0", Strenght = -1, PrintInfo = "Bye" } };
            ////ListOfAccessPoints.ItemsSource = accessPoints;
        }
    }

}