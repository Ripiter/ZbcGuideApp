using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ZbcGuideApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            var image = new Image { Source = "Picture.jpg" };
            image.Source = Device.RuntimePlatform == Device.Android ? ImageSource.FromFile("Picture.jpg") : ImageSource.FromFile("Images/Picture.jpg");
        }

    }
}
