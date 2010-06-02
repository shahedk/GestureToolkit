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
using TouchToolkit.GestureProcessor.Utility.ShapeRecognizers;

/* Helper class that implements the Hough Transform algorithm to find an image with a line*/
namespace TouchToolkit.Framework.ShapeRecognizers
{
    public class HoughLine : IShapeRecognizer
    {
        private double THRESHHOLD = .4; //Value between 0 and 1
        private MyLine[,] AccumulatorArray; // A(theta, r)
        private MyLine[,] ClusteredArray;
        private MyLine[] Lines;
        private TouchPoint2 Points;
        private bool IsLine;
        private bool IsRect;

        public int Theta;
        public int R;
        public int LineQuorum;

        public HoughLine(TouchPoint2 p)
        {
            IsRect = false;
            Lines = new MyLine[4];
            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = null;
            }
            Points = p;
            LineQuorum = 0;
            Theta = 0;
            R = 0;
            BuildAccumulatorArray();
        }

        public bool IsMatch()
        {
            return IsLine;
        }

        public bool IsRectangle()
        {
            return IsRect;
        }

        private MyLine[,] InitializeArray(int size1, int size2)
        {
            MyLine[,] array = new MyLine[size1, size2];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = new MyLine(i, j);
                }
            }
            return array;
        }

        private void BuildAccumulatorArray()
        {
            double DegToRad = Math.PI / 180;
            int range = GetRange(); //Finds the upperbound of R

            //** Initialize Array(s) **//
            AccumulatorArray = InitializeArray(361, range);

            ClusteredArray = InitializeArray(13, (int)Math.Ceiling(range/100) + 1);

            //** Begin Voting **//
            for (int theta = 0; theta < 361; theta++)
            {
                int length = Points.Stroke.StylusPoints.Count;
                for (int i = 0; i < length; i++)
                {
                    //** Precise Accumulator **//
                    double x = Points.Stroke.StylusPoints[i].X;
                    double y = Points.Stroke.StylusPoints[i].Y;
                    double r = Math.Abs(x * Math.Cos(theta * DegToRad) +
                               y * Math.Sin(theta * DegToRad));
                    MyLine thisLine = AccumulatorArray[theta, (int)r];
                    thisLine.Quorum++;

                    if (thisLine.Quorum > LineQuorum)
                    {
                        R = (int) thisLine.R;
                        Theta = thisLine.Theta;
                        LineQuorum = thisLine.Quorum;
                    }
                    AccumulatorArray[theta, (int)r] = thisLine;
                }
            }

            //** Publish Results into IsLine **//
            int total = Points.Stroke.StylusPoints.Count;
            double determinant = THRESHHOLD * total;
            IsLine = LineQuorum > determinant;
            
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
    }
}
