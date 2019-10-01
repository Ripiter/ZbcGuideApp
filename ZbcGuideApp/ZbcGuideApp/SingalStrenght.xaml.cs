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

namespace ZbcGuideApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingalStrenght : ContentPage
	{
        test a;
        public SingalStrenght()
        {
            InitializeComponent();
            a = new test();
            //wifiChecker = new Thread(CheckForWifi);
        }

        private void GetStrenght(object sender, EventArgs e)
        {
            


            if (test.oc.Count != 0)
                test.oc.Clear();
            System.Diagnostics.Debug.WriteLine("button clicked");
            a.GetWifiNetworks();
            //wifiChecker.Start();
            System.Diagnostics.Debug.WriteLine("Searching for wifi");

            ListOfAccessPoints.ItemsSource = test.oc;

            ////List<AccessPoint> testData = new List<AccessPoint>() { new AccessPoint() { PrintInfo = "a" }, new AccessPoint() { Ssid = "0", Strenght = -1, PrintInfo = "Bye" } };
            ////ListOfAccessPoints.ItemsSource = accessPoints;
        }
    }

}