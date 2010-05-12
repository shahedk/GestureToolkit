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
using Gestures.Objects;

namespace Gestures.ReturnTypes
{
    public class PositionCalculator : IReturnTypeCalculator
    {
        /// <summary>
        /// Returns top-left position
        /// </summary>
        /// <param name="relatedTouches"></param>
        /// <returns></returns>
        public IReturnType Calculate(ValidSetOfTouchPoints set)
        {
            if (set.Count > 0)
            {

                Position p = new Position();
                p.X = set[0].Stroke.GetBounds().Left;
                p.Y = set[0].Stroke.GetBounds().Top;

                return p;
            }
            else
            {
                return null;
            }
        }


    }
}
