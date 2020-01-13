using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZbcGuideApp.Services;

namespace ZbcGuideApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QrCodeMain : ContentPage
    {
        public QrCodeMain()
        {
            InitializeComponent();
            CameraStart();
        }

        private async void CameraStart()
        {
            var scanner = DependencyService.Get<IQrScanningService>();
            var result = await scanner.ScanAsync();
            if (result != null)
            {
                txtBarcode.Text = result;
                string[] res = result.Split('-');

                OurPosition.xPos = uint.Parse(res[0]);
                OurPosition.yPos = uint.Parse(res[1]);
                OurPosition.floor = int.Parse(res[2]);
                OurPosition.scanned = true;
                await Navigation.PopAsync();
            }
            else
                txtBarcode.Text = "Error plz try again";

        }


        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            var scanner = DependencyService.Get<IQrScanningService>();
            var result = await scanner.ScanAsync();
            if (result != null)
            {
                txtBarcode.Text = result;
                string[] res = result.Split('-');

                OurPosition.xPos  = uint.Parse(res[0]);
                OurPosition.yPos  = uint.Parse(res[1]);
                OurPosition.floor = int.Parse(res[2]);
                OurPosition.scanned = true;
                await Navigation.PopAsync();
            }

        }
    }
}