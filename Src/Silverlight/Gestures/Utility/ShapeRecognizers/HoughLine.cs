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

using System.Collections.Generic;
using TouchToolkit.GestureProcessor.Objects;

/* Helper class that implements the Hough Transform algorithm to find an image with a line*/
namespace TouchToolkit.Framework.ShapeRecognizers
{
    public class HoughLine : IShapeRecognizer
    {
        private double THRESHHOLD = .4; //Value between 0 and 1
        private int[,] AccumulatorArray; // A(theta, r)
        private TouchPoint2 Points;
        private int MaxValue = 0;
        private int Theta;
        private double R;
        private bool IsLine;

        public HoughLine(TouchPoint2 p)
        {
            Points = p;
            BuildAccumulatorArray();
        }

        public bool IsMatch()
        {
            return IsLine;
        }

        private int GetRange()
        {
            double miny = 0; double minx = 0; double maxy = 0; double maxx = 0;
            int length = Points.Stroke.StylusPoints.Count;
            for (int i = 0; i < length; i++)
            {
                double x = Points.Stroke.StylusPoints[i].X;
                double y = Points.Stroke.StylusPoints[i].Y;

                if (x > maxx)
                {
                    maxx = x;
                }
                if (y > maxy)
                {
                    maxy = y;
                }
                if (x < minx)
                {
                    minx = x;
                }
                if (y < miny)
                {
                    miny = y;
                }
            }

            double deltax = maxx - minx;
            double deltay = maxy - miny;

            return (int)Math.Ceiling(Math.Sqrt(Math.Pow(deltax, 2) + Math.Pow(deltay, 2)));
        }

        private void BuildAccumulatorArray()
        {
            TouchPoint2 ret;
            AccumulatorArray = new int[361, GetRange()];
            for (int i = 0; i < AccumulatorArray.GetLength(0); i++)
            {
                for (int j = 0; j < AccumulatorArray.GetLength(1); j++)
                {
                    AccumulatorArray[i ,j] = 0;
                }
            }
            for (int theta = 0; theta < 361; theta++)
            {
                int length = Points.Stroke.StylusPoints.Count;
                for (int i = 0; i < length; i++)
                {
                    double x = Points.Stroke.StylusPoints[i].X;
                    double y = Points.Stroke.StylusPoints[i].Y;
                    double r = Math.Abs(x * Math.Cos(theta) +
                               y * Math.Sin(theta));

                    AccumulatorArray[theta, (int)r]++;

                    if (AccumulatorArray[theta, (int)r] > MaxValue)
                    {
                        MaxValue = AccumulatorArray[theta, (int)r];
                        Theta = theta;
                        R = r;
                    }
                }
            }
            double determinant = THRESHHOLD * Points.Stroke.StylusPoints.Count;
            IsLine = MaxValue > determinant;
        }
    }
}
