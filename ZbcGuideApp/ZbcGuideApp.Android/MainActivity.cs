using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using System;

namespace ZbcGuideApp.Droid
{

    [Activity(Label = "ZbcGuideApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            test.context = this;


            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Permission a = ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneState);
            Permission b = ContextCompat.CheckSelfPermission(this, Manifest.Permission.ChangeWifiState);


            if (a != Permission.Granted)
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadPhoneState, Manifest.Permission.ChangeWifiState }, requestCode);

            //if(b != Permission.Granted)
            //    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ChangeWifiState }, requestCode);



            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


    }
}