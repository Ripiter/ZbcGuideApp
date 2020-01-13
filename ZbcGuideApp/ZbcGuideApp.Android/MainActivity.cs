using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Widget;
using System;
using System.IO;

namespace ZbcGuideApp.Droid
{

    [Activity(Label = "ZbcGuideApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            WifiConnection.context = this;

            // If the app doesnt have permission for access coarse location 
            // ask user about it
            if(ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessCoarseLocation,}, 0);
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessCoarseLocation, }, 1);



            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            // Check if app already got access to permission
            Permission a = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);
            Permission b = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);

            // if appp doesnt have access to that permission ask again
            if (a != Permission.Granted)
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessCoarseLocation }, requestCode);

            if (b != Permission.Granted)
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, requestCode);


            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}