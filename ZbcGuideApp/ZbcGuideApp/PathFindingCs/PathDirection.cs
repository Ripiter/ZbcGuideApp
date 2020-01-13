using Plugin.Compass;
using System.Diagnostics;
using System.Collections.Generic;
using Plugin.Compass.Abstractions;
namespace ZbcGuideApp
{
  
    //class PathDirection
    //{
    //    public PathDirection()
    //    {
    //        // set speed on how fast the compass updates
    //        SensorSpeed sensorSpeed = SensorSpeed.Normal;

    //        CrossCompass.Current.CompassChanged += (s, e) =>
    //        {
    //            compassValue = (int)e.Heading - 60;
    //            GetDirection(WifiConnection.dValues[0]);

    //        };
    //        CrossCompass.Current.Start(sensorSpeed);
    //    }
    //    public delegate void Dirrection_Changed(int x);
    //    public event Dirrection_Changed GoDirection;

    //    // Constants
    //    const int NORTH = 2;
    //    const int EAST = 5;
    //    const int SOUTH = 7;
    //    const int WEST = 4;

    //    const int GO_LEFT = -1;
    //    const int GO_BACKWARD = 0;
    //    const int GO_RIGHT = 1;

    //    int compassValue = 0;
    //    int compassDirection = 0;

    //    bool SetCompassDirection(int given, int xDegree, int yDegree)
    //    {
    //        if ((given >= xDegree) && (given < yDegree))
    //            return true;
    //        return false;
    //    }

    //    void SetGoDirection(int left, int right, int backward)
    //    {
    //        if (GoDirection == null)
    //            return;

    //        GoDirection(999);
    //        if (compassDirection == right)
    //            GoDirection(GO_RIGHT);
    //        if (compassDirection == backward)
    //            GoDirection(GO_BACKWARD);
    //        if (compassDirection == left)
    //            GoDirection(GO_LEFT);
    //    }

    //    public int GetDirection(int dValue)
    //    {
    //        // This is used to change the compass value if higher than 359 and lower than 0
    //        if (compassValue > 359) compassValue = 0;
    //        if (compassValue < 0) compassValue = 359;

    //        // This is defines what values is north
    //        if ((compassValue >= 315) || (compassValue < 45))
    //            compassDirection = NORTH;

    //        // This is defines what values is east
    //        if (SetCompassDirection(compassValue, 45, 135))
    //            compassDirection = EAST;

    //        // This is defines what values is south
    //        if (SetCompassDirection(compassValue, 135, 225))
    //            compassDirection = SOUTH;

    //        // This is defines what values is west
    //        if (SetCompassDirection(compassValue, 225, 315))
    //            compassDirection = WEST;

    //        // This is used to set what direction you have to go
    //        switch (dValue)
    //        {
    //            case NORTH: SetGoDirection(EAST, WEST, SOUTH); break;
    //            case EAST: SetGoDirection(NORTH, SOUTH, WEST); break;
    //            case SOUTH: SetGoDirection(EAST, WEST, NORTH); break;
    //            case WEST: SetGoDirection(SOUTH, NORTH, EAST); break;
    //        }
    //        return compassValue;
    //    }
    //}
}
