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
using System.Windows.Ink;
using TouchToolkit.GestureProcessor.Utility.TouchHelpers;

namespace TouchToolkit.GestureProcessor.Objects
{
    public class TouchPoint2
    {
        /// <summary>
        /// The UIElement where the touch was originated. Generally found by hit-test
        /// </summary>
        public UIElement Source
        {
            get;
            set;
        }

        public Point Position
        {
            get;
            set;
        }

        public TouchAction Action
        {
            get;
            set;
        }

        //Guid _uniqueId = Guid.NewGuid();
        //public Guid UniqueId
        //{
        //    get
        //    {
        //        return _uniqueId;
        //    }
        //}

        private Dictionary<object, dynamic> _tags = new Dictionary<object, dynamic>();
        public Dictionary<object, dynamic> Tags
        {
            get
            {
                return _tags;
            }
        }

        public Stroke Stroke { get; private set; }
        public int TouchDeviceId
        {
            get;
            set;
        }

        public StylusPoint StartPoint
        {
            get
            {
                return Stroke.StylusPoints[0];
            }
        }

        public DateTime StartTime { get; protected set; }
        public DateTime EndTime { get; protected set; }
        public TimeSpan Age
        {
            get
            {
                return EndTime - StartTime;
            }
        }

        public TouchPoint2(TouchInfo info, UIElement source)
        {
            this.Source = source;

#if SILVERLIGHT
            Stroke = new Stroke();
#else
            var stylusPoints = new StylusPointCollection(1);
            stylusPoints.Add(new StylusPoint(info.Position.X, info.Position.Y));
            Stroke = new Stroke(stylusPoints);
#endif

            TouchDeviceId = info.TouchDeviceId;
            StartTime = DateTime.Now;

#if SILVERLIGHT
            UpdateTouchStroke(info);
#endif
            UpdateTouchInfo(info);
        }

        public void Update(TouchInfo info)
        {
            UpdateTouchStroke(info);
            UpdateTouchInfo(info);
        }

        public TouchPoint2 GetRange(int index1, int index2)
        {
            
            TouchInfo info = new TouchInfo();
            info.ActionType = Action.ToTouchAction();
            info.Position = Position;
            info.TouchDeviceId = TouchDeviceId;

            TouchPoint2 output = new TouchPoint2(info, Source);
            StylusPointCollection spc = new StylusPointCollection();
            for(int i = index1; i < index2; i++)
            {
                spc.Add(Stroke.StylusPoints[i]);
            }
            output.Stroke.StylusPoints = spc;
            return output;
        }

        /// <summary>
        /// Return an empty version of this point
        /// </summary>
        public TouchPoint2 GetEmptyCopy()
        {
            TouchInfo info = new TouchInfo();
            info.ActionType = Action.ToTouchAction();
            info.Position = Position;
            info.TouchDeviceId = TouchDeviceId;

            TouchPoint2 output = new TouchPoint2(info, Source);
            return output;
        }

        private void UpdateTouchInfo(TouchInfo info)
        {
            Action = info.ActionType.ToTouchAction();
            Position = info.Position;
            EndTime = DateTime.Now;
        }

        private void UpdateTouchStroke(TouchInfo info)
        {
            // Adds a new point in the stroke collection
            Stroke.StylusPoints.Add(new StylusPoint(info.Position.X, info.Position.Y));
            
        }
    }
}