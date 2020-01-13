using System.IO;
using System.Reflection;

namespace ZbcGuideApp
{
    class ImportingMap
    {
        private static BMP24Bit importBitmap = null;


        public static BMP24Bit ImportBitmap
        {
            get
            {
                StartLoading();
                return importBitmap;
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
                importBitmap = new BMP24Bit(stream);

            }
        }
    }
}
