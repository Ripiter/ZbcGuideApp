using Android.Content;
using Android.Net.Wifi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace ZbcGuideApp
{
    public class test
    {
        public static Context context = null;
        private static WifiManager wifi;
        private WifiReceiver wifiReceiver;
        //public List<AccessPoint> WiFiNetworks;

        public static ObservableCollection<AccessPoint> oc;

        public test()
        {
            oc = new ObservableCollection<AccessPoint>();
            
            //GetWifiNetworks();
        }

        public void GetWifiNetworks()
        {
            // WiFiNetworks = new List<AccessPoint>();

            // Get a handle to the Wifi
            wifi = (WifiManager)context.GetSystemService(Context.WifiService);

            // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
            if (wifiReceiver == null)
                wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            try
            {
                Debug.WriteLine("seding in test");
                Debug.WriteLine( wifi.StartScan());
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
            public override void OnReceive(Context context, Intent intent)
            {
                Debug.WriteLine("Receve");

                //oc.Clear();
                scanwifinetworks = wifi.ScanResults;
                foreach (ScanResult wifinetwork in scanwifinetworks)
                {
                    printInfo = wifinetwork.Ssid + " with lvl dBm " + wifinetwork.Level;

                    oc.Add(new AccessPoint() { Ssid = wifinetwork.Ssid, Strenght = wifinetwork.Level, PrintInfo = printInfo });

                    Debug.WriteLine(printInfo);
                }
                wifi.ScanResults.Clear();
                scanwifinetworks.Clear();
            }
        }
    }
}
