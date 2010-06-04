using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using TouchToolkit.GestureProcessor.Objects;
using TouchToolkit.GestureProcessor.Utility;

namespace TouchToolkit.Framework.ShapeRecognizers
{
    /* Algorithm to help identify circles given a set of points.*/
    public class HoughCircle : IShapeRecognizer
    {
        private double deg_to_radians = Math.PI / 180;
        private int tol = 5;
        private double threshhold = 0.4;
        private bool IsCircle;
        private TouchPoint2 points;
        private int[,,] Accumulator;
        private int binmax;
        private int xmin;
        private int xmax;
        private int ymin;
        private int ymax;
        private int rmax;

        public HoughCircle(TouchPoint2 p)
        {
            points = p;
            IsCircle = false;
            FindRanges();
            binmax = 0;
            int xsize = xmax - xmin;
            int ysize = ymax - ymin;
            Accumulator = new int[xsize, ysize, rmax];
            for (int x = 0; x < xsize; x++)
            {
                for (int y = 0; y < ysize; y++)
                {
                    for (int r = 0; r < rmax; r++)
                    {
                        Accumulator[x, y, r] = 0;
                    }
                }
            }
            FindCircle();
        }

        public bool IsMatch()
        {
            return IsCircle;
        }

        //Find smallest/largest x,y values of our points
        private void FindRanges()
        {
            xmin = (int)points.Stroke.StylusPoints[0].X; xmax = (int)points.Stroke.StylusPoints[0].X;
            ymin = (int)points.Stroke.StylusPoints[0].Y; ymax = (int)points.Stroke.StylusPoints[0].Y;

            foreach (var point in points.Stroke.StylusPoints)
            {
                if (point.X < xmin)
                    xmin = (int)point.X;
                if (point.X > xmax)
                    xmax = (int)point.X;
                if (point.Y < ymin)
                    ymin = (int)point.Y;
                if (point.Y > ymax)
                    ymax = (int)point.Y;
            }
            rmax = (int)TrigonometricCalculationHelper.GetDistanceBetweenPoints(new Point(xmin, ymin), new Point(xmax, ymax));
        }

        private TouchPoint2 FindCircle()
        {
            int length = points.Stroke.StylusPoints.Count;
            foreach (var point in points.Stroke.StylusPoints)
            {
                double x = point.X;
                double y = point.Y;

                for (int a = xmin; a < xmax - xmin; a++)
                {
                    for (int b = ymin; b < ymax - ymin; b++)
                    {
                        for (int r = 0; r < rmax; r++)
                        {
                            for (int theta = 0; theta < 360; theta++)
                            {
                                double theta_rad = theta * deg_to_radians;
                                bool xmatch = x - a + r * Math.Cos(theta_rad) < tol;
                                bool ymatch = y - b + r * Math.Sin(theta_rad) < tol;
                                if (xmatch && ymatch)
                                {
                                    //increment accumulator
                                    Accumulator[a, b, r]++;
                                    //check bin size to see if it's max
                                    if (Accumulator[a, b, r] > binmax)
                                    {
                                        binmax = Accumulator[a, b, r];
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            IsCircle = binmax > length * threshhold;

            if (IsCircle)
            {
                return points;
            }
            else
            {
                return points;
            }
        }
    }
}
