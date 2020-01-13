using System;
using System.IO;


namespace ZbcGuideApp
{
    public struct BitmapRGB
    {
        public byte red, green, blue;
    }
    class BMP24Bit
    {
        // File structure
        public uint FormatName { get; set; } = 0; // 16-BIT
        public uint AlocatedFileSize { get; set; } = 0; // 32-BIT
        public uint Reserved01 { get; set; } = 0; // 16-BIT
        public uint Reserved02 { get; set; } = 0; // 16-BIT
        public uint OffsetOfImageData { get; set; } = 0; // 32-BIT
        public uint DIBHdrSize { get; set; } = 0; // 32-BIT
        public uint Width { get; set; } = 0; // 32-BIT
        public uint Height { get; set; } = 0; // 32-BIT
        public uint Planes { get; set; } = 0; // 16-BIT
        public uint BitsPx { get; set; } = 0; // 16-BIT
        public uint Compress { get; set; } = 0; // 32-BIT
        public uint ImgSize { get; set; } = 0; // 32-BIT
        public uint XResolution { get; set; } = 0; // 32-BIT
        public uint YResolution { get; set; } = 0; // 32-BIT
        public uint Colors { get; set; } = 0; // 32-BIT
        public uint ImportantColors { get; set; } = 0; // 32-BIT
        public uint[,] BitmapArray { get; set; }      // 24-BIT  
        public uint[] PaddingArray { get; set; }      // 0-BIT, 8-BIT, 16-BIT, 24-BIT

        /***********************************************************************************************************************/
        public BMP24Bit() { }

        /// <summary>
        /// Calls Import
        /// </summary>
        public BMP24Bit(Stream fileStream)
        {
            Import(fileStream);
        }

        /// <summary>
        /// Calls GenerateTemplate
        /// </summary>
        public BMP24Bit(uint width, uint height)
        {
            GenerateTemplate(width, height);
        }

        /***********************************************************************************************************************/
        /// <summary>
        /// Used to generate a template for bitmap with width, height and white pixels
        /// </summary>
        public void GenerateTemplate(uint width, uint height)
        {
            // Sets some default value for file structure
            FormatName = 0x4D42;
            AlocatedFileSize = (width * height) * 3;
            Reserved01 = 0;
            Reserved02 = 0;
            OffsetOfImageData = 54;
            DIBHdrSize = 40;
            Width = width;
            Height = height;
            Planes = 0;
            BitsPx = 24;
            Compress = 0;
            ImgSize = 0;
            XResolution = 0;
            YResolution = 0;
            Colors = 0;
            ImportantColors = 0;

            // Generate bRGBArray and bPaddingArray
            BitmapArray = new uint[Height, Width];
            PaddingArray = new uint[Height];

            // Sets white color to RGB- and padding array 
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    BitmapArray[y, x] = 0xFFFFFF;
                }
                PaddingArray[y] = 0;
            }
        }

        /***********************************************************************************************************************/
        /// <summary>
        ///  Used to set RGB pixel in bitmap 
        /// </summary>
        public void SetPixelRGB(uint x, uint y, uint red, uint green, uint blue)
        {
            BitmapArray[y, x] = (uint)(red << 16) | (uint)(green << 8) | (uint)(blue);
        }
        /// <summary>
        ///  Used to set RGB pixel in bitmap 
        /// </summary>
        public void SetPixelRGB(uint x, uint y, BitmapRGB rgb)
        {
            BitmapArray[y, x] = (uint)(rgb.red << 16) | (uint)(rgb.green << 8) | (uint)(rgb.blue);
        }
        /// <summary>
        ///  Used to get RGB pixel in bitmap 
        /// </summary>
        public BitmapRGB GetPixelRGB(uint x, uint y)
        {
            BitmapRGB rgb;
            uint hex = BitmapArray[y, x];

            rgb.red = (byte)(hex >> 16);
            rgb.green = (byte)(hex >> 8);
            rgb.blue = (byte)(hex >> 0);
            return rgb;
        }

        /***********************************************************************************************************************/
        /// <summary>
        /// Used to import 24-BIT bmp
        /// </summary>
        public string Import(Stream fileStream)
        {
            // Trys to open file
            Stream inFile;
            try
            {
                inFile = fileStream;
            }
            catch (Exception e)
            {
                return "Could't import bitmap...\n Exception: " + e.ToString();
            }

            // Read fileformat
            FormatName = ReadUInt(inFile, 2);
            if (FormatName == 0x4D42) // Checks if bmp file is valid
            {
                // Read filesize
                AlocatedFileSize = ReadUInt(inFile, 4);

                // Read reserved chunks
                Reserved01 = ReadUInt(inFile, 2);
                Reserved02 = ReadUInt(inFile, 2);

                // Read offset of image data
                OffsetOfImageData = ReadUInt(inFile, 4);

                // Read DIB HDR size
                DIBHdrSize = ReadUInt(inFile, 4);

                // Read width and height
                Width = ReadUInt(inFile, 4);
                Height = ReadUInt(inFile, 4);

                // Read planes
                Planes = ReadUInt(inFile, 2);

                // Read bits/px
                BitsPx = ReadUInt(inFile, 2);

                if (BitsPx == 24) // Checks if bmp file is 24 Bits
                {
                    // Read compress
                    Compress = ReadUInt(inFile, 4);

                    // Read img size
                    ImgSize = ReadUInt(inFile, 4);

                    // Read x and y resolution
                    XResolution = ReadUInt(inFile, 4);
                    YResolution = ReadUInt(inFile, 4);

                    // Read colors and important colors
                    Colors = ReadUInt(inFile, 4);
                    ImportantColors = ReadUInt(inFile, 4);

                    // Generate bRGBArray and bPaddingArray
                    BitmapArray = new uint[Height, Width];
                    PaddingArray = new uint[Height];

                    // Read RGB- and padding array 
                    byte paddingOffset = (byte)(Width % 4);
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            // The RGB array must be read upsidedown to work properly
                            BitmapArray[(-y + Height - 1), x] = ReadUInt(inFile, 3);
                        }
                        if (paddingOffset != 0)
                            PaddingArray[(-y + Height - 1)] = ReadUInt(inFile, paddingOffset);
                    }
                }
                else return "Error, only suports 24-bit bmp, your format is: " + BitsPx + "-bit";
            }
            else return "Error, could't load image...";

            // File closes
            inFile.Close();
            return "Successful!";
        }

        // Used to convert the given amount of bytes to uint
        private byte[] byteArray = new byte[4];
        private uint ReadUInt(Stream file, byte byteAmount)
        {
            for (int i = 0; i < byteAmount; i++)
                byteArray[i] = (byte)file.ReadByte();
            return BitConverter.ToUInt32(byteArray, 0);
        }

        /***********************************************************************************************************************/
        /// <summary>
        /// Used to export 24-BIT bmp from the current class properties
        /// </summary>
        public string Export(Stream fileStream)
        {
            // Trys to generate file and open file
            Stream outFile;
            try
            {
                outFile = fileStream;
            }
            catch (Exception e)
            {
                return "Could't export bitmap...\n Exception: " + e.ToString();
            }

            // Write fileformat
            WriteUInt(outFile, FormatName, 2);

            // Write filesize
            WriteUInt(outFile, AlocatedFileSize, 4);

            // Write reserved chunks
            WriteUInt(outFile, Reserved01, 2);
            WriteUInt(outFile, Reserved02, 2);

            // Write offset of image data
            WriteUInt(outFile, OffsetOfImageData, 4);

            // Write DIB HDR size
            WriteUInt(outFile, DIBHdrSize, 4);

            // Write width and height
            WriteUInt(outFile, Width, 4);
            WriteUInt(outFile, Height, 4);

            // Write planes
            WriteUInt(outFile, Planes, 2);

            // Write bits/px
            WriteUInt(outFile, BitsPx, 2);

            // Write compress
            WriteUInt(outFile, Compress, 4);

            // Write img size
            WriteUInt(outFile, ImgSize, 4);

            // Write x and y resolution
            WriteUInt(outFile, XResolution, 4);
            WriteUInt(outFile, YResolution, 4);

            // Write colors and important colors
            WriteUInt(outFile, Colors, 4);
            WriteUInt(outFile, ImportantColors, 4);

            // Write RGB- and padding array
            byte paddingOffset = (byte)(Width % 4);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // The RGB array must be written upsidedown to work properly
                    WriteUInt(outFile, BitmapArray[(-y + Height - 1), x], 3);
                }
                if (paddingOffset != 0)
                    WriteUInt(outFile, PaddingArray[(-y + Height - 1)], paddingOffset);
            }
            outFile.Close();
            return "Successful!";
        }

        // Used to converts uint to bytes and write it to file
        private void WriteUInt(Stream file, uint value, byte byteAmount)
        {
            value = (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                    (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;

            byte[] byteArray = BitConverter.GetBytes(value);

            for (int i = 3; i >= 4 - byteAmount; i--)
                file.WriteByte(byteArray[i]);
        }
    }
}
