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
                HeightRequest = 175,
                Source = ImageSource.FromResource("ZbcGuideApp.Img.arrowright.png")
            };

            void x()
            {
                arrowleft.IsVisible = true;
                arrowright.IsVisible = false;
            }
        }
    }
}