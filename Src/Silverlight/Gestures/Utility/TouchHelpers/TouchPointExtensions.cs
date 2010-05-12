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
using Gestures.Objects;
using Gestures.Exceptions;

namespace Gestures.Utility.TouchHelpers
{
    public static class TouchPointExtensions
    {
        public static TouchAction2 ToTouchAction(this TouchAction action)
        {
            if (action == TouchAction.Down)
                return TouchAction2.Down;
            else if (action == TouchAction.Up)
                return TouchAction2.Up;
            else if (action == TouchAction.Move)
                return TouchAction2.Move;
            else
                throw new InvalidDataSetException("Unknown touch action type");
        }

        public static TouchAction ToTouchAction(this TouchAction2 action)
        {
            if (action == TouchAction2.Down)
                return TouchAction.Down;
            else if (action == TouchAction2.Up)
                return TouchAction.Up;
            else if (action == TouchAction2.Move)
                return TouchAction.Move;
            else
                throw new InvalidDataSetException("Unknown touch action type");
        }
    }
}
