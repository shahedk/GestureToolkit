using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchToolkit.GestureProcessor.Objects;
using System.Windows.Input;
using TouchToolkit.GestureProcessor.Utility;

namespace TouchToolkit.Framework.ShapeRecognizers
{
    //Attempts to find the circle of best fit given a set of points using a (modified) Least Squares approach after a polar transform
    class CircleRecognizer
    {
        public double Radius; //An estimation of the radius of the circle of best fit
        public double X_Center; //An estimation of the center
        public double Y_Center;
        public double R;

        public CircleRecognizer(TouchPoint2 points)
        {
            double xavg = 0;
            double yavg = 0;
            foreach (var p in points.Stroke.StylusPoints)
            {
                xavg += p.X;
                yavg += p.Y;
            }
            xavg = xavg / points.Stroke.StylusPoints.Count;
            yavg = yavg / points.Stroke.StylusPoints.Count;

            TouchPoint2 PolarTransform = points.GetEmptyCopy();
            StylusPoint center = new StylusPoint(xavg, yavg);
            foreach (var p in points.Stroke.StylusPoints)
            {
                double r = TrigonometricCalculationHelper.GetDistanceBetweenPoints(center, p);
                double xdelta = p.X - center.X;
                double ydelta = p.Y - center.Y;
                double theta = Math.Atan2(ydelta, xdelta);

                PolarTransform.Stroke.StylusPoints.Add(new StylusPoint(theta, r));
            }
            Correlation corr = new Correlation(PolarTransform);

            R = corr.RSquared;
            X_Center = xavg;
            Y_Center = yavg;
            Radius = corr.Intercept;
        }
    }
}
