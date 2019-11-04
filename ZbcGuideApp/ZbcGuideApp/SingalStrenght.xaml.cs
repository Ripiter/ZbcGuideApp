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
using System.Diagnostics;
using Android.Graphics;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace ZbcGuideApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingalStrenght : ContentPage
	{
        #region Scaling
        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 4;
        private const double OVERSHOOT = 0.15;
        private double StartScale, LastScale;
        private double StartX, StartY;
        #endregion
        
        SKBitmap resourceBitmap;
        
        

       // SKCanvas canvas;

        //WifiReceiver rec = new WifiReceiver();
        WifiConnection wifi;

        public SingalStrenght()
        {
            InitializeComponent();
            wifi = new WifiConnection();
            //rec.PathFound += DrawingOnCanvas;
            wifi.PathFound += DrawingOnCanvas;
            using (Stream stream = Android.App.Application.Context.Assets.Open("pathing.bmp"))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }
            //Content = CanvasView;
            GetStrenght();

          

        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            // In this example, we will draw a circle in the middle of the canvas


            //float scale = Math.Min((float)info.Width / resourceBitmap.Width,
            //                   info.Height / 3f / resourceBitmap.Height);

            //float left = (info.Width - scale * resourceBitmap.Width) / 2;
            //float top = (info.Height / 3 - scale * resourceBitmap.Height) / 2;
            //float right = left + scale * resourceBitmap.Width;
            //float bottom = top + scale * resourceBitmap.Height;
            //SKRect rect = new SKRect(left, top, right, bottom);
            //rect.Offset(0, 2 * info.Height / 3);

            //canvas.DrawBitmap(resourceBitmap, rect);
            
            canvas.DrawBitmap(resourceBitmap, new SKRect(0, info.Height / 3, info.Width, 2 * info.Height / 3));

            //DrawPoint();

            //canvas.DrawBitmap(resourceBitmap,  new SKRect(0, info.Height / 2, info.Width, 2 * info.Height));
        }

        #region Methods for zoomning in and out
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        private void OnTapped(object sender, EventArgs e)
        {
            if (Scale > MIN_SCALE)
            {
                this.ScaleTo(MIN_SCALE, 250, Easing.CubicInOut);
                this.TranslateTo(0, 0, 250, Easing.CubicInOut);
            }
            else
            {
                AnchorX = AnchorY = 0.5; //TODO tapped position
                this.ScaleTo(MAX_SCALE, 250, Easing.CubicInOut);
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    StartX = (1 - AnchorX) * Width;
                    StartY = (1 - AnchorY) * Height;
                    break;
                case GestureStatus.Running:
                    AnchorX = Clamp(1 - (StartX + e.TotalX) / Width, 0, 1);
                    AnchorY = Clamp(1 - (StartY + e.TotalY) / Height, 0, 1);
                    break;
            }
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    LastScale = e.Scale;
                    StartScale = Scale;
                    AnchorX = e.ScaleOrigin.X;
                    AnchorY = e.ScaleOrigin.Y;
                    break;
                case GestureStatus.Running:
                    if (e.Scale < 0 || Math.Abs(LastScale - e.Scale) > (LastScale * 1.3) - LastScale)
                    { return; }
                    LastScale = e.Scale;
                    var current = Scale + (e.Scale - 1) * StartScale;
                    Scale = Clamp(current, MIN_SCALE * (1 - OVERSHOOT), MAX_SCALE * (1 + OVERSHOOT));
                    break;
                case GestureStatus.Completed:
                    if (Scale > MAX_SCALE)
                        this.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);
                    else if (Scale < MIN_SCALE)
                        this.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
                    break;
            }
        }

        private T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
        {
            if (value.CompareTo(minimum) < 0)
                return minimum;
            else if (value.CompareTo(maximum) > 0)
                return maximum;
            else
                return value;
        }
        #endregion

        //private async void GetStrenght(object sender, EventArgs e)
        private async void GetStrenght()
        {
            if (WifiConnection.searching == true)
            {
                Debug.WriteLine(WifiConnection.searching);
                return;
            }
            
            LocationManager mc = (LocationManager)WifiConnection.context.GetSystemService(Context.LocationService);
            if (mc.IsProviderEnabled(LocationManager.GpsProvider))
                Debug.WriteLine("Enabled");
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

            
            Debug.WriteLine("button clicked");
            wifi.GetWifiNetworks();
            //wifiChecker.Start();
            Debug.WriteLine("Searching for wifi");

            // path finding
            //wifi.Test();

            


            //Thread tr = new Thread(WaitingForUpdate);
            //tr.Start();
        }

        private void DrawingOnCanvas(object senderr, EventArgs ee)
        {
            //for (int i = 0; i < 100; i++)
            //{
            //    canvas.DrawPoint(wifi.xValues[i], wifi.yValues[i], SKColor.Parse("#ff0000"));
            //}
            
            CanvasView.InvalidateSurface();
            CanvasView.PaintSurface += (sender, e) => {
                SKSurface surface = e.Surface;
                int surfaceWidth = e.Info.Width;
                int surfaceHeight = e.Info.Height;

                SKCanvas canvas = surface.Canvas;

                // draw on the canvas
                for (int i = 0; i < 100; i++)
                {
                    canvas.DrawPoint(wifi.xValues[i], wifi.yValues[i], SKColor.Parse("#ff0000"));
                }

                canvas.Flush();
            };
        }
    }
}

