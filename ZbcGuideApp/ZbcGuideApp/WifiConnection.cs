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
    public class WifiConnection
    {
        public event EventHandler PathFound;

        #region Public variables
        public static Context context = null;
        public static List<uint> xValues = null;
        public static List<uint> yValues = null;
        public static List<uint> dValues = null;

        public static int[] ourPosValues;
        #endregion

        #region Private variables
        //private static WifiManager wifi;
        private static int goingPosX = 0;
        private static int goingPosY = 0;
        PathFinding path = new PathFinding(ImportingMap.ImportBitmap);
        #endregion

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
        /// Gets somewhat accurate position of network in meters
        /// </summary>
        private double Distance(int signaldBm, double freqMhz)
        {
            double exp = (27.55 - (20 * Math.Log10(freqMhz)) + Math.Abs(signaldBm)) / 20;
            return Math.Pow(10, exp);
        }
        
        /// <summary>
        /// Generate map for all posible out comes
        /// </summary>
        public void GenerateMap()
        {
            //Generatepathmap called only when we changed where we want to go;
            path.GeneratePathMap((uint)goingPosX, (uint)goingPosY, false);

            PathWasFound();
        }

        /// <summary>
        /// Generates path based on our positin
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GeneratePath()
        {
            List<uint>[] list = path.GeneratePath(OurPosition.xPos, OurPosition.yPos);

            xValues = list[0];
            yValues = list[1];
            dValues = list[2];

        }

        public void PathWasFound()
        {
            PathFound(this, new EventArgs());
        }
     

    }


}
