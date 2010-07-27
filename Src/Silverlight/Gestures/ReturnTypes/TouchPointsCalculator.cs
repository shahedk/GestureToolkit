using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.Generic;
using TouchToolkit.GestureProcessor.Objects;

namespace TouchToolkit.GestureProcessor.ReturnTypes
{
    public class TouchPointsCalculator : IReturnTypeCalculator
    {
        public IReturnType Calculate(ValidSetOfTouchPoints set)
        {
            //Assumption: This return type is for single touch points 
            //and only returns all points of the first touch 
            TouchPoints points = new TouchPoints();
            if (set.Count > 0)
            {
                foreach (var item in set[0].Stroke.StylusPoints)
                {

                    points.Add(new Point(item.X, item.Y));
                }

            }
            return points;
        }
    }
}
