using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
namespace ZbcGuideApp
{
    class ImportBitmap
    {
        // Static array used to store bytes
        static byte[] byteArray = new byte[4];

        // Variables
        uint bmpFormatName;                       // 16-BIT            
        uint bmpWidth, bmpHeight;			      // 32-BIT
        uint bmpBitsPx;					          // 16-BIT
        uint[,] bmpRGBMapArray;                   // 24-BIT

        // Property
        #region property
        public uint BMPWidth
        {
            get { return bmpWidth; }
            set { bmpWidth = value; }
        }
        public uint BMPHeight
        {
            get { return bmpHeight; }
            set { bmpHeight = value; }
        }
        public uint[,] BMPMapArray
        {
            get { return bmpRGBMapArray; }
            set { bmpRGBMapArray = value; }
        }
        #endregion

        // Converts the given amount of bytes to uint
        uint ByteToInt( int byteAmount)
        {
            for (int i = 0; i < byteAmount; i++)
                byteArray[i] = (byte)inFile.ReadByte();
            return BitConverter.ToUInt32(byteArray, 0);
        }

        int none;
        // Goes forward in file by the amount given
        void GoForward(int amount)
        {
            for (int i = 0; i < amount; i++)
                none = inFile.ReadByte();
        }
        Stream inFile;
        /// <summary>
        /// Used to import a 24-bitmap to a 2D array
        /// </summary>
        /// <param name="filename"></param>
        public void Import24Bitmap(Stream filename)
        {
            inFile = filename;
            // Read fileformat
            bmpFormatName = ByteToInt(2);

            if (bmpFormatName == 0x4D42) // Checks if bmp file is valid
            {
                // Skips unnecessary variables
                GoForward(16);

                // Read width and height
                bmpWidth = ByteToInt(4);
                bmpHeight = ByteToInt(4);

                // Skips unnecessary variables
                GoForward(2);

                // Read bits/px
                bmpBitsPx = ByteToInt(2);

                // Checks if bmp file is 24 Bits
                if (bmpBitsPx == 24)
                {
                    // Skips unnecessary variables
                    GoForward(24);

                    // Read RGB map array
                    bmpRGBMapArray = new uint[bmpWidth, bmpWidth];
                    for (int y = 0; y < bmpHeight; y++)
                        for (int x = 0; x < bmpWidth; x++)
                            bmpRGBMapArray[x, (-y + bmpHeight - 1)] = ByteToInt(3);
                }
                else Debug.WriteLine("Error, only suports 24-bit bmp, your format is: " + bmpBitsPx + "-bit");
            }
            else Debug.WriteLine("Error, could't load image...");

            // File closes
            inFile.Close();
        }
    }
}

