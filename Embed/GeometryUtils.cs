using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace Qorpent.Embed{
    public struct Point {
        public Point(double x, double y) {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;

        public static Point operator -(Point a, Point b) {
            return new Point {X = a.X - b.X, Y = a.Y - b.Y};
        }
    }

    public static class GeometryUtil {
        private static readonly double ZeroAngleAtan = Math.Atan2(1, 0);
        public static double GetOffsetAngleRadians(Point srcPoint, Point trgPoint) {
            var vector2 = trgPoint - srcPoint;
            var atan = Math.Atan2(vector2.Y, vector2.X);
            return  ZeroAngleAtan - atan;
        }

        public static Point GetRadianceAngledOffset(Point src, double angle, double distance) {
            var realangle =  ZeroAngleAtan - angle; 
            var x = distance*Math.Cos(realangle);
            var y = distance*Math.Sin(realangle);
            return new Point(src.X+x,src.Y+y);
        }

        public static Point GetDegreeAngledOffset(Point src, double angle, double distance) {
            return GetRadianceAngledOffset(src, DegreeToRadian(angle), distance);
        }

        public static double GetOffsetAngleDegree(Point srcPoint, Point trgPoint) {
            return RadianToDegree(GetOffsetAngleRadians(srcPoint, trgPoint));
        }

        public static double DegreeToRadian(double angle)
        {
           return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
    
}