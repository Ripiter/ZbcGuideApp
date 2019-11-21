using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace ZbcGuideApp
{
    class ImportingMap
    {
        static ImportBitmap importBitmap = null;
        

        public static uint[,] ImpArray
        {
            get
            {
                return importBitmap.BMPMapArray;
            }
        }

        public static  int ImgHeight
        {
            get
            {
                return (int)importBitmap.BMPHeight;
            }
        }

        public static int ImgWidth
        {
            get
            {
                return (int)importBitmap.BMPWidth;
            }
        }


        /// <summary>
        /// Loads map, values will be used to pathfind
        /// </summary>
        public static void StartLoading()
        {
            if (importBitmap == null)
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(WifiConnection)).Assembly;
                Stream stream = assembly.GetManifestResourceStream("ZbcGuideApp.mapOfRoskilde.bmp");
                importBitmap = new ImportBitmap();
                importBitmap.Import24Bitmap(stream);
            }
        }
    }
}
