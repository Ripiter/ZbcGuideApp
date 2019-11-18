using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZbcGuideApp
{
    class ImportBitmap
    {
        // Variables
        uint bmpFormatName;                       // 16-BIT            
        uint bmpAlocatedFileSize;			      // 32-BIT
        uint bmpReserved01, bmpReserved02;        // 16-BIT
        uint bmpOffsetOfImageData;		          // 32-BIT
        uint bmpDIBHdrSize;				          // 32-BIT
        uint bmpWidth, bmpHeight;			      // 32-BIT
        uint bmpPlanes;					          // 16-BIT
        uint bmpBitsPx;					          // 16-BIT
        uint bmpCompress;					      // 32-BIT
        uint bmpImgSize;					      // 32-BIT
        uint bmpXResolution, bmpYResolution;      // 32-BIT
        uint bmpColors, bmpImportantColors;       // 32-BIT
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

        uint ByteToInt(Stream file, int amountOfByte)
        {
            uint[] readAmountOfByte = new uint[amountOfByte];
            uint[] readAmountOfByteReverse = new uint[amountOfByte];
            uint result = 0;

            for (int i = 0; i < amountOfByte; i++)
                readAmountOfByte[i] = (uint)file.ReadByte();

            for (int i = 0, j = amountOfByte - 1; (i < amountOfByte) && (j > -1); i++, j--)
            {
                readAmountOfByteReverse[i] = readAmountOfByte[j];
                switch (i)
                {
                    case 0: break;
                    case 1: result = readAmountOfByteReverse[0] * 0xFF + readAmountOfByteReverse[0] + readAmountOfByteReverse[1]; break;
                    case 2: result = result * 0xFF + result + readAmountOfByteReverse[2]; break;
                    default: result = result * 0xFF + result + readAmountOfByteReverse[3]; break;
                }
            }
            return result;
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
                // Read filesize
                bmpAlocatedFileSize = ByteToInt(inFile, 4);

                // Read reserved chunks
                bmpReserved01 = ByteToInt(inFile, 2);
                bmpReserved02 = ByteToInt(inFile, 2);

                // Read offset of image data
                bmpOffsetOfImageData = ByteToInt(inFile, 4);

                // Read DIB HDR size
                bmpDIBHdrSize = ByteToInt(inFile, 4);

                // Read width and height
                bmpWidth = ByteToInt(inFile, 4);
                bmpHeight = ByteToInt(inFile, 4);

                // Read planes
                bmpPlanes = ByteToInt(inFile, 2);

                // Read bits/px
                bmpBitsPx = ByteToInt(inFile, 2);

                if (bmpBitsPx == 24) // Checks if bmp file is 24 Bits
                {
                    // Read compress
                    bmpCompress = ByteToInt(inFile, 4);

                    // Read img size
                    bmpImgSize = ByteToInt(inFile, 4);

                    // Read x and y resolution
                    bmpXResolution = ByteToInt(inFile, 4);
                    bmpYResolution = ByteToInt(inFile, 4);

                    // Read colors and important colors
                    bmpColors = ByteToInt(inFile, 4);
                    bmpImportantColors = ByteToInt(inFile, 4);

                    // Read RGB map array
                    bmpRGBMapArray = new uint[bmpWidth, bmpWidth];
                    for (int y = 0; y < bmpHeight; y++)
                    {
                        for (int x = 0; x < bmpWidth; x++)
                        {
                            bmpRGBMapArray[(-y + bmpHeight - 1), x] = ByteToInt(inFile, 3);
                        }
                    }
                }
                else Console.WriteLine("Error, only suports 24-bit bmp, your format is: " + bmpBitsPx + "-bit");
            }
            else Console.WriteLine("Error, could't load image...");
            inFile.Close();
        }

        /// <summary>
        /// Used to write all bitmap data to the console
        /// </summary>
        /// <param name="filename"></param>
        public void ShowBitmapInfo()
        {
            Console.WriteLine("Alocated filesize: " + bmpAlocatedFileSize + " BYTES");
            Console.WriteLine("Alocated filesize: " + bmpAlocatedFileSize / 1024 + " KB");
            Console.WriteLine("Alocated filesize: " + (bmpAlocatedFileSize / 1024) / 1024 + " MB");
            Console.WriteLine("\nReserved01: " + bmpReserved01);
            Console.WriteLine("Reserved02: " + bmpReserved02);
            Console.WriteLine("\nOffset of img data: " + bmpOffsetOfImageData);
            Console.WriteLine("\nDIB HDR size " + bmpDIBHdrSize);
            Console.WriteLine("\nWidth: " + bmpWidth);
            Console.WriteLine("Height: " + bmpHeight);
            Console.WriteLine("\nPlanes: " + bmpPlanes);
            Console.WriteLine("Bits/px: " + bmpBitsPx + "-bit");
            Console.WriteLine("\nCompress: " + bmpCompress);
            Console.WriteLine("Image size: " + bmpImgSize);
            Console.WriteLine("\nX-resolution: " + bmpXResolution);
            Console.WriteLine("Y-resolution: " + bmpYResolution);
            Console.WriteLine("\nColors: " + bmpColors);
            Console.WriteLine("Important colors: " + bmpImportantColors);
        }

        /// <summary>
        /// Used to draw the bitmap as hex values to the console 
        /// </summary>
        /// <param name="filename"></param>
        public void DrawBitmap()
        {
            for (int y = 0; y < bmpHeight; y++)
            {
                for (int x = 0; x < bmpWidth; x++)
                {
                    Console.Write("0x");
                    if (bmpRGBMapArray[x, y] < 0x000010) Console.Write("00000");
                    else if (bmpRGBMapArray[x, y] < 0x000100) Console.Write("0000");
                    else if (bmpRGBMapArray[x, y] < 0x001000) Console.Write("000");
                    else if (bmpRGBMapArray[x, y] < 0x010000) Console.Write("00");
                    else if (bmpRGBMapArray[x, y] < 0x100000) Console.Write("0");
                    Console.Write("{0:X}", bmpRGBMapArray[x, y]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Used to draw the bitmap only with black and white
        /// </summary>
        /// <param name="filename"></param>
        //// TODO: Add more colors
        public void DrawBitmapBlackWhite()
        {
            for (int y = 0; y < bmpHeight; y++)
            {
                for (int x = 0; x < bmpWidth; x++)
                {
                    if (bmpRGBMapArray[x, y] == 0xFFFFFF)
                        Console.Write(" ");
                    else
                        Console.Write("x");
                }
                Console.WriteLine();
            }
        }
    }
}
