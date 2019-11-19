using Android.Content;
using Android.Net.Wifi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace ZbcGuideApp
{
    public class WifiConnection :  BroadcastReceiver
    {
        public static Context context = null;
        public static WifiManager wifi;
        public static bool searching = false;

        string printInfo = string.Empty;
        string jsonString = string.Empty;
        List<AccessPoint> accessPoints = new List<AccessPoint>();
        List<AccesPoint> testData = new List<AccesPoint>();

        public void GetWifiNetworks()
        {
            searching = true;
            // Get a handle to the Wifi
            wifi = (WifiManager)context.GetSystemService(Context.WifiService);

            // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
            //if (wifiReceiver == null)
            //    wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(this, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            try
            {
                Debug.WriteLine(wifi.StartScan());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            IList<ScanResult>  scanwifinetworks = WifiConnection.wifi.ScanResults;
            foreach (ScanResult wifinetwork in scanwifinetworks)
            {
                Debug.WriteLine(wifinetwork.Bssid + wifinetwork.Ssid);
                //printInfo = "Mac address " + wifinetwork.Bssid + " with name " + wifinetwork.Ssid + " with lvl dBm " + wifinetwork.Level;
                //printInfo = wifinetwork.Ssid + " with lvl dBm " + wifinetwork.Level;
                //printInfo = "X Y cords " + SerializeJson(wifinetwork.Bssid) + " of " + wifinetwork.Bssid;
                //printInfo = "Network " + wifinetwork.Ssid + " is " + Distance(wifinetwork.Level, wifinetwork.Frequency).ToString("n") + "m away";

                //oc.Add(new AccessPoint() { Mac=wifinetwork.Bssid, Ssid = wifinetwork.Ssid, Strenght = wifinetwork.Level, PrintInfo = printInfo });
                accessPoints.Add(new AccessPoint() { Mac = wifinetwork.Bssid, Ssid = wifinetwork.Ssid, Strenght = wifinetwork.Level, PrintInfo = printInfo });
                //Debug.WriteLine(printInfo);
            }
            Results(accessPoints);
            WifiConnection.wifi.ScanResults.Clear();
            scanwifinetworks.Clear();
            accessPoints.Clear();

        }

        private void Results(List<AccessPoint> scanResults)
        {
            try
            { 
                ReadingJson();
                foreach (AccessPoint item in scanResults)
                {
                    SerializeJson(item.Mac, item.Strenght);
                }

                XyCords(testData[0].Strenght, testData[1].Strenght, testData[2].Strenght);
            }
            catch (Exception e)
            {

            }

            WifiConnection.searching = false;
        }


        /// <summary>
        /// Gets somewhat accurate position of network in meters
        /// </summary>
        private double Distance(int signaldBm, double freqMhz)
        {
            double exp = (27.55 - (20 * Math.Log10(freqMhz)) + Math.Abs(signaldBm)) / 20;
            return Math.Pow(10, exp);
        }
        
        ImportBitmap importBitmap = null;
        public string progress = "";
        private void XyCords(int s1, int s2, int s3)
        {
            if (importBitmap == null)
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(WifiConnection)).Assembly;
                Stream stream = assembly.GetManifestResourceStream("ZbcGuideApp.mapOfRoskilde.bmp");
                //progress = "Loading files";
                //StatusChanged(this, new EventArgs());
                Debug.WriteLine("started here");
                importBitmap = new ImportBitmap();
                importBitmap.Import24Bitmap(stream);
            }

            int x = 0;
            int y = 0;
            try
            {
                double px = ((s1 * s1) - (s2 * s2) + (testData[1].X * testData[1].X)) / ((double)(2 * testData[1].X));

                double py = ((s1 * s1) - (s3 * s3) + (testData[2].X * testData[2].X) + (testData[2].Y * testData[2].Y)) / (2 * testData[2].Y) - (testData[2].X / (double)testData[2].Y) * px;

                px = px * 2.01;
                py = py * 1.79;

                x = (int)px;
                y = (int)py;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                wifi.StartScan();
            }


            //progress = "Finding path";
            //StatusChanged(this, new EventArgs()); 
            PathFinding pathFinding = new PathFinding();
            pathFinding.GenerateNewMap((int)importBitmap.BMPHeight, (int)importBitmap.BMPWidth, 0, 0xff, importBitmap.BMPMapArray);

            // Path to ubuy
            pathFinding.GeneratePath(x, y, 175, 460, false);
            //pathFinding.GeneratePath(415, 478, 364, 506, false);

            xValues = pathFinding.XPath;
            yValues = pathFinding.YPath;
            dValues = pathFinding.DirectionPath;

            //Debug.WriteLine("Our Location (calc): X:" + Math.Round(px) + " Y: " + Math.Round(py));
            PathWasFound();
            testData.Clear();
        }
        public int[] xValues = null;
        public int[] yValues = null;
        public static int[] dValues = null;

        public void PathWasFound()
        {
            PathFound(this, new EventArgs());
        } 

        public event EventHandler PathFound;
        public event EventHandler StatusChanged;

        private void ReadingJson()
        {
            string s = string.Empty;
            using (Stream readingStream = Android.App.Application.Context.Assets.Open("jsonFormated.json"))
            {
                byte[] temp = new byte[10];
                UTF8Encoding encoding = new UTF8Encoding(true);

                int len = 0;

                while ((len = readingStream.Read(temp, 0, temp.Length)) > 0)
                {
                    // Converts to string.
                    s += encoding.GetString(temp, 0, len);
                }
            }
            jsonString = s;
        }

        private void SerializeJson(string mac, int strenght)
        {
            try
            {
                JObject json = JObject.Parse(jsonString);
                JToken jToken = json[mac];

                int x = (int)jToken["x"];
                int y = (int)jToken["y"];
                Debug.WriteLine($"x {x} y {y} of {mac}");
                testData.Add(new AccesPoint { X = x, Y = y, Strenght = strenght });
            }
            catch
            {

            }
        }

    }

    class AccesPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Strenght { get; set; }
    }
}
