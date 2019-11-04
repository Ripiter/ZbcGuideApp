using Android.Content;
using Android.Net.Wifi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZbcGuideApp
{
    public class WifiConnection :  BroadcastReceiver
    {
        public static Context context = null;
        public static WifiManager wifi;
        //private WifiReceiver wifiReceiver;
        public static bool searching = false;

        

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

        #region test

        public void Test()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            PathFinding pathFinding = new PathFinding();
            ImportBitmap importBitmap = new ImportBitmap();
            importBitmap.Import24Bitmap(Android.App.Application.Context.Assets.Open("potato.bmp"));
            pathFinding.GenerateNewMap((int)importBitmap.BMPHeight, (int)importBitmap.BMPHeight, 0, 0xff, importBitmap.BMPMapArray);
            pathFinding.GeneratePath(90, 70, 430, 430, false);
            pathFinding.DrawMap();
            //xvalues = pathFinding.xValues;
            //yvalues = pathFinding.yValues;

           
            Debug.WriteLine(stopwatch.Elapsed);
            stopwatch.Stop();
        }
        #endregion


    
        string printInfo = string.Empty;
        string jsonString = string.Empty;
        IList<ScanResult> scanwifinetworks = null;
        List<AccessPoint> accessPoints = new List<AccessPoint>();
        List<AccesPoint> testData = new List<AccesPoint>();

        //public WifiReceiver()
        //{
        //    ReadingJson();
        //    Debug.WriteLine("Json Called");
        //}
        public override void OnReceive(Context context, Intent intent)
        {
            scanwifinetworks = WifiConnection.wifi.ScanResults;
            foreach (ScanResult wifinetwork in scanwifinetworks)
            {
                Debug.WriteLine(wifinetwork.Bssid + wifinetwork.Ssid);
                //printInfo = "Mac address " + wifinetwork.Bssid + " with name " + wifinetwork.Ssid + " with lvl dBm " + wifinetwork.Level;
                //printInfo = wifinetwork.Ssid + " with lvl dBm " + wifinetwork.Level;
                printInfo = "X Y cords " + SerializeJson(wifinetwork.Bssid) + " of " + wifinetwork.Bssid;
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
            //List<AccessPoint> sortedList = scanResults.OrderByDescending(o => o.Strenght).ToList();

            // Use this code
            //XyCords(scanResults[0].Strenght, scanResults[1].Strenght, scanResults[2].Strenght);
            
            // testing 
            XyCords(0,0,0);

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

        
        private void XyCords(int s1, int s2, int s3)
        {
            //double px = ((s1 * s1) - (s2 * s2) + (testData[1].X * testData[1].X)) / ((double)(2 * testData[1].X));

            //double py = ((s1 * s1) - (s3 * s3) + (testData[2].X * testData[2].X) + (testData[2].Y * testData[2].Y)) / (2 * testData[2].Y) - (testData[2].X / (double)testData[2].Y) * px;

            //px = px * 2.01;
            //py = py * 1.79;

            //int x = (int)px;
            //int y = (int)py;

            //Debug.WriteLine(x);
            //Debug.WriteLine(y);

            PathFinding pathFinding = new PathFinding();
            ImportBitmap importBitmap = new ImportBitmap();
            importBitmap.Import24Bitmap(Android.App.Application.Context.Assets.Open("pathing.bmp"));
            pathFinding.GenerateNewMap((int)importBitmap.BMPHeight, (int)importBitmap.BMPHeight, 0, 0xff, importBitmap.BMPMapArray);
            //pathFinding.GeneratePath(x, y, 175, 475, false);
            pathFinding.GeneratePath(420, 480, 175, 475, false);
            pathFinding.DrawMap();

            //Metodenavn = returnmetode

            xValues = pathFinding.xValues;
            yValues = pathFinding.yValues;

            //Debug.WriteLine("Our Location (calc): X:" + Math.Round(px) + " Y: " + Math.Round(py));
            PathWasFound();
        }
        public List<int> xValues = new List<int>();
        public List<int> yValues = new List<int>();
        public void PathWasFound()
        {
            PathFound(this, new EventArgs());
        }

        public event EventHandler PathFound;

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

        private string SerializeJson(string mac)
        {
            try
            {
                JObject json = JObject.Parse(jsonString);
                JToken jToken = json[mac];

                int x = (int)jToken["x"];
                int y = (int)jToken["y"];
                Debug.WriteLine($"x {x} y {y} of {mac}");
                testData.Add(new AccesPoint { X = x, Y = y });
                return (string)jToken["x"] + (string)jToken["y"];
            }
            catch
            {

            }
            return "";
        }

    }

    class AccesPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
