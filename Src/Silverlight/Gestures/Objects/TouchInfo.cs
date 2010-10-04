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

namespace TouchToolkit.GestureProcessor.Objects
{
    public enum TouchAction2
    {
        Down = 1,
        Move = 2,
        Up = 3
    }

    public class TouchInfo
    {
        public int TouchDeviceId { get; set; }
        public TouchAction2 ActionType { get; set; }
        public Point Position { get; set; }
        public Dictionary<string, string> Tags = new Dictionary<string, string>();

        private int _groupId = 0;
        public int GroupId
        {
            get
            {
                return _groupId;
            }
            set
            {
                _groupId = value;
            }
        }

        public override string ToString()
        {
            return string.Format("Id:{0} Point:{1},{2} Action:{3} Group:{4}", TouchDeviceId, Position.X, Position.Y, ActionType.ToString(), GroupId);
        }
    }
}
