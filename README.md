# ZbcGuideApp

This is a school projekt that is made to help people that are not from around here, to locate the place they need to be.

## Xamarin-forms (currently only android)

We do not have the tools to develop on IOS 

## How our projekt works
We get a mac-address of a known wifi network place is on our map of school and from placing it on our map we get x and y
that we can give to that mac address 


Image:

![image](https://raw.githubusercontent.com/Ripiter/ZbcGuideApp/master/ZbcGuideApp/ZbcGuideApp.Android/Assets/mapOfRoskilde.bmp)

Sample of json file
Json:
``` Json
  "/*Mac-address here */: {
    "city": "/*Name of place*/",
    "name": "/*Name of network*/",
    "floor": "1",
    "x": "392", // position of this point on map
    "y": "484"
```

We read values from json string and if its know we put it in a list 
if its not know we get a error 
``` Csharp
private void SerializeJson(string mac, int strenght)
{
     try
     {
          JObject json = JObject.Parse(jsonString);
          JToken jToken = json[mac];

          int x = (int)jToken["x"];
          int y = (int)jToken["y"];
          Debug.WriteLine($"x {x} y {y} of {mac}");
          testData.Add(new AccesPoint { X = x, Y = y, Strenght = strenght });
     }
     catch
     {

    }
}
```

### Known Issue 
Depending on where you work/study you can encounter with encrypted mac-address that is changing once in a while 


## Triangulating your position
The s1, s2, s3 are the strenght of the known access points combined with x and y of the first 3 known accesspoints
```
private void XyCords(int s1, int s2, int s3)
{
      int x = 0;
      int y = 0;

      double px = ((s1 * s1) - (s2 * s2) + (testData[1].X * testData[1].X)) / ((double)(2 * testData[1].X));

      double py = ((s1 * s1) - (s3 * s3) + (testData[2].X * testData[2].X) + (testData[2].Y * testData[2].Y)) / (2 * testData[2].Y) - (testData[2].X / (double)testData[2].Y) * px;

      px = px * 2.05;
      py = py * 2.09;

      x = (int)px;
      y = (int)py;

      GeneratePath(x, y);
}
```
### Known Issue pt.1
```
      px = px * 2.05;
      py = py * 2.09;
```
The 2.05 and 2.09 are offset values for the x and y. The calculation that we made from strength and x, y position are off by avg. of 2
Feel free to change the values to the ones that fits better(from our expirence theses numbers are most accurate)

### Known Issue pt.2
If user is moving while the triangulation is ongoing, it completely gets lost and gets x and y value way off.

## Libery to draw path
When user wants to see map and the path that was found.
We are using SkiaSharp to draw the path with the values we got from pathfinding 
and drawing the map that u can see above

```Charp
SKImageInfo info = args.Info;
SKSurface surface = args.Surface;
SKCanvas canvas = surface.Canvas;

canvas.Clear();
topOffset = info.Height / 5;

canvas.DrawBitmap(resourceBitmap, new SKRect(0, info.Height / 5f, info.Width, 2 * info.Height / 2.5f));

for (int i = 0; i < WifiConnection.xValues.Length; i++)
{
      canvas.DrawPoint(WifiConnection.xValues[i] + 3, WifiConnection.yValues[i] + topOffset + 20, SKColor.Parse("#ff0000"));
}
```

### Known Issue:
Sometimes error may occure when x and y values where not set to reference of the object 
(our best guess is that the main thread didnt get enough time to set x and y values)


TODO: Draw Uml diagram for the projekt



MIT License

Copyright (c) 2019 Peter Cholub

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

