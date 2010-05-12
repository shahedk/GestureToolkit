﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Gestures.Objects
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
    }
}
