using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ZbcGuideApp
{
	//[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GpsLocationXaml : ContentPage
	{
		public GpsLocationXaml ()
		{
			InitializeComponent ();
		}


        private async void Button_Clicked(object sender, EventArgs e)
        {
            await GetLocation();
        }

        private async Task GetLocation()
        {
            Debug.WriteLine("Clicked");
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 20;

            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(1000));
            
            Lang.Text = "Latitude " + position.Latitude.ToString();
            Long.Text = "Longitude " +  position.Longitude.ToString();

            Debug.WriteLine(position.Latitude.ToString());
            Debug.WriteLine(position.Longitude.ToString());
        }

    }
}