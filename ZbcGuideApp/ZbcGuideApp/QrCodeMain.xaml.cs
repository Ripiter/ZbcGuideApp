using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            try
            {
                var scanner = DependencyService.Get<IQrScanningService>();
                var result = await scanner.ScanAsync();
                if (result != null)
                {
                    txtBarcode.Text = result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}