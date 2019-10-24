using System;
using System.Collections.Generic;
using System.Text;

namespace ZbcGuideApp
{
    class PathFinding
    {
        // Variables
        int xStartPos, yStartPos;
        int xEndPos, yEndPos;
        int xPathCheck, yPathCheck;
        int mapWidth, mapHeight;
        uint solidValue;
        uint pathValue;
        const uint emptyValue = 0;

        // Arrays
        uint[,] mapArray;
        uint[,] mapDistanceArray;

        // Enums
        enum PATH : uint
        {
            SOLID = 65536,
            STARTPOS = 65535,
            ENDPOS = 65534,
            NEW_CHUNK = 65533,
            CHUNK_IN_QUEUE = 64432
        };

        // Property
        #region property
        public uint[,] MapArray
        {
            get { return mapArray; }
            set { mapArray = value; }
        }
        

        #endregion

        /// <summary>
        /// Used to generate new map
        /// /// </summary>
        /// <param name="filename"></param>
        public void GenerateNewMap(int mapHeight, int mapWidth, uint solidValue, uint pathValue, uint[,] mapArray)
        {
            // Sets map dimensions
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;

            // Sets the defaultsoild value and path value
            this.solidValue = solidValue;
            this.pathValue = pathValue;

            // Sets array sizes
            this.mapArray = new uint[mapWidth, mapHeight];
            mapDistanceArray = new uint[mapWidth, mapHeight];

            // Sets the values to this map array from map array
            for (int y = 0; y < mapHeight; y++)
                for (int x = 0; x < mapWidth; x++)
                    this.mapArray[x, y] = mapArray[x, y];
        }

        /// <summary>
        /// Used set a value that the path finding algorithm should avoid
        /// </summary>
        /// <param name="filename"></param>
        public void AddNewSolid(uint[] valueToAvoid)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    for (int i = 0; i < valueToAvoid.Length; i++)
                        if (mapArray[x, y] == valueToAvoid[i]) mapArray[x, y] = solidValue;
                }
            }
        }

        // Used to discover new chunks and set them to be in queue
        void NewPathQueue(int xCurrent, int yCurrent)
        {
            // Sets a new chunk to be in queue on the y axis
            for (int y = -1; y < 2; y++)
                if ((mapArray[y + yCurrent, xCurrent] != solidValue) && (mapDistanceArray[y + yCurrent, xCurrent] == emptyValue))
                    mapDistanceArray[y + yCurrent, xCurrent] = (uint)PATH.CHUNK_IN_QUEUE;

            // Sets a new chunk to be in queue on the x axis
            for (int x = -1; x < 2; x++)
                if ((mapArray[yCurrent, x + xCurrent] != solidValue) && (mapDistanceArray[yCurrent, x + xCurrent] == emptyValue))
                    mapDistanceArray[yCurrent, x + xCurrent] = (uint)PATH.CHUNK_IN_QUEUE;
        }

        /// <summary>
        /// Used to generate a path between to cordinates
        /// </summary>
        /// <param name="filename"></param>
        public void GeneratePath(int xStart, int yStart, int xEnd, int yEnd, bool strafeMode)
        {
            // Sets start and end cordinates 
            xStartPos = xStart;
            yStartPos = yStart;
            xEndPos = xEnd;
            yEndPos = yEnd;

            // Variables
            uint lowestValue = 65536;
            uint currentValue = 0;
            uint currentDistance = 0;

            // Debug! sets a start and end cordinate in the map array
            mapArray[yStartPos, xStartPos] = (uint)PATH.STARTPOS;
            mapArray[yEndPos, xEndPos] = (uint)PATH.ENDPOS;

            // Sets a start cordinate
            mapDistanceArray[yStartPos, xStartPos] = (uint)PATH.STARTPOS;
            NewPathQueue(xStartPos, yStartPos);

            // Generates the distances
            while (mapDistanceArray[yEndPos, xEndPos] == emptyValue)
            {
                // Sets the CHUNK_IN_QUEUE to be a NEW_CHUNK to be used
                for (int y = 0; y < mapHeight; y++)
                    for (int x = 0; x < mapWidth; x++)
                        if (mapDistanceArray[y, x] == (uint)PATH.CHUNK_IN_QUEUE)
                            mapDistanceArray[y, x] = (uint)PATH.NEW_CHUNK;

                currentDistance += 1; // Counts the distance

                // Replaces the NEW_CHUNK to be a distance
                for (int y = 0; y < mapHeight; y++)
                    for (int x = 0; x < mapWidth; x++)
                        if (mapDistanceArray[y, x] == (uint)PATH.NEW_CHUNK)
                        {
                            mapDistanceArray[y, x] = currentDistance;
                            NewPathQueue(x, y);
                        }
            }
            // Sets the pathchecker position to endposition
            xPathCheck = xEndPos;
            yPathCheck = yEndPos;

            // Generates the path
            while (true)
            {
                lowestValue = 65536; // Sets the lowestValue to highest possible

                // When stafeMode is false then the path is allowed to go all directions
                if (strafeMode == true)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            // Checks what value is lowest in the neighbour spaces
                            if (mapDistanceArray[y + yPathCheck, x + xPathCheck] != emptyValue)
                            {
                                currentValue = mapDistanceArray[y + yPathCheck, x + xPathCheck];
                                if (currentValue < lowestValue)
                                    lowestValue = currentValue;
                            }
                            // Stops the while loop when it hits startpos
                            if (mapDistanceArray[y + yPathCheck, x + xPathCheck] == (uint)PATH.STARTPOS)
                                return;
                        }
                    }
                }
                else   // When stafeMode is true then the path is only allowed to go up & down, left & right
                {
                    // First checks y axis
                    for (int y = -1; y < 2; y++)
                    {
                        // Checks what value is lowest in the neighbour spaces
                        if (mapDistanceArray[y + yPathCheck, xPathCheck] != emptyValue)
                        {
                            currentValue = mapDistanceArray[y + yPathCheck, xPathCheck];
                            if (currentValue < lowestValue)
                                lowestValue = currentValue;
                        }
                        // Stops the while loop when it hits startpos
                        if (mapDistanceArray[y + yPathCheck, xPathCheck] == (uint)PATH.STARTPOS)
                            return;
                    }

                    // then checks x axis
                    for (int x = -1; x < 2; x++)
                    {
                        // Checks what value is lowest in the neighbour spaces
                        if (mapDistanceArray[yPathCheck, x + xPathCheck] != emptyValue)
                        {
                            currentValue = mapDistanceArray[yPathCheck, x + xPathCheck];
                            if (currentValue < lowestValue)
                                lowestValue = currentValue;
                        }
                        // Stops the while loop when it hits startpos
                        if (mapDistanceArray[yPathCheck, x + xPathCheck] == (uint)PATH.STARTPOS)
                            return;
                    }
                }
                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        if (mapDistanceArray[y + yPathCheck, x + xPathCheck] == lowestValue)
                        {
                            // Set a marker
                            mapArray[y + yPathCheck, x + xPathCheck] = pathValue;

                            // Move path check
                            xPathCheck += x;
                            yPathCheck += y;

                            // Stop loop
                            y = 2; x = 2;
                        }
                    }
                }
            }
        }

        public  List<int> xvalues = new List<int>();
        public  List<int> yvalues = new List<int>();
        /// <summary>
        /// Used to draw the map 
        /// </summary>
        /// <param name="filename"></param>
        public void DrawMap()
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    //if (mapArray[x, y] == solidValue)
                    //    Console.Write("■");

                    //if (mapArray[x, y] == (uint)PATH.STARTPOS)
                    //    Console.Write("S");

                    //if (mapArray[x, y] == (uint)PATH.ENDPOS)
                    //    Console.Write("G");

                    //if (mapArray[x, y] == pathValue)
                    //    Console.Write("*");

                    if (mapArray[x, y] == pathValue)
                    {
                        xvalues.Add(x);
                        yvalues.Add(y);
                    }

                    //if (mapArray[x, y] == 0xFFFFFF)
                    //    Console.Write(" ");
                }
                //Console.WriteLine();
            }
        }
    }
}

