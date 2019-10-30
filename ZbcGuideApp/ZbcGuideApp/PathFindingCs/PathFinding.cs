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

        //Coordinate memory
        List<int> xMemory = new List<int>();
        List<int> yMemory = new List<int>();
        List<int> xNewMemory = new List<int>();
        List<int> yNewMemory = new List<int>();

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
        /// Used to generate new map /// </summary>
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
        /// Used set a value that the path finding algorithm should avoid/// </summary>
        /// <param name="filename"></param>
        public void AddNewSolid(uint[] valueToAvoid)
        {
            for (int y = 0; y < mapHeight; y++)
                for (int x = 0; x < mapWidth; x++)
                    for (int i = 0; i < valueToAvoid.Length; i++)
                        if (mapArray[x, y] == valueToAvoid[i]) mapArray[x, y] = solidValue;
        }
        public List<int> xValues = new List<int>();
        public List<int> yValues = new List<int>();
        /// <summary>
        /// Used to draw the map /// </summary>
        /// <param name="filename"></param>
        public void DrawMap()
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {

                    if (mapArray[x, y] == pathValue) {
                        xValues.Add(x);
                        yValues.Add(y);
                    }
                }
                Console.WriteLine();
            }
        }

        //**********************************************************************************************//
        // PATH LOGIC MAIN                                                                              //
        //**********************************************************************************************//

        // Used to check if y position is out of bounds
        bool PositionValidY(int y)
        {
            if ((y < mapHeight) && (y > -1))
                return true;
            else
                return false;
        }

        // Used to check if x position is out of bounds
        bool PositionValidX(int x)
        {
            if ((x < mapWidth) && (x > -1))
                return true;
            else return false;
        }

        // Used to discover new chunks and set them to be in queue
        void NewPathQueue(int xCurrent, int yCurrent)
        {
            // Sets a new chunk to be in queue on the y axis
            for (int y = -1; y < 2; y++)
            {

                // Check if position is out of bounds at y axis)
                if (PositionValidY(y + yCurrent))
                {
                    if ((mapArray[y + yCurrent, xCurrent] != solidValue) &&                   // Checks if positions is not a solid wall,
                        (mapDistanceArray[y + yCurrent, xCurrent] == emptyValue) && (y != 0)) // cell dosn't have a distance value and avoid the center 0 
                    {                                                                             // Example
                        mapDistanceArray[y + yCurrent, xCurrent] = (uint)PATH.CHUNK_IN_QUEUE;     // 0 Q 0
                        xMemory.Add(xCurrent);                                                    // 0 S 0
                        yMemory.Add(y + yCurrent);                                                // 0 Q 0
                    }
                }
            }

            // Sets a new chunk to be in queue on the x axis
            for (int x = -1; x < 2; x++)
            {
                // Check if position is out of bounds at x axis)
                if (PositionValidY(x + xCurrent))
                {
                    if ((mapArray[yCurrent, x + xCurrent] != solidValue) &&                   // Checks if positions is not a solid wall,
                        (mapDistanceArray[yCurrent, x + xCurrent] == emptyValue) && (x != 0)) // cell dosn't have a distance value and avoid the center 0 
                    {                                                                             // Example
                        mapDistanceArray[yCurrent, x + xCurrent] = (uint)PATH.CHUNK_IN_QUEUE;     // 0 Q 0
                        xMemory.Add(x + xCurrent);                                                // Q S Q
                        yMemory.Add(yCurrent);                                                    // 0 Q 0
                    }
                }
            }
        }

        /// <summary>
        /// Used to generate a path between to cordinates/// </summary>
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

            // Clear old content in the lists, if any.
            for (int i = xMemory.Count - 1; i > -1; i--)
            {
                xMemory.RemoveAt(i);
                yMemory.RemoveAt(i);
            }
            for (int i = xNewMemory.Count - 1; i > -1; i--)
            {
                xNewMemory.RemoveAt(i);
                yNewMemory.RemoveAt(i);
            }

            // Debug! sets a start and end cordinate in the map array
            mapArray[yStartPos, xStartPos] = (uint)PATH.STARTPOS;
            mapArray[yEndPos, xEndPos] = (uint)PATH.ENDPOS;

            // Sets a start cordinate                                                               // Example
            mapDistanceArray[yStartPos, xStartPos] = (uint)PATH.STARTPOS;                           // E 0 0 0 0 0 0 0 0	  E = END	 Q = NEW_CHUNK_IN_QUEUE
            NewPathQueue(xStartPos, yStartPos);                                                     // 0 0 0 # # # 0 0 Q	  S = START	 N = NEW_CHUNK
                                                                                                    // 0 0 0 0 0 0 0 Q S	  # = SOLID
                                                                                                    // Generates the distances
            while (mapDistanceArray[yEndPos, xEndPos] == emptyValue)
            {
                // Checks if path is valid
                if (xMemory.Count == 0) { Console.WriteLine("NO POSSIBLE WAY!"); return; }

                // Sets the NEW_CHUNK_IN_QUEUE to be a NEW_CHUNK to be used	
                for (int i = xMemory.Count - 1; i > -1; i--)
                {                                      // Example
                    if (mapDistanceArray[yMemory[i], xMemory[i]] == (uint)PATH.CHUNK_IN_QUEUE)     // E 0 0 0 0 0 0 0 0   E 0 0 0 0 0 0 0 N   E 0 0 0 0 0 0 N 2   E 0 0 0 0 0 N 3 2   E 0 0 0 N 5 4 3 2   E 0 0 N 6 5 4 3 2   E 0 N 7 6 5 4 3 2   E N 8 7 6 5 4 3 2
                    {                                                                              // 0 0 0 # # # 0 0 N   0 0 0 # # # 0 N 1   0 0 0 # # # N 2 1   0 0 0 # # # 3 2 1   0 0 0 # # # 3 2 1   0 0 N # # # 3 2 1   0 N 7 # # # 3 2 1   N 8 7 # # # 3 2 1
                        mapDistanceArray[yMemory[i], xMemory[i]] = (uint)PATH.NEW_CHUNK;           // 0 0 0 0 0 0 0 N S   0 0 0 0 0 0 N 1 S   0 0 0 0 0 N 2 1 S   0 0 0 0 N 3 2 1 S   0 0 N 5 4 3 2 1 S   0 N 6 5 4 3 2 1 S   N 7 6 5 4 3 2 1 S   8 7 6 5 4 3 2 1 S

                        // Memory change
                        xNewMemory.Add(xMemory[i]);
                        yNewMemory.Add(yMemory[i]);
                        xMemory.RemoveAt(i);
                        yMemory.RemoveAt(i);
                    }
                }
                currentDistance += 1; // Counts the distance

                // Replaces the NEW_CHUNK to be a distance			
                for (int i = xNewMemory.Count - 1; i > -1; i--)
                {                                   // Example
                    if (mapDistanceArray[yNewMemory[i], xNewMemory[i]] == (uint)PATH.NEW_CHUNK)    // E 0 0 0 0 0 0 0 Q   E 0 0 0 0 0 0 Q 2   E 0 0 0 0 0 Q 3 2   E 0 0 0 0 Q 4 3 2   E 0 0 Q 6 5 4 3 2   E 0 Q 7 6 5 4 3 2   E Q 8 7 6 5 4 3 2   E 9 8 7 6 5 4 3 2
                    {                                                                              // 0 0 0 # # # 0 Q 1   0 0 0 # # # Q 2 1   0 0 0 # # # 3 2 1   0 0 0 # # # 3 2 1   0 0 Q # # # 3 2 1   0 Q 7 # # # 3 2 1   Q 8 7 # # # 3 2 1   9 8 7 # # # 3 2 1
                        mapDistanceArray[yNewMemory[i], xNewMemory[i]] = currentDistance;          // 0 0 0 0 0 0 Q 1 S   0 0 0 0 0 Q 2 1 S   0 0 0 0 Q 3 2 1 S   0 0 0 Q 4 3 2 1 S   0 Q 6 5 4 3 2 1 S   Q 7 6 5 4 3 2 1 S   8 7 6 5 4 3 2 1 S   8 7 6 5 4 3 2 1 S
                        NewPathQueue(xNewMemory[i], yNewMemory[i]);

                        // Memory change
                        xNewMemory.RemoveAt(i);
                        yNewMemory.RemoveAt(i);
                    }
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
                            // Check if position is out of bounds at x- and y axis 
                            if ((PositionValidX(x + xPathCheck)) && (PositionValidY(y + yPathCheck)))
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
                }
                else   // When stafeMode is true then the path is only allowed to go up & down, left & right
                {
                    // First checks y axis
                    for (int y = -1; y < 2; y++)
                    {
                        // Check if position is out of bounds at y axis)
                        if (PositionValidY(y + yPathCheck))
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
                    }

                    // Then checks x axis
                    for (int x = -1; x < 2; x++)
                    {
                        // Check if position is out of bounds at x axis
                        if (PositionValidX(x + xPathCheck))
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
                }

                // Then set a marker and go one forward in the path
                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        // Check if position is out of bounds at x- and y axis 
                        if ((PositionValidX(x + xPathCheck)) && (PositionValidY(y + yPathCheck)))
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
        }
    }
}

