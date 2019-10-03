using Android.Content;
using Android.Locations;
using Android.Net.Wifi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace ZbcGuideApp
{
    public class WifiConnection
    {
        public static Context context = null;
        private static WifiManager wifi;
        private WifiReceiver wifiReceiver;
        public static ObservableCollection<AccessPoint> oc;

        public WifiConnection()
        {
            oc = new ObservableCollection<AccessPoint>();
        }

        public void GetWifiNetworks()
        {
            // Get a handle to the Wifi
            wifi = (WifiManager)context.GetSystemService(Context.WifiService);

            // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
            if (wifiReceiver == null)
                wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            try
            {
                Debug.WriteLine("seding in test");
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
                    printInfo = wifinetwork.Ssid + " with lvl dBm " + wifinetwork.Level;

                    //oc.Add(new AccessPoint() { Mac=wifinetwork.Bssid, Ssid = wifinetwork.Ssid, Strenght = wifinetwork.Level, PrintInfo = printInfo });
                    accessPoints.Add(new AccessPoint() { Mac = wifinetwork.Bssid, Ssid = wifinetwork.Ssid, Strenght = wifinetwork.Level, PrintInfo = printInfo });

                    Debug.WriteLine(printInfo);
                }

                Top3(accessPoints);
                wifi.ScanResults.Clear();
                scanwifinetworks.Clear();
                accessPoints.Clear();
            }

            /// <summary>
            /// Shows top 3 access point based on power dBm
            /// </summary>
            /// <param name="scanResults"></param>
            private void Top3(List<AccessPoint> scanResults)
            {
                List<AccessPoint> sortedList = scanResults.OrderByDescending(o => o.Strenght).ToList();

                if (sortedList.Count >= 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        oc.Add(sortedList[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < sortedList.Count; i++)
                    {
                        oc.Add(sortedList[i]);
                    }
                }
                sortedList.Clear();
            }
        }
    }
}
