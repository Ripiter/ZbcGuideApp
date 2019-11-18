using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZbcGuideApp.CustomViews;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace ZbcGuideApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPage : ContentPage
    {
        int a = 0;
        public CameraPage()
        {
            InitializeComponent();
            x();
            Image ImageArrowLeft = new Image
            {
                HeightRequest = 175,
                Source = ImageSource.FromResource("ZbcGuideApp.Img.arrowleft.png")
            };

            Image ImageArrorRight = new Image
            {
                Source = ImageSource.FromResource("ZbcGuideApp.Img.arrowright.png")
            };

        }
        void x()
        {
            switch (WifiConnection.dValues[a])
            {
                case 5:
                    arrowright.IsVisible = true;
                    break;
                case 4:
                    arrowleft.IsVisible = true;
                    break;
                default:
                    arrowleft.IsVisible = false;
                    arrowright.IsVisible = false;
                    break;
            }
        }
    }

}
