namespace ZbcGuideApp
{
    class Direction
    {
        public enum Direct
        {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        }

        public enum DWay
        {
            GoLeft = -1,
            GoBackward = 0,
            GoRight = 1
        }

        int[] distanceArray = { 2, 2, 5, 5, 7, 7, 4, 4 };
        string[] compass = { "NORTH", "EAST", "SOUTH", "WEST" };

        int[] sampleArray = { 2, 5, 7, 4 };
        int compassValue = 0;
        int compasDirection; ///To do add direction from adroid

        int currentLocation = 0;
        int currentDirection = 0;

        int goDirection = 0;

        public int GoDirection1 { get => goDirection; set => goDirection = value; }

        bool CheckDirection(int given, int xDegree, int yDegree)
        {
            if ((given >= xDegree) && (given < yDegree))
                return true;
            return false;
        }

        int DirectionValue(int n, int e, int s, int w)
        {
            switch (compasDirection)
            {
                case (int)Direct.North:
                    return n;

                case (int)Direct.East:
                    return e;

                case (int)Direct.South:
                    return s;

                case (int)Direct.West:
                    return w;
            }
            return 0;
        }

        void GoDirection(int left, int right, int backward)
        {
            if (currentDirection == right)
                GoDirection1 = (int)DWay.GoRight;
            else if (currentDirection == backward)
                GoDirection1 = (int)DWay.GoBackward;
            else if (currentDirection == left)
                GoDirection1 = (int)DWay.GoLeft;
            else
                GoDirection1 = 999;
        }

        public void DoSomething()
        {
            if (compassValue > 359)
                compassValue = 0;

            if (compassValue < 0)
                compassValue = 359;

            if ((compassValue >= 315) || (compassValue < 45))
                compasDirection = (int)Direct.North;
            if (CheckDirection(compassValue, 45, 135))
                compasDirection = (int)Direct.East;
            if (CheckDirection(compassValue, 135, 225))
                compasDirection = (int)Direct.South;
            if (CheckDirection(compassValue, 225, 315))
                compasDirection = (int)Direct.West;


            currentDirection = DirectionValue(2, 5, 7, 4);

            //#define M_NORTH 2
            //#define M_EAST 5
            //#define M_SOUTH 7
            //#define M_WEST 4
            //switch (distanceArray[currentLocation])
            //{
            //    case M_NORTH: GoDirection(M_EAST, M_WEST, M_SOUTH); break;
            //    case M_EAST: GoDirection(M_NORTH, M_SOUTH, M_WEST); break;
            //    case M_SOUTH: GoDirection(M_EAST, M_WEST, M_NORTH); break;
            //    case M_WEST:
            //        GoDirection(M_SOUTH, M_NORTH, M_EAST); break;


            switch (distanceArray[currentLocation])
            {
                case 2:
                    GoDirection(5, 4, 7);
                    break;
                case 5:
                    GoDirection(2, 7, 4);
                    break;
                case 7:
                    GoDirection(5, 4, 2);
                    break;
                case 4:
                    GoDirection(7, 2, 5);
                    break;
            }
        }

    }
}
