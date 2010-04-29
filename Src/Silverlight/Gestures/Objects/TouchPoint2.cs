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
using System.Windows.Ink;

namespace Gestures.Objects
{
    public class TouchPoint2
    {
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

        Guid _uniqueId = Guid.NewGuid();
        public Guid UniqueId
        {
            get
            {
                return _uniqueId;
            }
        }

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

        public TouchPoint2(TouchInfo info)
        {

#if SILVERLIGHT
            Stroke = new Stroke();
#else
            Stroke = new Stroke(new StylusPointCollection());
#endif

            TouchDeviceId = info.TouchDeviceId;
            StartTime = DateTime.Now;

            UpdatePosition(info);
        }

        public void UpdatePosition(TouchInfo info)
        {
            // Adds a new point in the stroke collection
            Stroke.StylusPoints.Add(new StylusPoint(info.Position.X, info.Position.Y));

            Action = (TouchAction)info.ActionType;
            Position = info.Position;
            EndTime = DateTime.Now;
        }
    }
}
