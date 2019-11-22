using Android.Content;
using Android.Net.Wifi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZbcGuideApp
{
    public class WifiConnection : BroadcastReceiver
    {
        #region Events
        public event EventHandler PathFound;
        public event EventHandler ErrorLoading;
        #endregion

        #region Public variables
        public static Context context = null;
        public static bool searching = false;
        public static int[] xValues = null;
        public static int[] yValues = null;
        public static int[] dValues = null;
        #endregion

        #region Private variables
        string jsonString = string.Empty;
        List<UnKnownAccessPoint> accessPoints = new List<UnKnownAccessPoint>();
        List<KnownAccessPoint> testData = new List<KnownAccessPoint>();
        PathFinding pathFinding = new PathFinding();
        private static WifiManager wifi;
        private static int goingPosX = 0;
        private static int goingPosY = 0;
        #endregion

        public void GetWifiNetworks()
        {
            searching = true;
            wifi = (WifiManager)context.GetSystemService(Context.WifiService);

            context.RegisterReceiver(this, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            try
            {
                // If cannot start scan trow a error
                if (wifi.StartScan() == false)
                {
                    ErrorLoading(this, new EventArgs());
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// After wifi.StartScan() have ended this method will be run
        /// it contains all wifi networks found in this area
        /// including 2.4 ghz, 5 ghz and hidden networks
        /// </summary>
        public override void OnReceive(Context context, Intent intent)
        {
            IList<ScanResult> scanwifinetworks = wifi.ScanResults;
            foreach (ScanResult wifinetwork in scanwifinetworks)
            {
                Debug.WriteLine(wifinetwork.Bssid + " " + wifinetwork.Ssid);
                accessPoints.Add(new UnKnownAccessPoint { Mac = wifinetwork.Bssid, Ssid = wifinetwork.Ssid, Strenght = wifinetwork.Level });
            }
            Results(accessPoints);
            wifi.ScanResults.Clear();
            scanwifinetworks.Clear();
            accessPoints.Clear();
        }

        /// <summary>
        /// Sets x and y position according to
        /// item selected in dropdown menu
        /// </summary>
        /// <param name="name"></param>
        public void SetGoPos(string name)
        {
            switch (name.ToLower())
            {
                case "ubuy":
                    goingPosX = 175;
                    goingPosY = 454;
                    break;
                case "food hall":
                    goingPosX = 181;
                    goingPosY = 359;
                    break;
                case "foredragssalen":
                    goingPosX = 181;
                    goingPosY = 214;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reads json file and put it in a string while 
        /// adding known mac-address to a list 
        /// </summary>
        /// <param name="scanResults"></param>
        private void Results(List<UnKnownAccessPoint> scanResults)
        {
            try
            {
                ReadingJson();
                foreach (UnKnownAccessPoint item in scanResults)
                {
                    SerializeJson(item.Mac, item.Strenght);
                }

                XyCords(testData[0].Strenght, testData[1].Strenght, testData[2].Strenght);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                GetWifiNetworks();

                if (ErrorLoading != null)
                    ErrorLoading(this, new EventArgs());
            }

            searching = false;
        }


        /// <summary>
        /// Gets somewhat accurate position of network in meters
        /// </summary>
        private double Distance(int signaldBm, double freqMhz)
        {
            double exp = (27.55 - (20 * Math.Log10(freqMhz)) + Math.Abs(signaldBm)) / 20;
            return Math.Pow(10, exp);
        }

        /// <summary>
        /// Gets strenght of 3 closest accesspoints and calculates your position
        /// based on strenght and position of the known mac-addresses
        /// </summary>
        private void XyCords(int s1, int s2, int s3)
        {
            int x = 0;
            int y = 0;

            double px = ((s1 * s1) - (s2 * s2) + (testData[1].X * testData[1].X)) / ((double)(2 * testData[1].X));

            double py = ((s1 * s1) - (s3 * s3) + (testData[2].X * testData[2].X) + (testData[2].Y * testData[2].Y)) / (2 * testData[2].Y) - (testData[2].X / (double)testData[2].Y) * px;

            px = px * 2.05;
            py = py * 2.09;

            x = (int)px;
            y = (int)py;

            GeneratePath(x, y);
        }

        /// <summary>
        /// Generates path according to the place user selected
        /// and position that was triangulated, also
        /// sets x, y and direction values.
        /// <para>Triggers event that path was found</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void GeneratePath(int x, int y)
        {
            pathFinding.GenerateNewMap(ImportingMap.ImgHeight, ImportingMap.ImgWidth, 0, 0xff, ImportingMap.ImpArray);

            pathFinding.GeneratePath(x, y, goingPosX, goingPosY, false);

            xValues = pathFinding.XPath;
            yValues = pathFinding.YPath;
            dValues = pathFinding.DirectionPath;

            PathWasFound();
            testData.Clear();
        }

        public void PathWasFound()
        {
            PathFound(this, new EventArgs());
        }
        
        /// <summary>
        /// Reads json file from assests folder and sets 
        /// it in a string variable
        /// </summary>
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
                    // Converts and adds to a string.
                    s += encoding.GetString(temp, 0, len);
                }
            }
            jsonString = s;
        }

        /// <summary>
        /// Checks if the mac address is known in the json file
        /// if its known add's it to a list
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="strenght"></param>
        private void SerializeJson(string mac, int strenght)
        {
            try
            {
                JObject json = JObject.Parse(jsonString);
                JToken jToken = json[mac];

                int x = (int)jToken["x"];
                int y = (int)jToken["y"];
                Debug.WriteLine($"x {x} y {y} of {mac}");
                testData.Add(new KnownAccessPoint { X = x, Y = y, Strenght = strenght });
            }
            catch
            {

            }
        }

    }


}
