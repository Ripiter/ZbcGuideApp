using System.Collections.Generic;
using System.IO;
namespace ZbcGuideApp
{

    enum PathImage
    {
        ColorImage = 0,
        WaveImage = 1,
    };
    enum PathDirection
    {
        Up = 1,
        TopRightCorner = 2,
        Right = 3,
        LowerRightCorner = 4,
        Down = 5,
        LowerLeftCorner = 6,
        Left = 7,
        TopLeftCorner = 8,
    };
    class PathFinding
    {
        // Path map
        private uint mWidth, mHeight;
        private uint[,] mColorArray, mDistanceArray;
        private BMP24Bit mBitmap;

        // Path calucation
        private uint pCurrentDistance;
        private bool pStrafe;

        // Path coordinate memory
        private List<uint> xMemory = new List<uint>();
        private List<uint> yMemory = new List<uint>();
        private List<uint> xNewMemory = new List<uint>();
        private List<uint> yNewMemory = new List<uint>();

        // Path distance definitions
        private const uint SOLID = 0xFFFFFFFF;
        private const uint ENDPOS = 0xFFFFFFFD;
        private const uint NEW_CHUNK = 0xFFFFFFFC;
        private const uint CHUNK_IN_QUEUE = 0xFFFFFFFB;
        private const uint UNSET = 0x00000000;

        // Path list definitions
        private const uint LIST_XCOORD = 0;
        private const uint LIST_YCOORD = 1;
        private const uint LIST_DIRECTION = 2;

        // Path color properties
        public uint WallColor { get; set; } = 0x000000; // Default black
        public uint PathColor { get; set; } = 0xFF00FF; // Default mangeta
        public uint EndColor { get; set; } = 0xFF0000; // Default Red
        public uint StartColor { get; set; } = 0x00FF00; // Default Green

        // Path map properties
        public uint Width { get { return mWidth; } }
        public uint Height { get { return mHeight; } }
        public uint[,] ColorArray { get { return mColorArray; } }
        public uint[,] DistanceArray { get { return mDistanceArray; } }

        // Path properties
        public bool ListCoordinates { get; set; } = true;

        /***********************************************************************************************************************/
        public PathFinding() { }

        /// <summary>
        /// Calls SetMap with bitmap
        /// </summary>
        public PathFinding(BMP24Bit bitmap)
        {
            SetMap(bitmap);
        }
        /// <summary>
        /// Calls SetMap with uintArray
        /// </summary>
        public PathFinding(uint[,] uintArray, uint width, uint height)
        {
            SetMap(uintArray, width, height);
        }

        /***********************************************************************************************************************/
        /// <summary>
        /// Used to set a bmp image for the path map
        /// </summary>
        public void SetMap(BMP24Bit bitmap)
        {
            mWidth = (uint)bitmap.Width;
            mHeight = (uint)bitmap.Height;
            mColorArray = new uint[mHeight, mWidth];
            mDistanceArray = new uint[mHeight, mWidth];
            mBitmap = bitmap;
        }
        /// <summary>
        /// Used to set a uint array for the path map
        /// </summary>
        public void SetMap(uint[,] uintArray, uint width, uint height)
        {
            mWidth = width;
            mHeight = height;
            mColorArray = new uint[mHeight, mWidth];
            mDistanceArray = new uint[mHeight, mWidth];
            mBitmap.BitmapArray = uintArray;
        }

        /// <summary>
        /// Used to save path to bitmap
        /// </summary>
        public void SavePathToBitmap(Stream filename, PathImage pathImage)
        {
            // Genrates bitmap
            BMP24Bit bitmap = new BMP24Bit(mWidth, mHeight);

            // Sets bitmap data to the specified path image
            switch (pathImage)
            {
                case PathImage.ColorImage:
                    bitmap.BitmapArray = mColorArray;
                    break;
                case PathImage.WaveImage:
                    bitmap.BitmapArray = mDistanceArray;
                    break;
            }

            // Saves bitmap
            bitmap.Export(filename);
        }

        /***********************************************************************************************************************/
        /// <summary>
        /// Used to generate the path map from the end position,
        /// <para>Returns string if path map generation was successful or failed</para>
        /// </summary>
        public string GeneratePathMap(uint xEnd, uint yEnd, bool pathStrafe)
        {
            // Reset old pathmap
            ResetPathMap();

            // Set strafe mode
            pStrafe = pathStrafe;

            // Check if out of bounds
            if ((xEnd >= mWidth) || (yEnd >= mHeight))
                return "Failed to generate path map, end position outside the map!";

            // Used to check if end coordinates is in a wall
            if (mColorArray[yEnd, xEnd] == WallColor)
                return "Failed to generate path map, end position is in a wall!";

            // Sets the end cordinate to distance- and color array		
            mColorArray[yEnd, xEnd] = EndColor;
            mDistanceArray[yEnd, xEnd] = ENDPOS;
            GenerateNewChunks(xEnd, yEnd);

            // Generates the distances
            // While loop ends if coordinate memory is empty
            while ((xMemory.Count != 0) || (yMemory.Count != 0)) // *** Path generation map starts here ***
            {
                // Sets the CHUNK_IN_QUEUE to be a NEW_CHUNK to be used	
                for (int i = xMemory.Count - 1; i > -1; i--)
                {
                    mDistanceArray[yMemory[i], xMemory[i]] = NEW_CHUNK;
                    xNewMemory.Add(xMemory[i]); yNewMemory.Add(yMemory[i]);
                    xMemory.RemoveAt(i); yMemory.RemoveAt(i);
                }

                // Counts the distance
                pCurrentDistance += 1;

                // Replaces the NEW_CHUNK to be a distance			
                for (int i = xNewMemory.Count - 1; i > -1; i--)
                {
                    mDistanceArray[yNewMemory[i], xNewMemory[i]] = pCurrentDistance;
                    GenerateNewChunks(xNewMemory[i], yNewMemory[i]);
                    xNewMemory.RemoveAt(i); yNewMemory.RemoveAt(i);
                }
            }
            return "The path map has been generated successfully!";
        }

        // Used to reset old path map
        private void ResetPathMap()
        {
            // Reset distance
            pCurrentDistance = 0;

            // Clear old memory
            xMemory.Clear(); yMemory.Clear();
            xNewMemory.Clear(); yNewMemory.Clear();

            // Sets distance array to UNSET
            for (int y = 0; y < mHeight; y++)
            {
                for (int x = 0; x < mWidth; x++)
                {
                    mDistanceArray[y, x] = UNSET;
                    mColorArray[y, x] = mBitmap.BitmapArray[y, x];
                }
            }
        }

        // Used to set a new chunk to be in queue on the y and x axis		
        private void SetNewChunk(uint x, uint y)
        {
            // Check for out of bounds
            if ((x >= mWidth) || (y >= mHeight))
                return;

            // Checks for valid cell then add coordinate to memory
            if ((mColorArray[y, x] != WallColor) && (mDistanceArray[y, x] == UNSET))
            {
                mDistanceArray[y, x] = CHUNK_IN_QUEUE;
                xMemory.Add(x); yMemory.Add(y);
            }
            // If cell is a wall then mark it as solid
            else if (mColorArray[y, x] == WallColor)
                mDistanceArray[y, x] = SOLID;
        }

        // Calls SetNewChunk multiple times
        private void GenerateNewChunks(uint x, uint y)
        {
            for (int i = -1; i < 2; i++)
            {
                SetNewChunk((uint)((int)(i + x)), y);
                SetNewChunk(x, (uint)(int)(i + y));
                if (pStrafe == true)
                    SetNewChunk((uint)((int)(i + x)), (uint)(int)(i + y));
            }
        }

        /***********************************************************************************************************************/
        /// <summary>
        /// Used to generate path from start position
        /// <para>Returns a list array of x cordinates, y cordinates and direction index</para>
        /// </summary>
        public List<uint>[] GeneratePath(uint xStart, uint yStart)
        {
            // Generate path coordinate list
            List<uint>[] pathCordinates = new List<uint>[3];

            // Check if out of bounds
            if ((xStart >= mWidth) || (yStart >= mHeight))
                return pathCordinates;

            // Check if start position is in a invalid cell
            if ((mDistanceArray[yStart, xStart] == SOLID) || (mDistanceArray[yStart, xStart] == UNSET))
                return pathCordinates;

            // Set list definition
            pathCordinates[LIST_XCOORD] = new List<uint>();
            pathCordinates[LIST_YCOORD] = new List<uint>();
            pathCordinates[LIST_DIRECTION] = new List<uint>();

            // Path coordinates
            uint xPathCheck = xStart;
            uint yPathCheck = yStart;
            uint xClosest = 0, yClosest = 0;

            // Set path values
            uint pCurrentValue = 0;
            uint pLowestValue = 0xFFFFFFFF;

            // Sets start position color
            mColorArray[yStart, xStart] = StartColor;

            // Used to check lowest distance value
            bool CheckLowestValue(uint x, uint y)
            {
                // Check if out of bounds
                if ((x >= mWidth) || (y >= mHeight))
                    return false;

                // Checks if the values is lower than the previews value
                pCurrentValue = mDistanceArray[y, x];
                if (pCurrentValue <= pLowestValue)
                {
                    pLowestValue = pCurrentValue;
                    xClosest = x; yClosest = y;
                }

                // Stops the while loop when it hits the end position
                if (mDistanceArray[y, x] == ENDPOS)
                    return true;
                return false;
            }

            while (true) // *** Path generation starts here ***
            {
                // Checks strafemode state
                switch (pStrafe)
                {
                    case true: // Used to check the surrounding cells for lowest value in all directions
                        for (uint i = 0; i < 3; i++)
                            if ((CheckLowestValue(xPathCheck + i - 1, yPathCheck)) || (CheckLowestValue(xPathCheck, yPathCheck + i - 1)) || (CheckLowestValue(xPathCheck + i - 1, yPathCheck + i - 1)))
                                return pathCordinates;
                        break;
                    case false: // Used to check the surrounding cells for lowest value horzontal and vertical direction only
                        for (uint i = 0; i < 3; i++)
                            if ((CheckLowestValue(xPathCheck + i - 1, yPathCheck)) || (CheckLowestValue(xPathCheck, yPathCheck + i - 1)))
                                return pathCordinates;
                        break;
                }

                if (ListCoordinates == true)
                {
                    // Updates list
                    pathCordinates[LIST_XCOORD].Add(xClosest);
                    pathCordinates[LIST_YCOORD].Add(yClosest);
                    pathCordinates[LIST_DIRECTION].Add((uint)GetDirection((int)(xPathCheck - xClosest), (int)(yPathCheck - yClosest)));
                }

                // Sets path color and updates position to closest cell 
                mColorArray[yClosest, xClosest] = PathColor;
                xPathCheck = xClosest; yPathCheck = yClosest;
            }
        }

        // Used to get the direction
        private PathDirection GetDirection(int x, int y)
        {
            if ((x == 0) && (y == 1)) return PathDirection.Up;
            if ((x == 0) && (y == -1)) return PathDirection.Down;
            if ((x == -1) && (y == 0)) return PathDirection.Right;
            if ((x == 1) && (y == 0)) return PathDirection.Left;
            if ((x == -1) && (y == 1)) return PathDirection.TopRightCorner;
            if ((x == -1) && (y == -1)) return PathDirection.LowerRightCorner;
            if ((x == 1) && (y == 1)) return PathDirection.TopLeftCorner;
            if ((x == 1) && (y == -1)) return PathDirection.LowerLeftCorner;
            return 0;
        }
    }
}



