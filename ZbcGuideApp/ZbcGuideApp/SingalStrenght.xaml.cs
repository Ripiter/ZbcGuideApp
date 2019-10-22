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
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace ZbcGuideApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingalStrenght : ContentPage
	{
        SKBitmap resourceBitmap;
        SKMatrix matrix = SKMatrix.MakeIdentity();
        WifiConnection a;
        public SingalStrenght()
        {
            
            InitializeComponent();
            a = new WifiConnection();
            //wifiChecker = new Thread(CheckForWifi);
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;

            using (Stream abc = Android.App.Application.Context.Assets.Open("potato.bmp"))
                resourceBitmap = SKBitmap.Decode(abc);
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            if (resourceBitmap != null)
            {
                canvas.Scale(-1, 1, info.Width / 2, 0);
                canvas.SetMatrix(matrix);
                
                //works
                resourceBitmap.SetPixel(1, 1, SKColor.Parse("#000000"));

                
                canvas.DrawBitmap(resourceBitmap, new SKRect(0, info.Height / 3, info.Width, 2 * info.Height / 3));
            }
        }


        private async void GetStrenght(object sender, EventArgs e)
        {
            if (WifiConnection.searching == true)
            {
                System.Diagnostics.Debug.WriteLine(WifiConnection.searching);
                return;
            }

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