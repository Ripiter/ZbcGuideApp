using Android.Content;
using Android.Locations;
using Android.Net.Wifi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using Android.Content.Res;

namespace ZbcGuideApp
{
    public class WifiConnection
    {
        public static Context context = null;
        private static WifiManager wifi;
        private WifiReceiver wifiReceiver;
        public static ObservableCollection<AccessPoint> oc;
        public static bool searching = false;

        public WifiConnection()
        {
            oc = new ObservableCollection<AccessPoint>();
        }

        public void GetWifiNetworks()
        {
            searching = true;
            // Get a handle to the Wifi
            wifi = (WifiManager)context.GetSystemService(Context.WifiService);

            // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
            if (wifiReceiver == null)
                wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            try
            {
                Debug.WriteLine(wifi.StartScan());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        class WifiReceiver : BroadcastReceiver
        {
            string printInfo = string.Empty;
            IList<ScanResult> scanwifinetworks = null;
            List<AccessPoint> accessPoints = new List<AccessPoint>();

            public override void OnReceive(Context context, Intent intent)
            {
                scanwifinetworks = wifi.ScanResults;
                foreach (ScanResult wifinetwork in scanwifinetworks)
                {
                    //printInfo = "Mac address " + wifinetwork.Bssid + " with name " + wifinetwork.Ssid + " with lvl dBm " + wifinetwork.Level;
                    //printInfo = wifinetwork.Ssid + " with lvl dBm " + wifinetwork.Level;
                    printInfo = "Network " +wifinetwork.Ssid +" is "+ Distance(wifinetwork.Level, wifinetwork.Frequency).ToString("n") + "m away";
                    
                    //oc.Add(new AccessPoint() { Mac=wifinetwork.Bssid, Ssid = wifinetwork.Ssid, Strenght = wifinetwork.Level, PrintInfo = printInfo });
                    accessPoints.Add(new AccessPoint() { Mac = wifinetwork.Bssid, Ssid = wifinetwork.Ssid, Strenght = wifinetwork.Level, PrintInfo = printInfo });
                    Distance(wifinetwork.Level, wifinetwork.Frequency);
                    Debug.WriteLine(printInfo);
                }
                TopResults(accessPoints);
                wifi.ScanResults.Clear();
                scanwifinetworks.Clear();
                accessPoints.Clear();
            }

            /// <summary>
            /// Shows top 3 access point based on power dBm
            /// </summary>
            /// <param name="scanResults"></param>
            private void TopResults(List<AccessPoint> scanResults)
            {
                int amountToShow = 4;

                List<AccessPoint> sortedList = scanResults.OrderByDescending(o => o.Strenght).ToList();

                if (sortedList.Count >= amountToShow)
                    for (int i = 0; i < amountToShow; i++)
                        oc.Add(sortedList[i]);
                else
                    for (int i = 0; i < sortedList.Count; i++)
                        oc.Add(sortedList[i]);

                WifiConnection.searching = false;
                sortedList.Clear();
            }
            /// <summary>
            /// Gets somewhat accurate position of network in meters
            /// </summary>
            /// <param name="signaldBm"></param>
            /// <param name="freqMhz"></param>
            /// <returns></returns>
            private double Distance(int signaldBm, double freqMhz)
            {
                double exp = (27.55 - (20 * Math.Log10(freqMhz)) + Math.Abs(signaldBm)) / 20;
                return Math.Pow(10, exp);
            }
            enum Colors
            {
                AccessPointBlack = 0x0000ff,
                UserRed = 0xff0000,
            };






            List<AccesPoint> testData = new List<AccesPoint>();

            private void Start()
            {
                Cords a = new Cords();

                //string path = @"./Resources/drawable/Test.bmp";
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(WifiConnection)).Assembly;

                string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                var path1 = "/storage/emulated/0/files";
                //var mp3Files = Directory.EnumerateFiles(path1, "*.png", SearchOption.AllDirectories);
                Stream stream =  Android.App.Application.Context.Assets.Open("potato.bmp");


                // Debug.WriteLine(_fileName);
                ////string path = "Unavngivet.png";
                a.Import24Bitmap(stream);


                //a.DrawBitmapBlackWhite();
                for (int y = 0; y < a.BMPHeight; y++)
                {
                    for (int x = 0; x < a.BMPWidth; x++)
                    {
                        if (a.BMPMapArray[x, y] == (uint)Colors.AccessPointBlack)
                        {
                            testData.Add(new AccesPoint { X = x, Y = y });
                            Debug.WriteLine("X: " + x + " Y: " + y);
                        }

                        if (a.BMPMapArray[x, y] == (uint)Colors.UserRed)
                        {
                            testData.Add(new AccesPoint { X = x, Y = y });
                            Debug.WriteLine("Our location (hardcode): X: " + x + " Y: " + y);
                        }
                    }
                }
                XyCords(-45, -56, -59);
            }


            private void XyCords(int s1, int s2, int s3)
            {
                var px = ((s1 * s1) - (s2 * s2) + (testData[1].X * testData[1].X)) / ((double)(2 * testData[1].X));

                var py = ((s1 * s1) - (s3 * s3) + (testData[2].X * testData[2].X) + (testData[2].Y * testData[2].Y)) / (2 * testData[2].Y) - (testData[2].X / (double)testData[2].Y) * px;

                px = px * 2.01;
                py = py * 1.79;


                Debug.WriteLine("Out Location (calc): X:" + Math.Round(px) + " Y: " + Math.Round(py));
            }

        }
    }

    class AccesPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

}
