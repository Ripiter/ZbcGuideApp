using Plugin.Compass;
using System.Collections.Generic;
namespace ZbcGuideApp
{
    class Direction
    {

        public Direction()
        {
            Plugin.Compass.Abstractions.SensorSpeed sensorSpeed = Plugin.Compass.Abstractions.SensorSpeed.Normal;
            int counter = 0;
            CrossCompass.Current.CompassChanged += (s, e) =>
            {
                compassValue = (int)e.Heading;
                if (compassAvg.Count < 6)
                {
                    compassAvg.Add(compassValue);
                }
                else if (compassAvg.Count == 6)
                {
                    SetAvgValue();
                    compassAvg.Clear();
                    //Debug.WriteLine(compassValue);

                    if (WifiConnection.dValues != null)
                    {
                        if (WifiConnection.dValues.Length - 1 == counter)
                        {
                            counter = 0;
                        }
                        else
                            DoSomething(WifiConnection.dValues[0]);
                    }
                }

            };
            CrossCompass.Current.Start(sensorSpeed);
        }
        List<int> compassAvg = new List<int>();

        private void SetAvgValue()
        {
            int all = 0;
            foreach (var item in compassAvg)
            {
                all += item;
            }
            all = all / compassAvg.Count;

            compassValue = all;

        }
        public delegate void Dirrection_Changed(int x);
        public event Dirrection_Changed Changed;
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
            //if (currentDirection == right)
            //    GoDirection1 = (int)DWay.GoRight;
            //else if (currentDirection == backward)
            //    GoDirection1 = (int)DWay.GoBackward;
            //else if (currentDirection == left)
            //    GoDirection1 = (int)DWay.GoLeft;
            //else
            //    GoDirection1 = 999;
            if (Changed == null)
                return;

            if (currentDirection == right)
                Changed((int)DWay.GoRight);
            else if (currentDirection == backward)
                Changed((int)DWay.GoBackward);
            else if (currentDirection == left)
                Changed((int)DWay.GoLeft);
            else
                Changed(999);
        }

        public void DoSomething(int dValue)
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


            switch (dValue)
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
