using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Gestures.Utility
{
    public class TrigonometricCalculationHelper
    {

        public static double GetSlopBetweenPoints(Point p1, Point p2)
        {
            return GetSlopBetweenPoints(new StylusPoint(p1.X, p1.Y), new StylusPoint(p2.X, p2.Y));
        }

        // NOTE: The overload with 'StylusPoint' is used more frequently than the other one.
        public static double GetSlopBetweenPoints(StylusPoint p1, StylusPoint p2)
        {
            double m = (p1.Y - p2.Y) / (p1.X - p2.X);
            double slope = Math.Atan(m) * 180 / 3.14;

            //if (p1.X < p2.X)
            //    slope += 180;

            return slope;
        }

        public static double GetDistanceBetweenPoints(Point p1, Point p2)
        {
            return GetDistanceBetweenPoints(new StylusPoint(p1.X, p1.Y), new StylusPoint(p2.X, p2.Y));
        }

        public static double GetDistanceBetweenPoints(StylusPoint p1, StylusPoint p2)
        {
            double xDiff = p1.X - p2.X;
            double yDiff = p1.Y - p2.Y;

            double distance = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

            return Math.Abs(distance);
        }

    }
}
