﻿using System;

namespace Assets.Scripts.DataClasses
{
    public class Line
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }
        public Enums.LineOrientation Orientation { get; set; }

        public override string ToString()
        {
            return "Line from " + X1 + "," + Y1 + " to " + X2 + "," + Y2 + " " + Orientation;
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    [Serializable]
    public class RealPoint
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
