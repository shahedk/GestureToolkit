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

namespace TouchToolkit.Framework.ShapeRecognizers
{
    public class Correlation
    {
        public bool VerticalLine;
        public double SlopeRad;
        public double Slope;
        public double Intercept;

        public double RSquared;

        private double xavg;
        private double yavg;
        public Correlation(TouchPoint2 points)
        {
            VerticalLine = false;

            xavg = 0;
            yavg = 0;

            foreach (var p in points.Stroke.StylusPoints)
            {
                xavg += p.X;
                yavg += p.Y;
            }
            xavg = xavg / points.Stroke.StylusPoints.Count;
            yavg = yavg / points.Stroke.StylusPoints.Count;

            double numerator = 0;
            double denominator = 0;
            foreach (var p in points.Stroke.StylusPoints)
            {
                numerator += (p.X - xavg) * (p.Y - yavg);
                denominator += Math.Pow(p.X - xavg,2);
            }

            SlopeRad = Math.Atan2(numerator, denominator);
            if (denominator != 0)
            {
                Slope = numerator / denominator;
                Intercept = yavg - Slope * xavg;
            }
            else
            {
                VerticalLine = true;
            }

            RSquared = CalculateRSquared(points);
        }

        private double CalculateRSquared(TouchPoint2 points)
        {
            if (VerticalLine)
            {
                return 1;
            }
            //Sum of Squares
            double total = 0;
            double residual = 0;

            foreach (var p in points.Stroke.StylusPoints)
            {
                residual += Math.Pow(p.Y - F(p.X),2);
                total += Math.Pow(p.Y - yavg,2);
            }

            return 1 - (residual/total);
        }

        private double F(double x)
        {
            return Intercept + Slope * x;
        }
    }
}
