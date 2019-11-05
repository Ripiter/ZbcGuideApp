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
        uint ByteToInt(Stream file, int byteAmount)
        {
            for (int i = 0; i < byteAmount; i++)
                byteArray[i] = (byte)file.ReadByte();
            return BitConverter.ToUInt32(byteArray, 0);
        }

        // Goes forward in file by the amount given
        void GoForward(Stream file, int amount)
        {
            int none;
            for (int i = 0; i < amount; i++)
                none = file.ReadByte();
        }

        /// <summary>
        /// Used to import a 24-bitmap to a 2D array
        /// </summary>
        /// <param name="filename"></param>
        public void Import24Bitmap(Stream filename)
        {
            Stream inFile = filename;

            // Read fileformat
            bmpFormatName = ByteToInt(inFile, 2);

            if (bmpFormatName == 0x4D42) // Checks if bmp file is valid
            {
                // Skips unnecessary variables
                GoForward(inFile, 16);

                // Read width and height
                bmpWidth = ByteToInt(inFile, 4);
                bmpHeight = ByteToInt(inFile, 4);

                // Skips unnecessary variables
                GoForward(inFile, 2);

                // Read bits/px
                bmpBitsPx = ByteToInt(inFile, 2);

                // Checks if bmp file is 24 Bits
                if (bmpBitsPx == 24)
                {
                    // Skips unnecessary variables
                    GoForward(inFile, 24);

                    // Read RGB map array
                    bmpRGBMapArray = new uint[bmpWidth, bmpWidth];
                    for (int y = 0; y < bmpHeight; y++)
                        for (int x = 0; x < bmpWidth; x++)
                            bmpRGBMapArray[x, (-y + bmpHeight - 1)] = ByteToInt(inFile, 3);
                }
                else Debug.WriteLine("Error, only suports 24-bit bmp, your format is: " + bmpBitsPx + "-bit");
            }
            else Debug.WriteLine("Error, could't load image...");

            // File closes
            inFile.Close();
        }
    }
}

