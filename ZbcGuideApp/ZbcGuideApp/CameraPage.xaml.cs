using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
        PathDirection dArrow = new PathDirection();
        public CameraPage()
        {
            dArrow.GoDirection += X;
            InitializeComponent();
            #region
            /*          
            Image ImageArrowLeft = new Image
            {
                HeightRequest = 175,
                Source = ImageSource.FromResource("ZbcGuideApp.Img.arrowleft.png")
            };

            Image ImageArrowTurn = new Image
            {
                HeightRequest = 100,
                Source = ImageSource.FromResource("ZbcGuideApp.Img.turnaroundarrow.jpg")
            };

            Image ImageArrorRight = new Image
            {
                HeightRequest = 175,
                Source = ImageSource.FromResource("ZbcGuideApp.Img.arrowright.png")
            };
             */
            #endregion
        }

        void X(int x)
        {
            switch (x)
            {
            case -1:
                arrowright.IsVisible = false;
                arrowleft.IsVisible = true;
                turnaroundarrow.IsVisible = false;
                break;
            case 0:
                turnaroundarrow.IsVisible = true;
                arrowright.IsVisible = false;
                arrowleft.IsVisible = false;
                break;
            case 1:
                arrowleft.IsVisible = false;
                arrowright.IsVisible = true;
                turnaroundarrow.IsVisible = false;
                break;
            default:
                arrowleft.IsVisible = false;
                arrowright.IsVisible = false;
                turnaroundarrow.IsVisible = false;
                break;
            }
            cpCoord.Text = (dArrow.GetDirection(0)).ToString();
        }
    }
}
