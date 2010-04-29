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
using Framework.Exceptions;

namespace Framework.Utility
{
    public static class TouchPointExtensionMethods
    {
        public static TouchPointCollection ToTouchPointCollection(this List<TouchInfo> self)
        {
            TouchPointCollection list = Activator.CreateInstance(typeof(TouchPointCollection)) as TouchPointCollection;
            foreach (TouchInfo touchInfo in self)
            {
                TouchPoint point = touchInfo.ToTouchPoint();
                list.Add(point);
            }

            return list;
        }

        public static TouchPoint ToTouchPoint(this TouchInfo self)
        {

#if SILVERLIGHT
            TouchPoint point = new TouchPoint();
            point.SetValue("Position", self.Position);
#else
            TouchAction action = self.ActionType.ToTouchAction();
            Rect rect = new Rect(self.Position, self.Position);
            
            TouchPoint point = new TouchPoint(null, self.Position, rect, action);
#endif
            return point;
        }

        /// <summary>
        /// Creates a copy of the list
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ValidSetOfTouchPoints Copy(this List<TouchPoint2> self)
        {
            ValidSetOfTouchPoints copy = new ValidSetOfTouchPoints(self.Count);

            foreach (var item in self)
            {
                copy.Add(item);
            }

            return copy;

        }

        /// <summary>
        /// Reduces the number of points in a stroke of touch without losing any cretical data
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Point[] ToFilteredPoints(this StylusPointCollection self)
        {
            //TODO: This is a place holder, Ideally, this function should filter and reduce the ammount of points to increase the performance
            Point[] filteredPoints = new Point[self.Count];
            int i = 0;
            foreach (var item in self)
            {
                filteredPoints[i++] = new Point(item.X, item.Y);
            }

            return filteredPoints;
        }

        /// <summary>
        /// Adds the items from the specified collection into existing collection
        /// </summary>
        /// <param name="self"></param>
        /// <param name="itemsToAdd"></param>
        public static void Add(this ValidSetOfPointsCollection self, ValidSetOfPointsCollection itemsToAdd)
        {
            foreach (ValidSetOfTouchPoints item in itemsToAdd)
            {
                self.Add(item);
            }
        }

        public static Point ToPoint(this StylusPoint point)
        {
            // TODO: move to more appropriate location [FYI, same extension method also exists in Gesture assembly]
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Determines if the specified type implements certain interface
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool IsTypeOf(this Type type, Type interfaceType)
        {
            // TODO: move to more appropriate location [FYI, same extension method also exists in Gesture assembly]
            bool result = false;
            Type[] interfaces = type.GetInterfaces();
            foreach (var i in interfaces)
            {
                if (i.IsAssignableFrom(interfaceType))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Removes matching items of specified list from existing list
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sets"></param>
        public static void Remove(this ValidSetOfTouchPoints self, ValidSetOfPointsCollection sets)
        {
            foreach (var set in sets)
            {
                foreach (var point in set)
                {
                    self.Remove(point);
                }
            }
        }

        public static TouchAction2 ToTouchActions(this TouchAction action)
        {
            if (action == TouchAction.Down)
                return TouchAction2.Down;
            else if (action == TouchAction.Up)
                return TouchAction2.Up;
            else if (action == TouchAction.Move)
                return TouchAction2.Move;
            else
                throw new FrameworkException("Unknown touch action type");
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
                throw new FrameworkException("Unknown touch action type");
        }
    }
}
