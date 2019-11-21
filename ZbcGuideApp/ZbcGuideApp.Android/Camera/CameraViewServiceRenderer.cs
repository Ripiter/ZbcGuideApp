using Android.Content;
using Android.Graphics;
using ZbcGuideApp.Droid.Camera;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CameraPreview = ZbcGuideApp.CustomViews.CameraPreview;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraViewServiceRenderer))]
namespace ZbcGuideApp.Droid.Camera
{
    public class CameraViewServiceRenderer : ViewRenderer<CameraPreview, CameraDroid>
    {
        private CameraDroid camera;
        private CameraPreview currentElement;
        private readonly Context context;

        public CameraViewServiceRenderer(Context context) : base(context)
        {
            this.context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            camera = new CameraDroid(Context);

            SetNativeControl(camera);

            if (e.NewElement != null && camera != null)
            {
                currentElement = e.NewElement;
                camera.SetCameraOption(currentElement.Camera);
            }
        }
    }
}